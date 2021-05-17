using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class WorldHud : MonoBehaviour
{
    private Stack<GameObject> _pool = new Stack<GameObject>();
    private Dictionary<int, GameObject> _items = new Dictionary<int, GameObject>();
    [SerializeField]
    private int defaultSize = 10;
    [SerializeField]
    private GameObject prefab;
    private void Awake()
    {
        this.Subscribe<DisplayHullMessage>(OnHull);
        this.Subscribe<DamageMessage>(OnDamage);
        EnlargePool(defaultSize);
    }

    void EnlargePool(int size)
    {
        for (var i = 0; i < size; i++)
        {
            var item = Instantiate(prefab, transform);
            item.SetActive(false);
            _pool.Push(item);
        }
        
    }
    
    private void OnDamage(DamageMessage obj)
    {
    }
    private void OnHull(DisplayHullMessage obj)
    {
        if (obj.Active)
        {
            if (_pool.Count == 0 )  EnlargePool(5);
            var item = _pool.Pop();
            item.SetActive(true);
            _items.Add(obj.Id,item);
        }
        else
        {
            var item = _items[obj.Id];
            item.SetActive(false);
            _items.Remove(obj.Id);
            _pool.Push(item);
        }
        
    }

    private void Update()
    {
    }
}
