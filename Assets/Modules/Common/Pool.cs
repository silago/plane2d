using System.Collections.Generic;
using UnityEngine;
namespace Modules.Common
{
    public class Pool<T> where T : Component 
    {
        private Stack<T> _pool = new Stack<T>();
        private T _prefab;
        private Transform parent;
        public Pool(T prefab, Transform parent)
        {
            _prefab = prefab;
            this.parent = parent;
        }
        public Pool(T prefab)
        {
            _prefab = prefab;
            parent = prefab.transform.parent;
        }

        public T Pick()
        {
            if (_pool.Count == 0) EnlargePool(1);
            var item =  _pool.Pop();
            item.gameObject.SetActive(true);
            return item;
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
                var item = UnityEngine.Object.Instantiate(_prefab, parent);
                item.gameObject.SetActive(false);
                _pool.Push(item);
            }
        }
    }
}
