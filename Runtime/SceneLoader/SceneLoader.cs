using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Pastime.SceneLoader {
    public class SceneLoader : Singleton<SceneLoader> {
        [SerializeField] private SceneReference persistentScene;
        [SerializeField] private List<SceneGroup> sceneGroups;
        
        private SceneGroupLoader m_sceneGroupLoader;
        private bool m_isLoading;
        
        private LoadingScreenPlayer m_loadingScreenPlayer;
        private TransitionPlayer m_transitionPlayer;
        
        public bool IsLoading => m_isLoading;
        public SceneGroup CurrentSceneGroup { get; private set; }
        public SceneGroup PreviousSceneGroup { get; private set; }
        public event Action<SceneGroup> OnSceneGroupLoaded;
        
        protected override void Awake() {
            base.Awake();
            m_loadingScreenPlayer = GetComponentInChildren<LoadingScreenPlayer>();
            
            m_transitionPlayer = GetComponentInChildren<TransitionPlayer>();
            m_sceneGroupLoader = new SceneGroupLoader(persistentScene.SceneName);
            
            if (!string.IsNullOrEmpty(persistentScene.SceneName)) return;
            Debug.LogError("Boot scene name is not set in the SceneLoader. Please set the boot scene name in the " +
                           "SceneLoader inspector.");
        }
        
        public async Task LoadSceneGroup(SceneGroup sceneGroup) {
            if(m_isLoading) return;
            m_isLoading = true;

            if (sceneGroup.scenes.Any()) {
                // show out transition
        
                LoadingProgress progress = new LoadingProgress();
                if(sceneGroup.UseLoadingScreen()) m_loadingScreenPlayer.PlayLoadingScreen(sceneGroup.loadingProfile, progress);
                await m_sceneGroupLoader?.LoadScenes(sceneGroup, progress)!;
                if(sceneGroup.UseLoadingScreen()) m_loadingScreenPlayer.StopLoadingScreen();
        
                if (CurrentSceneGroup != null) {
                    PreviousSceneGroup = CurrentSceneGroup;
                }
                CurrentSceneGroup = sceneGroup;
        
                // show in transition
        
                OnSceneGroupLoaded?.Invoke(sceneGroup);
                m_isLoading = false;
            }
            else {
                Debug.LogError($"Scene group {sceneGroup.name} has no scenes to load.");
                m_isLoading = false;
            }
        }
    }
}