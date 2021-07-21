#region
using System;
using System.Collections.Generic;
using Events;
using Modules.Common;
using UnityEngine;
#endregion
namespace Modules.UI
{
    public class WorldHud : MonoBehaviour
    {
        [SerializeField]
        private Vector3 offset;
        [SerializeField]
        private HullWidget prefab;
        private readonly Dictionary<int, (Transform,HullWidget)> _items = new Dictionary<int, (Transform,HullWidget)>();

        private Pool<HullWidget> _pool;
        private void Awake()
        {
            this.Subscribe<DisplayHullMessage>(OnHull).BindTo(this);
            this.Subscribe<DamageMessage>(OnDamage).BindTo(this);
            _pool = new Pool<HullWidget>(prefab) {
            };
        }
        private void LateUpdate()
        {
            foreach (var (target, widget) in _items.Values)
            {
                widget.transform.position = offset + Camera.main.WorldToScreenPoint(target.position) ;
            }
        }

        private void OnDamage(DamageMessage obj)
        {
            if (_items.TryGetValue(obj.Id, out var item)) item.Item2.Current = obj.CurrentHull;
        }
        private void OnHull(DisplayHullMessage obj)
        {
            if (obj.Active)
            {
                var item = _pool.Pick();
                item.transform.SetParent(transform);
                item.gameObject.SetActive(true);
                item.Current = obj.CurrentHull;
                item.Initial = obj.InitialHull;
                _items.Add(obj.Id, (obj.Target,item));
            }
            else
            {
                var item = _items[obj.Id];
                item.Item2.gameObject.SetActive(false);
                _items.Remove(obj.Id);
                _pool.Return(item.Item2);
            }
        }
    }
}
