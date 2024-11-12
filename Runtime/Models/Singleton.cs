using UnityEngine;

namespace ScriptableSceneLoader {
    /// <summary>
    /// A generic Singleton class for Unity components.
    /// Ensures that only one instance of the component exists in the scene.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    public class Singleton<T> : MonoBehaviour where T : Component {
        protected static T instance;
        public static bool HasInstance => instance != null;
        public static T TryGetInstance() => instance;
        
        public static T Instance {
            get {
                if (instance == null) {
                    instance = FindFirstObjectByType<T>() ?? CreateNewInstance();
                }
                return instance;
            }
        }
        
        private static T CreateNewInstance() {
            var go = new GameObject(typeof(T).Name + " Auto-Generated");
            return go.AddComponent<T>();
        }
        
        protected virtual void Awake() {
            if (Application.isPlaying) instance = this as T;
        }
    }
}