using UnityEngine;
using System;

namespace Enemies
{
    public class EnemyController : MonoBehaviour
    {
        public event Action<EnemyController> OnDeath;
        
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private MeshRenderer meshRenderer;

        public Transform Target { get; private set; }
        public float MoveSpeed => moveSpeed;
        public int PoolIndex { get; private set; }

        private IEnemyState _currentState;
        private bool _isActive;

        public void Initialize(Transform target, int poolIndex)
        {
            Target = target;
            PoolIndex = poolIndex;
            _isActive = true;

            
            ChangeState(new EnemyWanderState());
        }

        private void Update()
        {
            if (!_isActive) return;
            _currentState?.Update(this);
        }

        public void ChangeState(IEnemyState newState)
        {
            _currentState?.Exit(this);
            _currentState = newState;
            _currentState?.Enter(this);
        }

        public void Die()
        {
            _isActive = false;
            _currentState?.Exit(this);
            _currentState = null;
            OnDeath?.Invoke(this);
        }

        private void OnMouseDown()
        {
            Die();
        }
    }
}
