using System.Linq;
using Thundertale.SceneLoader.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Pastime.SceneLoader.Editor {
    [CustomEditor(typeof(SceneGroup))]
    public class SceneGroupEditor : UnityEditor.Editor {
        private SceneGroup m_sceneGroup;
        
        private VisualElement m_root;

        public override VisualElement CreateInspectorGUI() {
            m_root = new VisualElement();
            
            m_sceneGroup = target as SceneGroup;
            
            m_root.Add(EditorStyleUtils.CreateHelpBox("As you can query the scene loader to load scene groups by name please ensure group names are unique and memorable. " +
                                                      "There is a validation check in the scene loader to ensure duplicates and empties " +
                                                      "do not exist", HelpBoxMessageType.None, new Color32(45, 45, 45, 255), 5));

            var groupName = new TextField("Group Name") {
                style = { marginTop = 3},
                bindingPath = "groupName"
            };
            
            m_root.Add(groupName);
            m_root.Add(EditorStyleUtils.CreateSplitter(6));
            
            var transitionIn = new ObjectField("Transition In") {
                objectType = typeof(TransitionProfile),
                bindingPath = "transitionIn"
            };
            
            m_root.Add(transitionIn);
            
            var transitionOut = new ObjectField("Transition Out") {
                objectType = typeof(TransitionProfile),
                bindingPath = "transitionOut"
            };
            
            m_root.Add(transitionOut);
            m_root.Add(EditorStyleUtils.CreateSplitter(6));
            
            var loadingProfile = new ObjectField("Loading Profile") {
                objectType = typeof(LoadingProfile),
                bindingPath = "loadingProfile"
            };
            
            m_root.Add(loadingProfile);
            
            var loadingDelay = new FloatField("Loading Delay") {
                bindingPath = "loadingDelay"
            };

            m_root.Add(loadingDelay);
            m_root.Add(EditorStyleUtils.CreateSplitter(6));

            var multiColumn = EditorStyleUtils.CreateMultiListView("scenes", "Scenes to Load")
                .AddColumn("scene", "Scenes")
                .AddColumn("isActiveScene", "Active Scene", 20, 100);
            multiColumn.Bind(serializedObject);
            
            var sceneValidationContainer = new VisualElement();
            m_root.RegisterCallback<ChangeEvent<bool>>(evt => {
                sceneValidationContainer.Clear();
                if(CheckActiveScenes(serializedObject.FindProperty("scenes"))) {
                    var sceneValidationHelpBox = new HelpBox("There is more than one scene marked as the active scene, this will" +
                                                             "cause the unexpected behaviour", HelpBoxMessageType.Warning);
                    sceneValidationContainer.Add(sceneValidationHelpBox);
                }else {
                    sceneValidationContainer.Clear();
                }
            });
            
            m_root.Add(sceneValidationContainer);
            m_root.Add(multiColumn);
            return m_root;
        }
        
        private bool CheckActiveScenes(SerializedProperty sceneDataList) {
            int activeScenes = 0;
            for (int i = 0; i < sceneDataList.arraySize; i++) {
                SerializedProperty element = sceneDataList.GetArrayElementAtIndex(i);
                SerializedProperty isActiveScene = element.FindPropertyRelative("isActiveScene");
                if (isActiveScene.boolValue) {
                    activeScenes++;
                }
            }
            return activeScenes > 1;
        }
    }
    
}
