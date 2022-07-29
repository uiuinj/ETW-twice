using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace WpfLibrary1
{
    public class Class1
    {
        public static Dictionary<int,int> dictionary=new Dictionary<int, int>();
        public static System.Collections.Concurrent.ConcurrentDictionary<int, int> conDict = new ConcurrentDictionary<int, int>();
        public static void Add(object obj){//多线程添加键值出现数据缺失异常
            Param pra = (Param)obj;
            int i = pra.praData;
            Console.WriteLine("Thread execute at {0}", pra.praData);
            dictionary.Add((int)i, (int)i);
            pra.mrEvent.Set();//解锁
        }
        public static void remove(object obj){//好像没有问题
            Param par = (Param)obj;
            int i = par.praData;
            Console.WriteLine("Thread execute at {0}", par.praData);
            dictionary.Remove(i);
            par.mrEvent.Set();//解锁
        }
        public static void insert(object obj){//丢失数据
            Param par = (Param)obj;
            int i = par.praData;
            Console.WriteLine("Thread execute at {0}", par.praData);
            dictionary.TryAdd(i, i);
            par.mrEvent.Set();//解锁
        }
        public static void getInfo(object obj){//cpu占用率变高了。。。。。（）
            foreach (KeyValuePair<int, int> kv in conDict)
            {
                Console.WriteLine("key：{0}, value：{1}", kv.Key, kv.Value);//打印结果
            }
            Param par = (Param)obj;
            Console.WriteLine("Thread execute at {0}", par.praData);
            par.mrEvent.Set();//解锁


        } 
        static void Main(string[] args){
            for (int i = 0; i < 50; i++)
            {
                conDict.TryAdd(i, i);
            }
            dictionary.Add(0, 0);
            /* 我的第一个 C# 程序*/
            List<ManualResetEvent> manualEvents = new List<ManualResetEvent>();//锁列表
            for (int i = 0; i < 50; i++){
                ManualResetEvent mre = new ManualResetEvent(false);
                manualEvents.Add(mre);//加锁
                Param pra = new Param();
                pra.mrEvent = mre;
                pra.praData = i;
                ThreadPool.QueueUserWorkItem(getInfo, pra);//送入线程池执行方法
            }
            WaitHandle.WaitAll(manualEvents.ToArray());//等待所有线程执行完毕，释放锁
            Console.WriteLine("finish");
            Console.WriteLine(dictionary.Count);
            //foreach (KeyValuePair<int, int> kv in dictionary){
            //    Console.WriteLine("key：{0}, value：{1}", kv.Key, kv.Value);//打印结果
            //}

        }
    }
    public class Param
    {
        public ManualResetEvent mrEvent;
        public int praData;
    }
}
