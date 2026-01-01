using UnityEngine;
using TMPro;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private TextMeshProUGUI enemyCountText;
        [SerializeField] private TextMeshProUGUI fpsText;

        private float _fpsTimer;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            // Update FPS every 0.5 seconds to be readable
            _fpsTimer += Time.unscaledDeltaTime;
            if (_fpsTimer > 0.5f)
            {
                float fps = 1.0f / Time.unscaledDeltaTime;
                fpsText.text = $"FPS: {Mathf.Ceil(fps)}";
                _fpsTimer = 0f;
            }
        }

        public void UpdateWaveText(int wave)
        {
            if (waveText) waveText.text = $"Wave: {wave}";
        }

        public void UpdateEnemyCount(int count)
        {
            if (enemyCountText) enemyCountText.text = $"{count}";
        }

        public void OnNextWaveClicked()
        {
            Systems.WaveManager.Instance?.StartNextWave();
        }

        public void OnToggleWavesClicked()
        {
            Systems.WaveManager.Instance?.StopResumeWaves();
        }

        public void OnDestroyWaveClicked()
        {
            Systems.WaveManager.Instance?.DestroyCurrentWave();
        }
    }
}
