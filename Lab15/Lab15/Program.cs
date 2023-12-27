using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;

namespace Lab15
{
    public class DirectoryChangedEventArgs : EventArgs // наследуемся от класса EventArgs
    {
        public string FileName { get; set; } // храним имя файла, вызвавшего событие
    }

    public delegate void DirectoryChangedEventHandler(object sender, DirectoryChangedEventArgs args); // делегат для обработки событий изменения директории

    public class DirectorySystemWatcher // основной класс для отслеживания изменений в директории
    {
        private string directoryPath; // путь к директории
        public Timer timer; //управление временем внутри класса
        private List<string> currentFiles; //список для хранения имён файлов, отслеживаемых в директории

        public event DirectoryChangedEventHandler FileChanged; //уведомления об изменениях

        public DirectorySystemWatcher(string directoryPath)
        {
            this.directoryPath = directoryPath;
            currentFiles = Directory.GetFiles(directoryPath).ToList(); //получаем файлы в директории

            timer = new Timer(CheckDirectory, null, TimeSpan.Zero, TimeSpan.FromSeconds(1)); // таймер вызывает метод CheckDirectory каждую секунду и повторяет вызов каждую секунду
                                                                                             // null вторым параметром указывает на вызов метода CheckDirectory.
        }

        public void CheckDirectory(object state) //метод проверки директории на изменения
        {
            var files = Directory.GetFiles(directoryPath); //получение списка файлов

            foreach (var file in files) //перебор всех файлов
            {
                if (currentFiles.Contains(file)) //проверка на содержание
                {
                    continue;
                }

                currentFiles.Add(file); // добавление нового файла в списокч

                OnFileChanged(file); //вызов события, чтобы оповестить об изменении
            }

            for (int i = currentFiles.Count - 1; i >= 0; i--)
            {
                var file = currentFiles[i]; //получение списка файлов
                if (!File.Exists(file)) //проверка на существование
                {
                    currentFiles.RemoveAt(i); //удаление из списка, если его нет в директории
                    OnFileChanged(file); //вызов события, чтобы оповестить об изменении
                }
            }
        }

        protected virtual void OnFileChanged(string fileName) //уведомление об изменении
        {
            FileChanged?.Invoke(this, new DirectoryChangedEventArgs { FileName = fileName }); //если есть подписчики на это событие, то вызывапется изменение файла
        }
    }

    public class DirectoryChangeHandler //обработка событий (изменение файлов в дир-ии)
    {
        public void OnFileChanged(object sender, DirectoryChangedEventArgs args)
        {
            Console.WriteLine($"File {args.FileName} has been changed");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var directorySystemWatcher = new DirectorySystemWatcher("C:\\Users\\user\\Desktop\\task1");

            var directoryChangeHandler = new DirectoryChangeHandler();

            directorySystemWatcher.FileChanged += directoryChangeHandler.OnFileChanged; //подписка на событие

            while (true)
            {
                directorySystemWatcher.CheckDirectory(directorySystemWatcher.timer); //бесконечный мониторинг
            }
        }
    }
}
