using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using CesiumForUnity;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class OsmLoader : MonoBehaviour
{
	public TextAsset XmlToLoad;
	public GameObject PrefabToPlace;

	private IEnumerator Start()
	{
		// load the dataset
		var stopwatch = Stopwatch.StartNew();
		var buildings = new List<double3>();
		var trees = new List<double3>();
		{
			// https://github.com/SimonCuddihy/osm-unity/blob/master/Assets/Scripts/MapReader.cs
			var xml = new XmlDocument();
			xml.LoadXml(XmlToLoad.text);
			var nodes = xml.SelectNodes("osm/node").Cast<XmlNode>()
				.ToDictionary(
					node => ulong.Parse(node.Attributes["id"].Value),
					node => new double3(double.Parse(node.Attributes["lon"].Value), double.Parse(node.Attributes["lat"].Value), 0)
				);
			// Debug.Log(string.Join("\n", nodes));

			foreach (var way in xml.SelectNodes("osm/way[tag[@k='building']]").Cast<XmlNode>())
			{
				var count = 0;
				var total = double3.zero;
				foreach (var node in way.SelectNodes("nd").Cast<XmlNode>().SkipLast(1))
				{
					var id = ulong.Parse(node.Attributes["ref"].Value);
					var nodePos = nodes[id];
					total += nodePos;
					count++;
				}

				buildings.Add(total / count);
			}
			// Debug.Log(string.Join("\n", buildings));

			foreach (var way in xml.SelectNodes("osm/way[tag[@k='natural']]").Cast<XmlNode>())
			{
				var boundary = way.SelectNodes("nd").Cast<XmlNode>().SkipLast(1)
					.Select(node => nodes[ulong.Parse(node.Attributes["ref"].Value)])
					.ToList();
				var (minlat, minlon, maxlat, maxlon) = (
					boundary.Min(x => x.y),
					boundary.Min(x => x.x),
					boundary.Max(x => x.y),
					boundary.Max(x => x.x)
				);
				trees.AddRange(boundary);

				var points = new List<double3>();
				const double step = 0.001;
				for (var i = minlon; i <= maxlon; i += step)
				{
					for (var j = minlat; j <= maxlat; j += step)
					{
						var point = new double3(i, j, 0);
						if (PolygonHelper.IsPointInPolygon(boundary, point))
							points.Add(point);
					}
				}

				trees.AddRange(points);
			}

		}
		Debug.Log($"dataset load took {stopwatch.ElapsedMilliseconds}ms with {trees.Count} trees and {buildings.Count} buildings");

		// place prefab
		stopwatch.Restart();
		{
			var georeference = GetComponentInParent<CesiumGeoreference>();
			var tileset = georeference.GetComponentInChildren<Cesium3DTileset>();
			
			// https://github.com/CesiumGS/cesium-unity/pull/507#issuecomment-2380048726
			var task = tileset.SampleHeightMostDetailed(trees.Concat(buildings).ToArray());
			yield return new WaitForTask(task);
			var result = task.Result;
			Debug.Log($"sample success = {result.sampleSuccess.All(x => x)} in {stopwatch.ElapsedMilliseconds}ms");
			foreach (var longLatHeightPoint in result.longitudeLatitudeHeightPositions)
			{
				var prefab = Instantiate(PrefabToPlace, georeference.transform);
				var anchor = prefab.GetComponent<CesiumGlobeAnchor>();
				anchor.longitudeLatitudeHeight = longLatHeightPoint;
			}
		}
		Debug.Log($"place prefab took {stopwatch.ElapsedMilliseconds}ms");
	}
}
