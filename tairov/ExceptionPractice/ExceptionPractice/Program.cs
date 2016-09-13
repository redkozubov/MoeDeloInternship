using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ExceptionPractice.MyExceptions;

namespace ExceptionPractice
{
    public class Program
    {
        static void Main()
        {
            var fileName = "persons.xml";
            var runtimeLog = new LoggerDummy();

            // Раскомментировать, если нужно сгенерить новый файл
            //GenerateXmlFile(fileName);

            try
            {
                var tasks = GetTasks(fileName);
                foreach (var task in tasks)
                {
                    try
                    {
                        ImportTask(task);
                    }
                    catch (NoDataException e)
                    {
                        Console.WriteLine("Ошибка: В записе отсутствуют данные");
                        Console.WriteLine(e.Message);
                        runtimeLog.Log(e.ToString());
                    }
                    catch (FactDateInFutureException e)
                    {
                        Console.WriteLine("Ошибка: Дата фактического выполнения находится в будущем");
                        Console.WriteLine(e.Message);
                        runtimeLog.Log(e.ToString());
                    }
                }
            }
            catch (IncorrectFileFormatException e)
            {
                Console.WriteLine("Ошибка: XML файл имеет некорректный формат");
                Console.WriteLine(e.Message);
                runtimeLog.Log(e.ToString());
            }
            catch (UserForgottenFileException e)
            {
                Console.WriteLine("Ошибка: Файла persons.xml не существует");
                Console.WriteLine(e.Message);
                runtimeLog.Log(e.ToString());

            }
            Console.ReadLine();
        }

        internal static void GenerateXmlFile(string fileName)
        {
            var tasks = new List<Task>
            {
                new Task
                {
                    Title = "Встретиться",
                    PlanDate = new DateTime(2016, 01, 01),
                    FactDate = new DateTime(2016, 02, 03)
                },
                new Task
                {
                    Title = "Позвонить",
                    PlanDate = new DateTime(2016, 02, 01),
                    FactDate = new DateTime(2016, 02, 03)
                },
                new Task
                {
                    Title = "Продать XXX",
                    PlanDate = new DateTime(2016, 01, 01),
                    FactDate = new DateTime(2016, 08, 03)
                }
            };

            // передаем в конструктор тип класса
            XmlSerializer formatter = new XmlSerializer(typeof(List<Task>));

            // получаем поток, куда будем записывать сериализованный объект
            using (FileStream fs = GetFileStream(fileName, true)) //new FileStream(fileName,FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, tasks);
                Console.WriteLine("Объект сериализован");
            }
        }

        internal static void ImportTask(Task task)
        {
            ValidateTask(task);
            // Заглушка
            Console.WriteLine("Успешный импорт: {0},{1},{2}", task.Title, task.PlanDate, task.FactDate);
        }
        private static void ValidateTask(Task task)
        {
            if (task.Title == null || task.PlanDate == null || task.PlanDate.Year == 1)
                throw new NoDataException();
            // if Фактическая дата выполнения задачи не может быть позже сегодняшнего дня
            if (task.FactDate > DateTime.Today)
                throw new FactDateInFutureException();

        }
        internal static List<Task> GetTasks(string fileName)
        {
            // передаем в конструктор тип класса
            XmlSerializer formatter = new XmlSerializer(typeof(List<Task>));
            List<Task> tasks;
            // десериализация
            FileStream fs = GetFileStream(fileName);
            try
            {
                tasks = (List<Task>) formatter.Deserialize(fs);
            }
            catch (InvalidOperationException e)
            {
                throw new IncorrectFileFormatException("Некорректный формат XML файла", e);
            }
            finally
            {
                fs.Close();
            }
            return tasks;
        }

        /// <summary>
        /// Получаем файловый поток
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isNewFile"></param>
        /// <returns></returns>
        private static FileStream GetFileStream(string fileName, bool isNewFile = false)
        {
            if (CheckIfFileExists(fileName) || isNewFile)
            {
                return new FileStream(fileName, FileMode.OpenOrCreate);
            }
            throw new UserForgottenFileException();
        }

        /// <summary>
        /// Проверяем доступность файла
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>

        private static bool CheckIfFileExists(string fileName)
        {
            return File.Exists(fileName);
        }

    }

    [Serializable]
    public class Task
    {
        public string Title { get; set; }
        public DateTime PlanDate { get; set; }
        public DateTime FactDate { get; set; }
    }
    public interface ILogger
    {
        void Log(string data);
    }
    public class LoggerDummy : ILogger
    {
        public void Log(string data)
        {
            Console.WriteLine("LoggerDummy:" + new string(data.Take(10).ToArray()));
        }
    }
}
