using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourObjectPoolBase<T> : MonoBehaviour, IObjectPool<T> where T : Component, ISpawnable {
    [SerializeField] private int _initialSize = 10;
    [SerializeField] private GameObject _assetReference;

    private Transform _container;

    private readonly Stack<T> _pool = new();
        
    private void Awake() {
        _container = new GameObject($"{typeof(T).Name}_Pool").transform;
        Initialize();
    }

    private void Initialize() {
        for (int i = 0; i < _initialSize; i++) {
            _pool.Push(Create());
        }
    }

    public T Get() {
        if (_pool.TryPop(out var item)) {
            item.OnSpawned();
            return item;
        }


        var newItem = Create();
        newItem.OnSpawned();
        return newItem;
    }

    private T Create() {
        var instantiatedItem = Instantiate(_assetReference, _container, false);
        var component = instantiatedItem.GetComponent<T>();
        component.Initialize();
        return component;
    }

    public void Return(T objectToReturn) {
        _pool.Push(objectToReturn);
        objectToReturn.transform.SetParent(_container, false);
        objectToReturn.OnDeSpawned();
    }
}