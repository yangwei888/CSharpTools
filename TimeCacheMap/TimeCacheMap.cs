using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimeCacheMap
{
    public class TimeCacheMap<K, V>:IDisposable
    {
        public delegate void ExpiredCallBack(Dictionary<K, V> kv);
        private LinkedList<Dictionary<K, V>> buckets;
        private readonly object Obj = new object();
        private static readonly int NumBuckets = 3;
        private Thread cleaner;
        public TimeCacheMap(int expirationSecs, int numBuckets, ExpiredCallBack ex)
        {
            if (numBuckets < 2)
                throw new ArgumentException("numBuckets must be >=2");
            this.buckets = new LinkedList<Dictionary<K, V>>();
            for (int i = 0; i < numBuckets; i++)
                buckets.AddFirst(new Dictionary<K, V>());
            var expirationMillis = expirationSecs * 1000;
            var sleepTime = expirationMillis / (numBuckets - 1);
            cleaner = new Thread(() =>
            {
                while (true)
                {
                    Dictionary<K, V> dead = null;
                    Thread.Sleep(sleepTime);
                    lock (Obj)
                    {
                        dead = buckets.Last();
                        buckets.RemoveLast();
                        buckets.AddFirst(new Dictionary<K, V>());
                    }
                    if (ex != null)
                        ex(dead);
                }
            });
            cleaner.IsBackground = true;
            cleaner.Start();
        }
        public TimeCacheMap(int expirationSecs, int numBuckets)
            : this(expirationSecs, numBuckets, null)
        {

        }
        public TimeCacheMap(int expirationSecs)
            : this(expirationSecs, NumBuckets, null)
        {

        }
        public void Put(K key, V value)
        {
            lock (Obj)
            {
                foreach (var item in buckets)
                {
                    item.Remove(key);
                }
                buckets.First().Add(key, value);
            }
        }
        public V Get(K key)
        {
            lock (Obj)
            {
                foreach (var item in buckets)
                {
                    if (item.ContainsKey(key))
                        return item[key];
                }
                return default(V);
            }
        }
        public void Remove(K key)
        {
            lock (Obj)
            {
                foreach (var item in buckets)
                {
                    if (item.ContainsKey(key))
                        item.Remove(key);
                }
            }
        }
        public bool ContainsKey(K key)
        {
            lock (Obj)
            {
                foreach (var item in buckets)
                {
                    if (item.ContainsKey(key))
                        return true;
                }
                return false;
            }
        }
        public int Size()
        {
            lock (Obj)
            {
                int size = 0;
                foreach (var item in buckets)
                {
                    size += item.Count;
                }
                return size;
            }
        }
        public void Dispose() {
            cleaner.Abort();
        }
    }
}