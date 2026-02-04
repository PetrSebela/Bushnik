using System.IO;
using Aircraft;
using Aircraft.Airfoil;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor script for converting csv files containing airfoil performance profile to internal representation 
/// </summary>
public class AirfoilParser : MonoBehaviour
{
    /// <summary>
    /// Parses selected csv file and creates file with internal representation
    /// </summary>
    [MenuItem("Assets/Create/Load Airfoil", false, 1)]
    public static void ParseAirfoil()
    {
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        var lines = File.ReadAllLines(path);
        
        var airfoil = ScriptableObject.CreateInstance<AirfoilData>();

        for (int i = 11; i < lines.Length; i++)
        {
            var columns = lines[i].Split(',');
            var alpha = float.Parse(columns[0]); 
            var lift = float.Parse(columns[1]); 
            var drag = float.Parse(columns[2]); 
            airfoil.AddSample(alpha, lift, drag);
        }
        
        var directory =  Path.GetDirectoryName(path);
        var name = Path.GetFileNameWithoutExtension(path);
        var airfoilPath = Path.Combine(directory, name + ".asset");
        
        EditorUtility.SetDirty(airfoil);
        AssetDatabase.CreateAsset(airfoil, airfoilPath);
        AssetDatabase.SaveAssets();
    }
    
    /// <summary>
    /// Checks if selected file is CSV
    /// </summary>
    /// <returns>True is file is .csv</returns>
    [MenuItem("Assets/Create/Load Airfoil", true)]
    public static bool IsCSV()
    {
        if(Selection.activeObject == null)
            return false;
        
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        var extenstion = Path.GetExtension(path);

        return extenstion == ".csv";
    }
}
