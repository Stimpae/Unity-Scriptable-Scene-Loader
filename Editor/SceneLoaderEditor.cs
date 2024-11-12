using UnityEditor;
using UnityEngine;

namespace RuntimeUtilities.SceneLoader.Editor {
    [CustomEditor(typeof(ScriptableSceneLoader.SceneLoader))]
    public class SceneLoaderEditor : UnityEditor.Editor {
        private ScriptableSceneLoader.SceneLoader m_sceneLoader;
        
        protected void OnEnable() {
            m_sceneLoader = (ScriptableSceneLoader.SceneLoader)target;
        }
        
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            EditorGUILayout.LabelField("Debugging", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();
            if (!Application.isPlaying) {
                EditorGUILayout.HelpBox("Debugging information will only show when the game is running",
                    MessageType.Warning);
                return;
            }
            
            EditorGUILayout.LabelField("Current Scene Group: ", m_sceneLoader.CurrentSceneGroup?.name);
            EditorGUILayout.LabelField("Previous Scene Group: ", m_sceneLoader.PreviousSceneGroup?.name);
            
            EditorGUILayout.Space();
        }
    }
}