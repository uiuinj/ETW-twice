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
        public static void Add(object obj){//���߳���Ӽ�ֵ��������ȱʧ�쳣
            Param pra = (Param)obj;
            int i = pra.praData;
            Console.WriteLine("Thread execute at {0}", pra.praData);
            dictionary.Add((int)i, (int)i);
            pra.mrEvent.Set();//����
        }
        public static void remove(object obj){//����û������
            Param par = (Param)obj;
            int i = par.praData;
            Console.WriteLine("Thread execute at {0}", par.praData);
            dictionary.Remove(i);
            par.mrEvent.Set();//����
        }
        public static void insert(object obj){//��ʧ����
            Param par = (Param)obj;
            int i = par.praData;
            Console.WriteLine("Thread execute at {0}", par.praData);
            dictionary.TryAdd(i, i);
            par.mrEvent.Set();//����
        }
        public static void getInfo(object obj){//cpuռ���ʱ���ˡ�������������
            foreach (KeyValuePair<int, int> kv in conDict)
            {
                Console.WriteLine("key��{0}, value��{1}", kv.Key, kv.Value);//��ӡ���
            }
            Param par = (Param)obj;
            Console.WriteLine("Thread execute at {0}", par.praData);
            par.mrEvent.Set();//����


        } 
        static void Main(string[] args){
            for (int i = 0; i < 50; i++)
            {
                conDict.TryAdd(i, i);
            }
            dictionary.Add(0, 0);
            /* �ҵĵ�һ�� C# ����*/
            List<ManualResetEvent> manualEvents = new List<ManualResetEvent>();//���б�
            for (int i = 0; i < 50; i++){
                ManualResetEvent mre = new ManualResetEvent(false);
                manualEvents.Add(mre);//����
                Param pra = new Param();
                pra.mrEvent = mre;
                pra.praData = i;
                ThreadPool.QueueUserWorkItem(getInfo, pra);//�����̳߳�ִ�з���
            }
            WaitHandle.WaitAll(manualEvents.ToArray());//�ȴ������߳�ִ����ϣ��ͷ���
            Console.WriteLine("finish");
            Console.WriteLine(dictionary.Count);
            //foreach (KeyValuePair<int, int> kv in dictionary){
            //    Console.WriteLine("key��{0}, value��{1}", kv.Key, kv.Value);//��ӡ���
            //}

        }
    }
    public class Param
    {
        public ManualResetEvent mrEvent;
        public int praData;
    }
}
