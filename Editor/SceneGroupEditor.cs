using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ScriptableSceneLoader.Editor {
    [CustomEditor(typeof(SceneGroup))]
    public class SceneGroupEditor : UnityEditor.Editor {
        private SceneGroup m_sceneGroup;
        private SerializedProperty m_sceneDataList;
        private ReorderableList m_reorderableList;
        private string[] availableScenes; // Array to hold available scenes for the dropdown
        private int selectedSceneIndex = -1; // Track selected index for the dropdown

        protected void OnEnable() {
            m_sceneGroup = (SceneGroup)target;
            m_sceneDataList = serializedObject.FindProperty("scenes");

            // Find all scenes in the project
            availableScenes = AssetDatabase.FindAssets("t:Scene")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .ToArray();

            // Initialize ReorderableList
            m_reorderableList = new ReorderableList(serializedObject, m_sceneDataList, true, true, false, true) {
                drawElementCallback = DrawListItems,
                drawHeaderCallback = DrawHeader,
                onAddCallback = OnAddElement
            };
        }

        // Draw header for the list
        private void DrawHeader(Rect rect) {
            float iconWidth = 20f;
            float toggleWidth = 100f;
            float pathWidth = rect.width - toggleWidth - iconWidth - 40f;

            EditorGUI.LabelField(new Rect(rect.x + iconWidth + 5f, rect.y, pathWidth, EditorGUIUtility.singleLineHeight), "Scene Path");
            EditorGUI.LabelField(new Rect(rect.x + pathWidth, rect.y, toggleWidth, EditorGUIUtility.singleLineHeight), "Is Active Scene");
        }

        // Draw each element of the list
        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused) {
            SerializedProperty element = m_sceneDataList.GetArrayElementAtIndex(index);
            rect.y += 2;

            // Adjust the layout: More space for scenePath, less for the toggle
            float iconWidth = 20f;
            float toggleWidth = 50f;
            float pathWidth = rect.width - toggleWidth - iconWidth - 120f; // Adjusted width for button

            // Check if the scene is in build settings
            SerializedProperty scenePathProp = element.FindPropertyRelative("scenePath");
            string scenePath = scenePathProp.stringValue;
            bool isInBuildSettings = IsSceneInBuildSettings(scenePath);

            // Draw status icon (tick or cross)
            Rect iconRect = new Rect(rect.x, rect.y, iconWidth, EditorGUIUtility.singleLineHeight);
            switch (isInBuildSettings) {
                case true:
                    EditorGUI.LabelField(iconRect, "✓"); // Tick if in build settings
                    break;
                // Make the row background red if the scene is not in the build settings
                case false:
                    EditorGUI.DrawRect(new Rect(rect.x, rect.y - 1, rect.width, EditorGUIUtility.singleLineHeight + 2f), new Color(1, 0.1f, 0.1f)); // Light red background
                    break;
            }
            
            // Draw the scene path field (takes up most of the space)
            EditorGUI.PropertyField(
                new Rect(rect.x + iconWidth + 5f, rect.y, pathWidth, EditorGUIUtility.singleLineHeight),
                scenePathProp,
                GUIContent.none
            );

            // Draw the isActiveScene toggle (smaller)
            SerializedProperty isActiveScene = element.FindPropertyRelative("isActiveScene");
            bool newIsActive = EditorGUI.Toggle(
                new Rect(rect.x + pathWidth + iconWidth + 10f, rect.y, toggleWidth, EditorGUIUtility.singleLineHeight),
                isActiveScene.boolValue
            );

            // If the toggle has changed, handle the exclusive active logic
            if (newIsActive != isActiveScene.boolValue) {
                isActiveScene.boolValue = newIsActive;
                if (newIsActive) {
                    SetOnlyActiveScene(index);
                }
            }

            // Add button to add scene to build settings if it's not already in it
            if (isInBuildSettings) return;
            if (GUI.Button(new Rect(rect.x + pathWidth + toggleWidth + 30f, rect.y, 80f, EditorGUIUtility.singleLineHeight), "Add to Build")) {
                AddSceneToBuildSettings(scenePath);
            }
        }

        // Ensure only one active scene or none
        private void SetOnlyActiveScene(int activeIndex) {
            for (int i = 0; i < m_sceneDataList.arraySize; i++) {
                if (i == activeIndex) continue; // Skip the one being set as active
                
                SerializedProperty element = m_sceneDataList.GetArrayElementAtIndex(i);
                SerializedProperty isActiveScene = element.FindPropertyRelative("isActiveScene");
                isActiveScene.boolValue = false; // Set all others to false
            }
        }

        // Dropdown to add a new scene from the project
        private void DrawSceneDropdown() {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Add Scene", GUILayout.MaxWidth(70));

            // Create a dropdown with all available scenes
            selectedSceneIndex = EditorGUILayout.Popup(selectedSceneIndex, availableScenes);

            if (GUILayout.Button("Add Scene", GUILayout.MaxWidth(100)) && selectedSceneIndex >= 0) {
                AddSelectedSceneToList();
            }

            GUILayout.EndHorizontal();
        }

        // Add the selected scene to the SceneData list and to Build Settings if necessary
        private void AddSelectedSceneToList() {
            string selectedScenePath = availableScenes[selectedSceneIndex];

            // Add the selected scene to the SceneData list
            int index = m_sceneDataList.arraySize;
            m_sceneDataList.InsertArrayElementAtIndex(index);

            SerializedProperty newElement = m_sceneDataList.GetArrayElementAtIndex(index);
            newElement.FindPropertyRelative("scenePath").stringValue = selectedScenePath;
            newElement.FindPropertyRelative("isActiveScene").boolValue = false;

            serializedObject.ApplyModifiedProperties();
        }

        // Check if a scene is already in the build settings
        private bool IsSceneInBuildSettings(string scenePath) {
            var scenes = EditorBuildSettings.scenes;
            return scenes.Any(s => s.path == scenePath);
        }

        // Add the scene to build settings if it's not already present
        private void AddSceneToBuildSettings(string scenePath) {
            var scenes = EditorBuildSettings.scenes;
            if (IsSceneInBuildSettings(scenePath)) return; // If it's already in build settings, do nothing

            var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
            for (int i = 0; i < scenes.Length; i++) {
                newScenes[i] = scenes[i];
            }
            newScenes[scenes.Length] = new EditorBuildSettingsScene(scenePath, true); // Add new scene

            EditorBuildSettings.scenes = newScenes; // Apply to build settings
        }

        // When a new element is added, initialize it (empty path and isActiveScene = false)
        private void OnAddElement(ReorderableList list) {
            int index = m_sceneDataList.arraySize;
            m_sceneDataList.InsertArrayElementAtIndex(index);
            
            SerializedProperty newElement = m_sceneDataList.GetArrayElementAtIndex(index);
            newElement.FindPropertyRelative("scenePath").stringValue = "";
            newElement.FindPropertyRelative("isActiveScene").boolValue = false;

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            serializedObject.Update();

            if (m_sceneDataList.arraySize > 0) {
                m_reorderableList.DoLayoutList();
            }

            // Add the scene dropdown
            DrawSceneDropdown();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
