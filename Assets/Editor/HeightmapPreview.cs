using Terrain;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// Class enabling preview of generated heightmaps
    /// Inefficient "autoreaload" of heightmaps
    /// </summary>
    public class HeightmapPreview : EditorWindow
    {
        [MenuItem ("Debug/Heightmap preview")]
        public static void  ShowWindow () {
            EditorWindow.GetWindow(typeof(HeightmapPreview));
        }

        /// <summary>
        /// Show generated heightmap on editor window
        /// </summary>
        private void OnGUI()
        {
            var generator = FindFirstObjectByType<TerrainGenerator>();
            if (generator == null)
            {
                Debug.LogError("No terrain generator active");
                return;
            }

            int size = 512;
            position = new Rect(position.x, position.y, size, size);
            EditorGUI.DrawPreviewTexture(new Rect(0,0,size, size), generator.PreviewHeightmap(size));
        }
    }
}
