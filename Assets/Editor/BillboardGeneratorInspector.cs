using Terrain.Foliage;
using UnityEditor;
using UnityEngine;
using Utility;

/// <summary>
/// Custom inspector to allow manual firing of billboard generator
/// </summary>
[CustomEditor(typeof(BillboardGenerator))]
public class BillboardGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var instance = target as BillboardGenerator;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Billboard"))
        {
            EditorUtility.SetDirty(instance.target);
            instance.GenerateBillboard();
        }
    }
}
