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
		var longLatHeightPoints = new List<double3>();
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
			
			var buildings = xml.SelectNodes("osm/way[tag[@k='building']]").Cast<XmlNode>()
				.Select(way =>
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

					return total / count;
				})
				.ToList();
			// Debug.Log(string.Join("\n", buildings));

			var forests = xml.SelectNodes("osm/way[tag[@k='natural']]").Cast<XmlNode>()
				.Select(way => way.SelectNodes("nd").Cast<XmlNode>().SkipLast(1)
					.Select(node => nodes[ulong.Parse(node.Attributes["ref"].Value)])
					.ToList()
				)
				.ToList();
			// TODO: choose random points in the polygon

			longLatHeightPoints = buildings;
		}
		Debug.Log($"dataset load took {stopwatch.ElapsedMilliseconds}ms");

		// place prefab
		stopwatch.Restart();
		{
			var georeference = GetComponentInParent<CesiumGeoreference>();
			var tileset = georeference.GetComponentInChildren<Cesium3DTileset>();
			
			// https://github.com/CesiumGS/cesium-unity/pull/507#issuecomment-2380048726
			var task = tileset.SampleHeightMostDetailed(longLatHeightPoints.ToArray());
			yield return new WaitForTask(task);
			var result = task.Result;
			Debug.Log($"sample success = {result.sampleSuccess.All(x => x)}");
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
