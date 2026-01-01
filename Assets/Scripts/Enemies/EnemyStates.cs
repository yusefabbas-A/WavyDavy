using UnityEngine;

namespace Enemies
{
    public interface IEnemyState
    {
        void Enter(EnemyController enemy);
        void Update(EnemyController enemy);
        void Exit(EnemyController enemy);
    }

    public class EnemyChaseState : IEnemyState
    {
        public void Enter(EnemyController enemy)
        {
            // Optional: ensuring we have a target or setting up animation trigger
        }

        public void Update(EnemyController enemy)
        {
            if (enemy.Target == null) return;

            // Simple movement towards target
            Vector3 direction = (enemy.Target.position - enemy.transform.position).normalized;
            enemy.transform.position += direction * enemy.MoveSpeed * Time.deltaTime;

            // Look at target
            Vector3 lookTarget = new Vector3(enemy.Target.position.x, enemy.transform.position.y, enemy.Target.position.z);
            enemy.transform.LookAt(lookTarget);
        }

        public void Exit(EnemyController enemy)
        {
            // Cleanup if needed
        }
    }

    public class EnemyWanderState : IEnemyState
    {
        private Vector3 _targetPosition;
        private float _waitTimer;
        private bool _isWaiting;

        public void Enter(EnemyController enemy)
        {
            SetNewRandomTarget(enemy);
        }

        public void Update(EnemyController enemy)
        {
            if (_isWaiting)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0)
                {
                    _isWaiting = false;
                    SetNewRandomTarget(enemy);
                }
                return;
            }

            // Move towards target
            Vector3 direction = (_targetPosition - enemy.transform.position).normalized;
            enemy.transform.position += direction * enemy.MoveSpeed * Time.deltaTime;

            // Look at target
            enemy.transform.LookAt(new Vector3(_targetPosition.x, enemy.transform.position.y, _targetPosition.z));

            // Check if reached
            if (Vector3.Distance(enemy.transform.position, _targetPosition) < 0.5f)
            {
                _isWaiting = true;
                _waitTimer = Random.Range(0.5f, 2f); // Wait for 0.5 to 2 seconds
            }
        }

        public void Exit(EnemyController enemy)
        {
        }

        private void SetNewRandomTarget(EnemyController enemy)
        {
            // Pick a random point within a radius (e.g., 9 units to stay safe on 20-unit platform)
            Vector2 randomCircle = Random.insideUnitCircle * 9f;
            _targetPosition = new Vector3(randomCircle.x, 0, randomCircle.y); 
        }
    }
}
