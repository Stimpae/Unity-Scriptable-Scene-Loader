using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pastime.SceneLoader {
    [CreateAssetMenu(fileName = "SceneGroup_Data", menuName = "Scene Loader/Scenes/Scene Group", order = 0)]
    public class SceneGroup : ScriptableObject {
        public string groupName;
        public TransitionProfile inTransition;
        public TransitionProfile outTransition;
        public LoadingProfile loadingProfile;
        public float loadingDelay = 0.5f;
        public List<SceneData> scenes = new List<SceneData>();
        
        public bool UseLoadingScreen() => loadingProfile != null;
        public bool UseInTransition() => inTransition != null;
        public bool UseOutTransition() => outTransition != null;
    }
    
    [Serializable]
    public class SceneData {
        public SceneReference scene;
        public bool isActiveScene;
    }
}