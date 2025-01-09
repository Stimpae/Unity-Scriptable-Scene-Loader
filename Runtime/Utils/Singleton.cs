using UnityEngine;

namespace Pastime.SceneLoader {
    /// <summary>
    /// A generic singleton class for Unity components.
    /// </summary>
    /// <typeparam name="T">The type of the singleton component.</typeparam>
    public class Singleton<T> : MonoBehaviour where T : Component {
        /// <summary>
        /// The instance of the singleton.
        /// </summary>
        protected static T instance;

        /// <summary>
        /// Gets a value indicating whether an instance of the singleton exists.
        /// </summary>
        public static bool HasInstance => instance != null;

        /// <summary>
        /// Tries to get the instance of the singleton.
        /// </summary>
        /// <returns>The instance of the singleton if it exists; otherwise, null.</returns>
        public static T TryGetInstance() => instance;

        /// <summary>
        /// Gets the current instance of the singleton.
        /// </summary>
        public static T Current => instance;

        /// <summary>
        /// Gets the instance of the singleton, creating it if it does not already exist.
        /// </summary>
        public static T Instance {
            get {
                if (instance == null) {
                    instance = FindAnyObjectByType<T>() ?? CreateNewInstance();
                }
                return instance;
            }
        }

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake() {
            InitializeSingleton();
        }

        /// <summary>
        /// Initializes the singleton instance.
        /// </summary>
        protected virtual void InitializeSingleton() {
            if (Application.isPlaying) {
                instance = this as T;
            }
        }

        /// <summary>
        /// Creates a new instance of the singleton if one does not already exist.
        /// </summary>
        /// <returns>The new instance of the singleton.</returns>
        private static T CreateNewInstance() {
            Debug.LogWarning($"No instance of {typeof(T).Name} found. Creating a new one.");
            GameObject obj = new GameObject($"{typeof(T).Name}_AutoCreated");
            return obj.AddComponent<T>();
        }
    }
}