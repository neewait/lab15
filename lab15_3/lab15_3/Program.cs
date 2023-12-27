using System;
using System.Threading;

namespace lab15_3
{
    public class SingleRandomizer
    {
        private static SingleRandomizer instance;
        private static readonly object lockObject = new object();

        private Random random; //генератор случайных чисел

        private SingleRandomizer()
        {
            random = new Random(); //инициализация
        }

        public static SingleRandomizer Instance //метод для получения единственного экземпляра
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new SingleRandomizer(); //создание единственного экземпляра
                        }
                    }
                }
                return instance;
            }
        }

        public int Next() //метод для следующего случайного числа
        {
            lock (lockObject)
            {
                return random.Next(); //генерация
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter the number");
            int numThreads = int.Parse(Console.ReadLine()); //чтение потоков из консоли

            Thread[] threads = new Thread[numThreads]; //массив потоков
            for (int i = 0; i < numThreads; i++)
            {
                threads[i] = new Thread(() =>
                {
                    SingleRandomizer randomizer = SingleRandomizer.Instance; //получение единственного экземпляра генератора
                    int randomNumber = randomizer.Next(); //генерация случайного числа
                    Console.WriteLine($"Thread ID: {Thread.CurrentThread.ManagedThreadId}, Random Number: {randomNumber}");
                });
                threads[i].Start(); //запуск потока
            }
            Console.ReadLine();
        }
    }
}
