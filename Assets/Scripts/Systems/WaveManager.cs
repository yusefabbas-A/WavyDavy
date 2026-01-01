using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Enemies;
using UI;

namespace Systems
{
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private EnemyController[] enemyPrefabs;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float timeBetweenWaves = 5f;
        [SerializeField] private float spawnInterval = 0.5f;

        [Header("State")]
        public int CurrentWave = 0;
        public int ActiveEnemyCount = 0;
        public bool IsAutoCycling = true;

        private List<ObjectPool<EnemyController>> _enemyPools;
        private List<EnemyController> _activeEnemies = new List<EnemyController>();
        private bool _isSpawning;
        private Coroutine _spawnRoutine;
        private Coroutine _waveWaitRoutine;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            InitializePools();
        }

        private void Start()
        {
            // Start first wave after a short delay
            _waveWaitRoutine = StartCoroutine(WaitForNextWave(1f));
        }

        private void InitializePools()
        {
            _enemyPools = new List<ObjectPool<EnemyController>>();
            foreach (var prefab in enemyPrefabs)
            {
                // Create a parent for cleanliness
                GameObject poolParent = new GameObject($"Pool_{prefab.name}");
                poolParent.transform.SetParent(transform);
                
                // Initialize pool with 20 items each initially
                var pool = new ObjectPool<EnemyController>(prefab, 20, poolParent.transform);
                _enemyPools.Add(pool);
            }
        }

        public void StartNextWave()
        {
            // Force start next wave immediately
            if (_spawnRoutine != null) StopCoroutine(_spawnRoutine);
            if (_waveWaitRoutine != null) StopCoroutine(_waveWaitRoutine);

            _isSpawning = false; // Reset spawning state
            CurrentWave++;
            _spawnRoutine = StartCoroutine(SpawnWaveRoutine());
        }

        private IEnumerator WaitForNextWave(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (IsAutoCycling)
            {
                StartNextWave();
            }
        }

        private IEnumerator SpawnWaveRoutine()
        {
            _isSpawning = true;
            UIManager.Instance?.UpdateWaveText(CurrentWave);
            
            // Calculate enemy count: 30, 50, 70, then +10 per wave
            int enemyCount;
            if (CurrentWave == 1) enemyCount = 30;
            else if (CurrentWave == 2) enemyCount = 50;
            else if (CurrentWave == 3) enemyCount = 70;
            else enemyCount = 70 + (CurrentWave - 3) * 10;

            for (int i = 0; i < enemyCount; i++)
            {
                if (!GameManager.Instance.IsGameActive) yield break;

                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }

            _isSpawning = false;

            // If all enemies died while we were spawning (unlikely but possible if player kills fast), check now
            CheckWaveCompletion();
        }

        private void SpawnEnemy()
        {
            // Pick random type
            int typeIndex = Random.Range(0, _enemyPools.Count);
            var enemy = _enemyPools[typeIndex].Get();

            // Pick random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = spawnPoint.rotation;

            // Initialize
            enemy.Initialize(GameManager.Instance.transform, typeIndex); 
            enemy.OnDeath += HandleEnemyDeath;
            
            _activeEnemies.Add(enemy);
            ActiveEnemyCount++;
            UIManager.Instance?.UpdateEnemyCount(ActiveEnemyCount);
        }

        private void HandleEnemyDeath(EnemyController enemy)
        {
            enemy.OnDeath -= HandleEnemyDeath;
            _activeEnemies.Remove(enemy);
            
            ReturnEnemyToPool(enemy, enemy.PoolIndex);

            ActiveEnemyCount--;
            UIManager.Instance?.UpdateEnemyCount(ActiveEnemyCount);
             
            CheckWaveCompletion();
        }

        private void CheckWaveCompletion()
        {
            if (ActiveEnemyCount == 0 && !_isSpawning)
            {
                if (IsAutoCycling)
                {
                    if (_waveWaitRoutine != null) StopCoroutine(_waveWaitRoutine);
                    _waveWaitRoutine = StartCoroutine(WaitForNextWave(timeBetweenWaves));
                }
            }
        }
        
        public void ReturnEnemyToPool(EnemyController enemy, int poolIndex)
        {
             if(poolIndex >= 0 && poolIndex < _enemyPools.Count)
                _enemyPools[poolIndex].ReturnToPool(enemy);
        }

        public void StopResumeWaves()
        {
            IsAutoCycling = !IsAutoCycling;
            
            // If we resumed and are sitting idle (no enemies, not spawning), start the timer or next wave
            if (IsAutoCycling && ActiveEnemyCount == 0 && !_isSpawning)
            {
                // Start immediate next wave or wait? 
                // Description says "wave cycles continue from where they left off".
                // If we were waiting, we should probably start.
                StartNextWave(); 
            }
        }

        public void DestroyCurrentWave()
        {
            // 1. Stop spawning interactions
            if (_spawnRoutine != null) StopCoroutine(_spawnRoutine);
            _isSpawning = false;

            // 2. Kill all active enemies
            // Create a copy because HandleDeath modifies the list
            var enemies = new List<EnemyController>(_activeEnemies);
            foreach (var enemy in enemies)
            {
                enemy.Die(); 
            }

            // 3. Logic in HandleDeath/CheckWaveCompletion will handle the rest.
            // If AutoCycle is ON, it will schedule next wave.
            // If AutoCycle is OFF, it will just sit there.
        }
    }
}
