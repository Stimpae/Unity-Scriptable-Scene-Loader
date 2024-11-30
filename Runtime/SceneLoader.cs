
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace ScriptableSceneLoader {
    public class SceneLoader : Singleton<SceneLoader> {
        [FormerlySerializedAs("bootScenePath")]
        [SerializeField] private string bootSceneName;
        
        private SceneGroupLoader m_sceneGroupLoader;
        private bool m_isLoading;
        
        private LoadingScreenPlayer m_loadingScreenPlayer;
        private TransitionPlayer m_transitionPlayer;

        public SceneGroup CurrentSceneGroup { get; private set; }
        public SceneGroup PreviousSceneGroup { get; private set; }
        public event Action OnSceneGroupLoaded;
        
        protected override void Awake() {
            base.Awake();
            
            m_loadingScreenPlayer = GetComponentInChildren<LoadingScreenPlayer>();
            m_transitionPlayer = GetComponentInChildren<TransitionPlayer>();
            m_sceneGroupLoader = new SceneGroupLoader(bootSceneName);
            
            if (!string.IsNullOrEmpty(bootSceneName)) return;
            Debug.LogError("Boot scene name is not set in the SceneLoader. Please set the boot scene name in the " +
                           "SceneLoader inspector.");
        }
        
        public async Task LoadSceneGroup(SceneGroup sceneGroup, Func<Task> postLoadAction = null, bool shouldHandleUnload = true) {
            if(m_isLoading) return;
            m_isLoading = true;

            if (sceneGroup.scenes.Any()) {
        
                // show out transition
        
                LoadingProgress progress = new LoadingProgress();
                if(sceneGroup.UseLoadingScreen()) m_loadingScreenPlayer.PlayLoadingScreen(sceneGroup.loadingProfile, progress);
                await m_sceneGroupLoader?.LoadScenes(sceneGroup, progress, shouldHandleUnload)!;
                if (postLoadAction != null) await postLoadAction();
                if(sceneGroup.UseLoadingScreen()) m_loadingScreenPlayer.StopLoadingScreen();
        
                if (CurrentSceneGroup != null) {
                    PreviousSceneGroup = CurrentSceneGroup;
                }
                CurrentSceneGroup = sceneGroup;
        
                // show in transition
        
                OnSceneGroupLoaded?.Invoke();
                m_isLoading = false;
            }
            else {
                Debug.LogError($"Scene group {sceneGroup.name} has no scenes to load.");
                m_isLoading = false;
            }
        }
        
        public async Task UnloadSceneGroupAsync(SceneGroup sceneGroup) {
            if (sceneGroup.scenes.Any()) {
                foreach (var scene in sceneGroup.scenes) {
                    await SceneManager.UnloadSceneAsync(scene.GetSceneName());
                }
            }
            else {
                Debug.LogError($"Scene group {sceneGroup.name} has no scenes to unload.");
                m_isLoading = false;
            }
        }
    }
}