using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pastime.SceneLoader {
    public class SceneGroupLoader {
        private SceneGroup m_activeSceneGroup;
        private readonly string m_bootSceneName;
        
        public SceneGroupLoader(string bootSceneName) {
          m_bootSceneName = bootSceneName;
        }

        public async Task LoadScenes(SceneGroup group, IProgress<float> progress,bool reloadDuplicateScenes = false) {
            m_activeSceneGroup = group;
            var loadedScenes = Enumerable.Range(0, SceneManager.sceneCount)
                .Select(i => SceneManager.GetSceneAt(i).name)
                .ToList();
            
            await UnloadScenes();
            var operationGroup = new AsyncOperationGroup(m_activeSceneGroup.scenes.Count);

            foreach (var sceneData in m_activeSceneGroup.scenes) {
                if (!reloadDuplicateScenes && loadedScenes.Contains(sceneData.scene.SceneName)) continue;
                var operation = SceneManager.LoadSceneAsync(sceneData.scene.SceneName, LoadSceneMode.Additive);
                operationGroup.operations.Add(operation);
            }
            
            operationGroup.AllowSceneActivation(false);
            while (!operationGroup.IsDone) {
                progress?.Report(operationGroup.Progress);
                if (operationGroup.Progress >= 0.9f) {
                    operationGroup.AllowSceneActivation(true);
                }
                await Task.Yield();
            }
            
            foreach (var sceneData in group.scenes.Where(sceneData => sceneData.isActiveScene)) {
                SceneManager.SetActiveScene(SceneManager.GetSceneByPath(sceneData.scene.ScenePath));
                break;
            }
            
            await Task.Delay((int)(group.loadingDelay * 1000));
        }
        
        private async Task UnloadScenes() {
            
            var scenes = Enumerable.Range(1, SceneManager.sceneCount - 1)
                .Select(SceneManager.GetSceneAt)
                .Where(scene => scene.isLoaded && scene.name != m_bootSceneName)
                .Select(scene => scene.name)
                .ToList();

            var operationGroup = new AsyncOperationGroup(scenes.Count);
            foreach (var scene in scenes) {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null) continue;
                operationGroup.operations.Add(operation);
            }

            while (!operationGroup.IsDone) {
                await Task.Delay(100);
            }
        }

        public class AsyncOperationGroup {
            public readonly List<AsyncOperation> operations;

            public float Progress => operations.Count == 0 ? 0 : operations.Average(o => o.progress);
            public bool IsDone => operations.All(o => o.isDone);

            public AsyncOperationGroup(int initialCapacity) {
                operations = new List<AsyncOperation>(initialCapacity);
            }
            
            public void AllowSceneActivation(bool allow) {
                foreach (var operation in operations) {
                    operation.allowSceneActivation = allow;
                }
            }
        }
    }
}