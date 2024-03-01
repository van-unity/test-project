using UnityEngine;

public interface IObjectPool<T> where T : Component, ISpawnable {
    T Get();
    void Return(T objectToReturn);
}