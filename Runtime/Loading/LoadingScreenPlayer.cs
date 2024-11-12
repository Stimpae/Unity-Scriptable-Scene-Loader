using System;
using UnityEngine;
using UnityEngine.UI;

namespace ScriptableSceneLoader {
    public class LoadingScreenPlayer : MonoBehaviour{
        [SerializeField] private GameObject loadingCanvas;
        [SerializeField] private Image loadingImage;
        [SerializeField] private Text loadingText; // maybe??
        [SerializeField] private Slider loadingSlider;

        private void Awake() {
            loadingCanvas.SetActive(false);
        }

        public void PlayLoadingScreen(LoadingProfile profile, LoadingProgress progress) {
            loadingCanvas.SetActive(true);
            loadingImage.sprite = profile.loadingScreenSprite;
            
            progress.OnProgressChanged -= HandleSlider;
            progress.OnProgressChanged += HandleSlider;
        }
        
        private void HandleSlider(float value) {
            loadingSlider.value = value;
        }
        
        public void StopLoadingScreen() {
            loadingSlider.value = 0;
            loadingCanvas.SetActive(false);
        }
        
    }
}