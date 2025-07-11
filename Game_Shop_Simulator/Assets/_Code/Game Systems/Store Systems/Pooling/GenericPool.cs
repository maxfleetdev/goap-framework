using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine;

public class GenericPool<T> where T : UnityEngine.Object
{
    private readonly Dictionary<int, Queue<T>> pool = new();
    private readonly Func<int, Task<T>> factory;

    public GenericPool(Func<int, Task<T>> factoryMethod)
    {
        factory = factoryMethod;
    }

    public async Task<T> GetAsync(int id)
    {
        if (!pool.TryGetValue(id, out var queue))
        {
            queue = new Queue<T>();
            pool[id] = queue;
        }

        if (queue.Count > 0)
        {
            var obj = queue.Dequeue();
            if (obj is GameObject go) go.SetActive(true);
            return obj;
        }

        var newObj = await factory(id);
        return newObj;
    }

    public void Return(int id, T obj)
    {
        if (obj is GameObject go) go.SetActive(false);
        pool[id].Enqueue(obj);
    }

    public void Clear() => pool.Clear();
}