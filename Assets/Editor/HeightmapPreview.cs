using System.IO;
using Terrain;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Class enabling preview of generated heightmaps
/// </summary>
public class HeightmapPreview : EditorWindow
{
    private FileSystemWatcher _watcher;
    private ComputeProxy _computeProxy;
    private bool _valid = false; 
    
    [MenuItem ("Debug/Heightmap preview")]
    public static void  ShowWindow () {
        EditorWindow.GetWindow(typeof(HeightmapPreview));
    }
    
    /// <summary>
    /// Setup for heightmap preview
    /// Setups file watcher to avoid unnecesary redraws
    /// </summary>
    public void Awake()
    {
        _computeProxy = FindFirstObjectByType<ComputeProxy>();
        
        _valid = _computeProxy != null;

        if (!_valid)
        {
            Debug.LogError("No terrain generator active");
            return;
        }
        
        _watcher = new FileSystemWatcher();
        _watcher.NotifyFilter = NotifyFilters.LastWrite;
        var generator = FindFirstObjectByType<ComputeProxy>();
        var shader = generator.TerrainComputeShader;
        var projectPath = Application.dataPath.Replace("Assets", "");
        var path = projectPath + AssetDatabase.GetAssetPath(shader);
        _watcher.Path = Path.GetDirectoryName(path);
        _watcher.Filter = Path.GetFileName(path);
        _watcher.EnableRaisingEvents = true;
        _watcher.Changed += WatcherCallback;
    }

    private void WatcherCallback(object source, FileSystemEventArgs args)
    {
        Debug.Log("Repaint");   
        Repaint();
    }
    
    /// <summary>
    /// Show generated heightmap on editor window
    /// </summary>
    private void OnGUI()
    {
        if(!_valid)
            return;

        const int size = 1024;
        position = new Rect(position.x, position.y, size, size);
        EditorGUI.DrawPreviewTexture(new Rect(0,0,size, size), _computeProxy.PreviewHeightmap(size));
    }
}
