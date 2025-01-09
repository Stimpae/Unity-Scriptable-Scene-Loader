using System;
using UnityEngine;

namespace Pastime.SceneLoader {
    public class LoadingProgress : IProgress<float> {
        public event Action<float> OnProgressChanged;
        private const float RATIO = 0.9f;
        
        public void Report(float value) {
            OnProgressChanged?.Invoke(value / RATIO);
        }
    }
}