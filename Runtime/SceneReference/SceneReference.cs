using UnityEngine;
using UnityEditor;
using System;

namespace Pastime.SceneLoader {
    /// <summary>
    /// A class that holds a reference to a scene, allowing it to be serialized and deserialized.
    /// </summary>
    [Serializable]
    public class SceneReference : ISerializationCallbackReceiver {
#if UNITY_EDITOR
        /// <summary>
        /// The scene asset in the editor.
        /// </summary>
        [SerializeField] private UnityEngine.Object sceneAsset; // Change SceneAsset to UnityEngine.Object
#endif
        /// <summary>
        /// The path to the scene.
        /// </summary>
        [SerializeField] private string scenePath = string.Empty;

        /// <summary>
        /// The GUID of the scene.
        /// </summary>
        [SerializeField] private string sceneGuid = string.Empty;

        /// <summary>
        /// Gets the path to the scene.
        /// </summary>
        public string ScenePath => scenePath;

        /// <summary>
        /// Gets the GUID of the scene.
        /// </summary>
        public string SceneGuid => sceneGuid;

        /// <summary>
        /// Gets the name of the scene.
        /// </summary>
        public string SceneName => System.IO.Path.GetFileNameWithoutExtension(scenePath);

#if UNITY_EDITOR
        /// <summary>
        /// Updates the scene path and GUID based on the scene asset.
        /// </summary>
        private void UpdateScenePathAndGuid() 
        {
            if (sceneAsset != null) {
                scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                sceneGuid = AssetDatabase.AssetPathToGUID(scenePath);
            }
        }

        /// <summary>
        /// Called when the script is loaded or a value changes in the inspector.
        /// </summary>
        public void OnValidate() {
            UpdateScenePathAndGuid();
        }
#endif

        /// <summary>
        /// Called before the object is serialized.
        /// </summary>
        public void OnBeforeSerialize() {
#if UNITY_EDITOR
            UpdateScenePathAndGuid();
#endif
        }

        /// <summary>
        /// Called after the object is deserialized.
        /// </summary>
        public void OnAfterDeserialize() {
#if UNITY_EDITOR
            // After deserialization, load the scene asset from the path if it's missing
            EditorApplication.delayCall += () => {
                if (string.IsNullOrEmpty(scenePath) && !string.IsNullOrEmpty(sceneGuid)) {
                    scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                }

                if (!string.IsNullOrEmpty(scenePath)) {
                    // Reload the scene asset using the path as a UnityEngine.Object
                    sceneAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(scenePath);
                }
            };
#endif
        }
    }
}