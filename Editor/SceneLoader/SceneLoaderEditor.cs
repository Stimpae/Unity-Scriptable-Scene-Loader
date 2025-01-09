using Thundertale.SceneLoader.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pastime.SceneLoader.Editor {
    [CustomEditor(typeof(SceneLoader))]
    public class SceneLoaderEditor : UnityEditor.Editor {
        private SceneLoader m_sceneLoader;
        private VisualElement m_debugContainer;
        
        public override VisualElement CreateInspectorGUI() {
            var root = new VisualElement();
            
            m_sceneLoader = target as SceneLoader;
            
            var sceneReference = serializedObject.FindProperty("persistentScene");
            var sceneReferenceField = new PropertyField(sceneReference);
            
            sceneReferenceField.Bind(serializedObject);
            
            root.Add(sceneReferenceField);
            root.Add(EditorStyleUtils.CreateSplitter(5, 20));
            root.Add(EditorStyleUtils.CreateListView("sceneGroups", "Scene Groups"));
            root.Add(EditorStyleUtils.CreateSplitter(5));
            
            // validation check here whenever a new scene group is added??
            
            // button here to add all scene groups in the project to the scene loader
            
            
            if (!Application.isPlaying) {
                var helpBox = new HelpBox("Debugging information will only show when the game is running", HelpBoxMessageType.Info);
                root.Add(helpBox);
                return root;
            }
            
            m_debugContainer = new VisualElement {
                style = {
                    marginLeft = 3,
                    fontSize = 11
                }
            };

            if (Application.isPlaying) {
                root.schedule.Execute(() => {
                    UpdateLabels(m_sceneLoader.CurrentSceneGroup);
                }).Every(300);
            }
            
            root.Add(m_debugContainer);
            root.Bind(serializedObject);
            return root;
        }
        
        void UpdateLabels(SceneGroup obj) {
            m_debugContainer.Clear();
            var currentSceneGroupLabel = new Label($"Current Scene Group: {m_sceneLoader.CurrentSceneGroup?.name}");
            m_debugContainer.Add(currentSceneGroupLabel);
    
            var previousSceneGroupLabel = new Label($"Previous Scene Group: {m_sceneLoader.PreviousSceneGroup?.name}");
            m_debugContainer.Add(previousSceneGroupLabel);
        }
    }
}