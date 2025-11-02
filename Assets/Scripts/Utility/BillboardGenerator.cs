using System.IO;
using Terrain.Foliage;
using UnityEditor;
using UnityEngine;

namespace Utility
{
    [ExecuteInEditMode]
    public class BillboardGenerator : MonoBehaviour
    {
        [Tooltip("Target for which the billboard will be generated")]
        public Foliage target;
        
        [Tooltip("Camera parent used for moving the camera around target")]
        public Transform cameraDolly;
        
        /// <summary>
        /// Reference to create target instance
        /// </summary>
        private GameObject _targetInstance;
        
        /// <summary>
        /// Number of billboards
        /// </summary>
        private readonly int _samples = 4;
        
        /// <summary>
        /// Billboard texture resolution
        /// </summary>
        private readonly int _resolution = 1024;
        
        public void GenerateBillboard()
        {
            // Check is there is valid target
            if (!target.foliagePrefab)
            {
                Debug.LogWarning("Foliage Prefab is missing");
                return;
            }

            // Create tmp render texture
            RenderTexture tmpTexture = new RenderTexture(_resolution, _resolution,32,  RenderTextureFormat.ARGB32);
            tmpTexture.Create();
            
            // Clear previous target
            if (_targetInstance)
                DestroyImmediate(_targetInstance);
            
            // Initialize textures
            Texture2D[] textures = new Texture2D[_samples];
            for (int i = 0; i < _samples; i++)
            {
                Texture2D texture2D = new Texture2D(_resolution, _resolution, TextureFormat.ARGB32, false, true);
                textures[i] = texture2D;
            }

            // Instantiate target
            _targetInstance = Instantiate(target.foliagePrefab);

            
            // Shoot billboards
            Camera.main.targetTexture = tmpTexture;
            RenderTexture.active = tmpTexture;
            float spacing = 180f / (_samples + 1);
            
            for (int i = 0; i < _samples; i++)
            {
                float angle = spacing * i;
                cameraDolly.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                Camera.main.Render();

                var texture = textures[i];
                
                texture.ReadPixels(new Rect(0, 0, _resolution, _resolution), 0, 0);
                texture.Apply();
            }
            
            // Convert to 2d texture array
            Texture2DArray _outputArray = new Texture2DArray(_resolution, _resolution, _samples, TextureFormat.ARGB32, false);
            for (int i = 0; i < _samples; i++)
            {
                var pixels = textures[i].GetPixels();
                _outputArray.SetPixels(pixels, i);
            }
            _outputArray.Apply();

            // Save to asset database
            var raw = AssetDatabase.GetAssetPath(target);
            var path = Path.GetDirectoryName(raw);
            var targetName = Path.GetFileNameWithoutExtension(raw);
            
            AssetDatabase.CreateAsset(_outputArray, $"{path}/{targetName}_lods.asset");
            
            Material material = new Material(Shader.Find("Shader Graphs/BillboardShader"));
            material.SetTexture("_Billboard_texture", _outputArray);
            material.enableInstancing = true;
            AssetDatabase.CreateAsset(material, $"{path}/{targetName}_material.asset");

            target.billboardMaterial = material;
            
            AssetDatabase.SaveAssets();
        }
    }
}
