using System;
using System.Collections.Generic;
using System.IO;
using Terrain.Foliage;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Utility
{
    /// <summary>
    /// Component for semi-automatic generation of billboard from models
    /// </summary>
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
        private readonly int _samples = 2;
        
        /// <summary>
        /// Billboard texture resolution
        /// </summary>
        private readonly int _resolution = 1024;

        public MeshRenderer[] previews;
        
        private Bounds _boundingBox;
        
        /// <summary>
        /// Generates billboard material and size
        /// </summary>
        public void GenerateBillboard()
        {
            // Check is there is valid target
            if (!target.foliagePrefab)
                throw new Exception("Foliage Prefab is missing");

            // Create render texture
            RenderTexture renderTarget = new RenderTexture(_resolution, _resolution,32,  RenderTextureFormat.ARGB32);
            renderTarget.Create();
            var lastRenderTarget = RenderTexture.active;
            Camera.main.targetTexture = renderTarget;
            RenderTexture.active = renderTarget;
            
            // Clear previous target
            if (_targetInstance != null)
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
            float size = GetCameraSide(_targetInstance);
            
            // Camera setup
            Camera.main.orthographicSize = size / 2;
            
            cameraDolly.transform.position = new Vector3(
                cameraDolly.transform.position.x,
                size / 2,
                cameraDolly.transform.position.z);
            
            
            // Shoot billboard pictures
            float spacing = 180f / (_samples); // Only take pictures on one half, since billboards render both sides
            for (int i = 0; i < _samples; i++)
            {
                float angle = spacing * i;
                cameraDolly.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                Camera.main.Render();
                
                // Copy from GPU to CPU 
                var texture = textures[i];
                texture.ReadPixels(new Rect(0, 0, _resolution, _resolution), 0, 0);
                texture.Apply();
            }
            
            // Convert to 2d texture array
            Texture2DArray outputTextures = new Texture2DArray(_resolution, _resolution, _samples, TextureFormat.ARGB32, false);
            for (int i = 0; i < _samples; i++)
            {
                var pixels = textures[i].GetPixels();
                outputTextures.SetPixels(pixels, i);
            }
            outputTextures.Apply();

            // Save to asset database
            var raw = AssetDatabase.GetAssetPath(target);
            var path = Path.GetDirectoryName(raw);
            var targetName = Path.GetFileNameWithoutExtension(raw);
            
            AssetDatabase.CreateAsset(outputTextures, $"{path}/{targetName}_lods.asset");
            
            Material material = new Material(Shader.Find("Shader Graphs/BillboardShader"));
            material.SetTexture("_Billboard_texture", outputTextures);
            material.enableInstancing = true;
            AssetDatabase.CreateAsset(material, $"{path}/{targetName}_material.asset");

            target.billboardMaterial = material;
            target.billboardSize = size / 2f;
            
            AssetDatabase.SaveAssets();
            
            // Cleanup
            RenderTexture.active = lastRenderTarget;
            renderTarget.Release();
            
            // Preview the billboard
            foreach (var preview in previews)
            {
                preview.sharedMaterial = material;
                preview.transform.localScale = Vector3.one * size / 2f;
            }
        }

        /// <summary>
        /// Computes bounding box for game object hierarchy
        /// </summary>
        /// <param name="computed">Object for which the AABB will be computed</param>
        /// <returns>AABB</returns>
        Bounds ComputeBoundingBox(GameObject computed)
        {
            List<MeshRenderer> meshes = new(computed.GetComponentsInChildren<MeshRenderer>());
            MeshRenderer origin = computed.GetComponent<MeshRenderer>();
            if(origin)
                meshes.Add(origin);
            
            Bounds bounds = new();
            foreach (var mesh in meshes)
                bounds.Encapsulate(mesh.bounds);
            
            return bounds;
        }

        /// <summary>
        /// Figures out size of the camera needed to fit the model
        /// </summary>
        /// <param name="computed"></param>
        /// <returns>Size</returns>
        private float GetCameraSide(GameObject computed)
        {
            Bounds bounds = ComputeBoundingBox(computed);
            float[] comps =
            {
                bounds.max.x,
                bounds.max.y,
                bounds.max.z,
            };
            return Mathf.Max(comps);
        }
        
        /// <summary>
        /// Debug view of the AABBs
        /// </summary>
        private void OnDrawGizmos()
        {
            if(!_targetInstance)
                return;
            
            Bounds bounds = ComputeBoundingBox(_targetInstance);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            
            float size = GetCameraSide(_targetInstance);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(Vector3.up * size / 2, Vector3.one * size);
        }
    }
}
#endif