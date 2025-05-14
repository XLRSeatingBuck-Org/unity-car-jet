using System;
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
	public GameObject BuildingPrefab, TreePrefab;

	public bool Loaded = false;
	/*
	private Mesh _mesh;
	private Material[] _materials;
	private Matrix4x4[] _matrices;
	*/

	private IEnumerator Start()
	{
		// load the dataset
		var stopwatch = Stopwatch.StartNew();
		var points = new List<double3>();
		int treeStartIndex;
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

			if (BuildingPrefab != null)
			{
				foreach (var way in xml.SelectNodes("osm/way[tag[@k='building']]").Cast<XmlNode>())
				{
					var count = 0;
					var total = double3.zero;
					foreach (var node in way.SelectNodes("nd").Cast<XmlNode>().SkipLast(1))
					{
						total += nodes[ulong.Parse(node.Attributes["ref"].Value)];
						count++;
					}

					points.Add(total / count);
				}

				// Debug.Log(string.Join("\n", buildings));
			}
			treeStartIndex = points.Count;

			if (TreePrefab != null)
			{
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
					// points.AddRange(boundary);

					var trees = new List<double3>();
					const double step = 0.001;
					for (var i = minlon; i <= maxlon; i += step)
					{
						for (var j = minlat; j <= maxlat; j += step)
						{
							// jitter it a bit
							var point = new double3(i + UnityEngine.Random.value * step, j + UnityEngine.Random.value * step, 0);
							if (PolygonHelper.IsPointInPolygon(boundary, point))
								trees.Add(point);
						}
					}

					points.AddRange(trees);
				}
			}
		}
		Debug.Log($"dataset load took {stopwatch.ElapsedMilliseconds}ms with {points.Count} points");

		// place prefab
		var prefabs = new List<GameObject>();
		stopwatch.Restart();
		{
			var georeference = GetComponentInParent<CesiumGeoreference>();
			var tileset = georeference.GetComponentInChildren<Cesium3DTileset>();

			yield return new WaitForSecondsRealtime(1);
			// https://github.com/CesiumGS/cesium-unity/pull/507#issuecomment-2380048726
			var task = tileset.SampleHeightMostDetailed(points.ToArray());
			yield return new WaitForTask(task);
			var result = task.Result;
			Debug.Log($"sample success = {result.sampleSuccess.All(x => x)} in {stopwatch.ElapsedMilliseconds}ms\n" +
			          $"warnings = {string.Join(", ", result.warnings)}");
			for (var i = 0; i < result.longitudeLatitudeHeightPositions.Length; i++)
			{
				var point = result.longitudeLatitudeHeightPositions[i];
				var isBuilding = i < treeStartIndex;
				var prefab = Instantiate(isBuilding ? BuildingPrefab : TreePrefab, georeference.transform);
				var anchor = prefab.GetComponent<CesiumGlobeAnchor>();
				anchor.longitudeLatitudeHeight = point;
				Destroy(anchor); // these lag and we dont need them if we dont do origin shift
				
				if (isBuilding)
				{
					var scale = prefab.transform.localScale;
					scale.y *= UnityEngine.Random.Range(.5f, 1f);
					scale.x *= UnityEngine.Random.Range(.5f, 1f);
					scale.z *= UnityEngine.Random.Range(.5f, 1f);
					prefab.transform.localScale = scale;
				}
				
				// if (i % 1000 == 0) yield return null;
				prefabs.Add(prefab);
			}
		}
		Debug.Log($"place prefab took {stopwatch.ElapsedMilliseconds}ms");
		Loaded = true;
		
		/*
		// convert to gpu instanced data
		yield return new WaitForSecondsRealtime(1); // let it reposition
		_mesh = TreePrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
		_materials = TreePrefab.GetComponentInChildren<MeshRenderer>().sharedMaterials;
		foreach (var material in _materials)
		{
			material.enableInstancing = true;
		}
		_matrices = prefabs.Select(x => x.GetComponentInChildren<MeshRenderer>().transform.localToWorldMatrix).ToArray();
		
		foreach (var tree in prefabs)
		{
			Destroy(tree);
		}
		*/
	}

	/*
	private void Update()
	{
		if (_materials != null)
		{
			for (var i = 0; i < _materials.Length; i++)
			{
				Graphics.RenderMeshInstanced(new RenderParams(_materials[i]), _mesh, i, _matrices);
			}
		}
	}
	*/
}
