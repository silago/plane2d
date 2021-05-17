using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Events;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class HullWidget : MonoBehaviour
{
    [SerializeField]
    private Text current;
    [SerializeField]
    private Text initial;
    public int Current { set => current.text = value.ToString(); }
    public int Initial { set => initial.text = value.ToString(); }
}

public class Pool<T> where T : Component 
{
    private Stack<T> _pool = new Stack<T>();
    private T _prefab;
    public Pool(T prefab, int initial = 0)
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

    private void EnlargePool(int size)
    {
        for (var i = 0; i < size; i++)
        {
            var item = UnityEngine.Object.Instantiate(_prefab);
            item.gameObject.SetActive(false);
            _pool.Push(item);
        }
    }
}
public class WorldHud : MonoBehaviour
{

    private Pool<HullWidget> _pool;
    private Dictionary<int, HullWidget> _items = new Dictionary<int, HullWidget>();
    [SerializeField]
    private int defaultSize = 10;
    [SerializeField]
    private HullWidget prefab;
    private void Awake()
    {
        this.Subscribe<DisplayHullMessage>(OnHull);
        this.Subscribe<DamageMessage>(OnDamage);
        _pool = new Pool<HullWidget>(prefab);
    }

    
    private void OnDamage(DamageMessage obj)
    {
        if (_items.TryGetValue(obj.Id, out var item))
        {
            item.Current = obj.CurrentHull;
        }
    }
    private void OnHull(DisplayHullMessage obj)
    {
        if (obj.Active)
        {
            var item = _pool.Pick();
            item.gameObject.SetActive(true);
            item.Current = obj.CurrentHull;
            item.Initial = obj.InitialHull;
            _items.Add(obj.Id, item);
        }
        else
        {
            var item = _items[obj.Id];
            item.gameObject.SetActive(false);
            _items.Remove(obj.Id);
            _pool.Return(item);
        }
    }
}
