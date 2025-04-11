using UnityEditor;
using UnityEngine;

public static class AddMeshColliders
{
    [MenuItem("Tools/Add Mesh Colliders")]
    public static void AddMeshColliders_()
    {
        foreach (var meshFilter in Selection.activeGameObject.GetComponentsInChildren<MeshFilter>())
        {
            meshFilter.gameObject.AddComponent<MeshCollider>();
        }
    }
}