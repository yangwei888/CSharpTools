using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimeCacheMap
{
    class Program
    {
        static void Main(string[] args)
        {
            TimeCacheMap<int, int> tc = new TimeCacheMap<int, int>(10);

            tc.Put(1, 1);
            tc.Put(2, 2);
            Console.WriteLine(tc.Get(1));
            Console.WriteLine(tc.Get(2));
            Thread.Sleep(16000);
            Console.WriteLine(tc.Get(1));
            Console.WriteLine(tc.Get(2));
            tc.Put(1, 1);
            tc.Put(2, 2);
            Console.WriteLine("CacheMap Count:" + tc.Size());
            Console.WriteLine(tc.ContainsKey(1));
            tc.Remove(1);
            Console.WriteLine(tc.Size());
            Console.WriteLine(tc.ContainsKey(1));
            Console.ReadLine();
        }
    }
}
