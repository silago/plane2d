using System.Collections.Generic;
using UnityEngine;
namespace Modules.Common
{
    public class Pool<T> where T : Component 
    {
        private Stack<T> _pool = new Stack<T>();
        private T _prefab;
        public Pool(T prefab)
        {
            _prefab = prefab;
        }

        public T Pick()
        {
            if (_pool.Count == 0) EnlargePool(1);
            return _pool.Pop();
        }

        public void Return(T item)
        {
            item.gameObject.SetActive(false);
            _pool.Push(item);
        }

        public void EnlargePool(int size)
        {
            for (var i = 0; i < size; i++)
            {
                var item = UnityEngine.Object.Instantiate(_prefab);
                item.gameObject.SetActive(false);
                _pool.Push(item);
            }
        }
    }
}
