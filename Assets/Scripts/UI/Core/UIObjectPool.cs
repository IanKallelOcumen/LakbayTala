using UnityEngine;
using System.Collections.Generic;

namespace LakbayTala.UI.Core
{
    /// <summary>
    /// Generic object pool for UI elements (e.g. leaderboard rows, lore card instances)
    /// to reduce allocations and GC. Call Get() to take an instance, Return() to recycle.
    /// </summary>
    public class UIObjectPool<T> where T : Component
    {
        private readonly T prefab;
        private readonly Transform parent;
        private readonly Queue<T> available = new Queue<T>();
        private readonly List<T> inUse = new List<T>();

        public UIObjectPool(T prefab, Transform parent, int prewarmCount = 0)
        {
            this.prefab = prefab;
            this.parent = parent;
            for (int i = 0; i < prewarmCount; i++)
                Return(CreateNew());
        }

        private T CreateNew()
        {
            T instance = parent != null
                ? Object.Instantiate(prefab, parent)
                : Object.Instantiate(prefab);
            instance.gameObject.SetActive(false);
            return instance;
        }

        public T Get()
        {
            T item = available.Count > 0 ? available.Dequeue() : CreateNew();
            inUse.Add(item);
            item.gameObject.SetActive(true);
            return item;
        }

        public void Return(T item)
        {
            if (item == null) return;
            if (!inUse.Remove(item)) return;
            item.gameObject.SetActive(false);
            if (parent != null && item.transform.parent != parent)
                item.transform.SetParent(parent, false);
            available.Enqueue(item);
        }

        public void ReturnAll()
        {
            for (int i = inUse.Count - 1; i >= 0; i--)
                Return(inUse[i]);
        }

        public int AvailableCount => available.Count;
        public int InUseCount => inUse.Count;
    }
}
