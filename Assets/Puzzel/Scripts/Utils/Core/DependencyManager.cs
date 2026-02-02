using System.Collections.Generic;
using UnityEngine;
using System;

public class DependencyManager : Singleton<DependencyManager>
{
    [SerializeField, Space] private List<MonoBehaviour> objects = new List<MonoBehaviour>();

    private Dictionary<Type, MonoBehaviour> objectDict;

    protected override void Awake()
    {
        base.Awake();
        InitializeDependency();
    }

    private void InitializeDependency()
    {
        objectDict = new Dictionary<Type, MonoBehaviour>();
        objects.ForEach(x =>
        {
            objectDict.Add(x.GetType(), x);
        });
    }

    public T Resolve<T>() where T : MonoBehaviour
    {
        return objectDict[typeof(T)] as T;
    }

    public T TryResolve<T>(bool searchInScene = true) where T : MonoBehaviour
    {
        if (objectDict.ContainsKey(typeof(T)))
        {
            return objectDict[typeof(T)] as T;
        }

        if (searchInScene)
        {
            T dependency = FindAnyObjectByType<T>();
            if (dependency != null)
            {
                AddDependency(dependency);
                return dependency;
            }
        }

        return null;
    }

    public void AddDependency<T>(T obj) where T : MonoBehaviour
    {
        Type type = typeof(T);
        if (!objectDict.ContainsKey(type))
        {
            objectDict.Add(type, obj);

            objects.Add(obj);
        }
    }

    public void RemoveDependency<T>() where T : MonoBehaviour
    {
        Type type = typeof(T);
        if (objectDict.ContainsKey(type))
        {
            MonoBehaviour obj = objectDict[type];
            objects.Remove(obj);

            objectDict.Remove(type);
        }
    }
}