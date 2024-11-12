using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ScriptableSceneLoader {
    [CreateAssetMenu(fileName = "SceneGroup_Data", menuName = "Data/Scenes/Scene Group", order = 0)]
    public class SceneGroup : ScriptableObject {
        public TransitionProfile inTransition;
        public TransitionProfile outTransition;
        public LoadingProfile loadingProfile;
        public float loadingDelay = 0.5f;
        [HideInInspector] public List<SceneData> scenes = new List<SceneData>();
        
        public bool UseLoadingScreen() {
            return loadingProfile != null;
        }
        
        public bool UseInTransition() {
            return inTransition != null;
        }
        
        public bool UseOutTransition() {
            return outTransition != null;
        }
        
    }
    
    [Serializable]
    public class SceneData {
        public string scenePath;
        public bool isActiveScene;
        
        // Function to get the scene name from the path
        public string GetSceneName() {
            if (string.IsNullOrEmpty(scenePath)) return string.Empty;

            // Ensure the path ends with '.unity' before extracting the scene name
            if (!scenePath.EndsWith(".unity", StringComparison.OrdinalIgnoreCase)) {
                scenePath += ".unity"; // Append the .unity extension if it's missing
            }

            // Extract the file name without the extension
            return Path.GetFileNameWithoutExtension(scenePath);
        }
    }
}