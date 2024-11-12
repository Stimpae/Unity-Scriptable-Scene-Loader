using UnityEngine;

namespace ScriptableSceneLoader {
    [CreateAssetMenu(fileName = "LoadingProfile_Data", menuName = "Data/Scenes/Loading Profile", order = 0)]
    public class LoadingProfile : ScriptableObject {
        public Sprite loadingScreenSprite;
        // Not sure what else?
    }
}