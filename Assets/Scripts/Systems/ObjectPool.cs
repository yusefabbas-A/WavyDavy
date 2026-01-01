using System.Collections.Generic;
using UnityEngine;

namespace Systems
{
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Queue<T> _pool = new Queue<T>();

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
        }

        private T CreateNewObject()
        {
            T instance = Object.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            _pool.Enqueue(instance);
            return instance;
        }

        public T Get()
        {
            if (_pool.Count == 0)
            {
                CreateNewObject();
            }

            T instance = _pool.Dequeue();
            instance.gameObject.SetActive(true);
            return instance;
        }

        public void ReturnToPool(T instance)
        {
            instance.gameObject.SetActive(false);
            _pool.Enqueue(instance);
        }
    }
}
