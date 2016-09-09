using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ExceptExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = "persons.xml";

            // Раскомментировать, если нужно сгенерить новый файл
            GenerateXMLFile(fileName);
            try
            {
                var tasks = GetTasks(fileName);

                foreach (var task in tasks)
                {
                    try
                    {
                        ImportTask(task);
                    }
                    catch (NoDataException exc)
                    {
                        Console.WriteLine(exc);
                    }
                    catch (FactDateInFutureException exc)
                    {
                        Console.WriteLine(exc);
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            Console.ReadLine();
        }
        

        internal static void GenerateXMLFile(string fileName)
        {
            var tasks = new List<Task>
            {
                new Task()
                {
                Title = "Встретиться",
                PlanDate = new DateTime(2016, 01,01),
                FactDate = new DateTime(2016, 02, 03)
                },
                new Task()
                {
                Title = "",
                PlanDate = new DateTime(2016, 02,01),
                FactDate = new DateTime(2016, 02, 03)
                },
                new Task()
                {
                Title = "Продать XXX",
                PlanDate = new DateTime(2016, 01,01),
                FactDate = new DateTime(2016, 08, 03)
                },
                new Task()
                {
                Title = "Планирование",
                PlanDate = new DateTime(2016, 01,01),
                FactDate = new DateTime(2016, 10, 03)
                },
            };
            
            // передаем в конструктор тип класса
            XmlSerializer formatter = new XmlSerializer(typeof(List<Task>));
            

            // получаем поток, куда будем записывать сериализованный объект
            using (FileStream fs = GetFileStream(fileName, true)) //new FileStream(fileName, FileMode.OpenOrCreate))
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


        /// <summary>
        /// Проверяем, правильно ли заполнено задание
        /// </summary>
        /// <param name="task"></param>
        private static void ValidateTask(Task task)
        {
            //  if заглоовок и плановая дата должны быть заполнены
            if (String.IsNullOrEmpty(task.Title) || task.PlanDate == null)
                throw new NoDataException("В задании не заполнены необхдимые поля.");

            //  if Фактическая дата выполнения задачи не может быть позже сегодняшнего дня
            if (task.FactDate.Date > DateTime.Now.Date)
                throw new FactDateInFutureException("Фактическая дата не может находиться в будущем");
        }



        internal static List<Task> GetTasks(string fileName)
        {
            // передаем в конструктор тип класса
            XmlSerializer formatter = new XmlSerializer(typeof(List<Task>));
            

            List<Task> tasks;
            // десериализация
            FileStream fs = GetFileStream(fileName);
            if (fs == null)
                throw new UserForgottenFileException("Не найден файл с данными о задачах.");

            try
            {
                tasks = (List<Task>)formatter.Deserialize(fs);
            }
            catch
            {
                throw new IncorrectFileFormatException("Некорректные данные в исходном файле XML.");
            }

            fs.Close();

            return tasks;
        }



        /// <summary>
        /// Получаем файловый поток
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static FileStream GetFileStream(string fileName, bool isNewFile = false)
        {
            if (CheckIfFileExists(fileName) || isNewFile)
            {
                return new FileStream(fileName, FileMode.OpenOrCreate);
            }
            return null;
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

    /// <summary>
    /// Генерируется при неудачной попытке открыть файл с XML
    /// </summary>
    class UserForgottenFileException : Exception
    {
        public UserForgottenFileException (string msg) : base(msg) { }
        public override string ToString()
        {
            return Message;
        }
    }

    /// <summary>
    /// Генерируется при неверном формате исходных данных в XML файле
    /// </summary>
    class IncorrectFileFormatException : Exception
    {
        public IncorrectFileFormatException (string msg) : base (msg)
        {
        }

        public override string ToString()
        {
            return Message;
        }
    }

    /// <summary>
    /// Генерируется отсутствии данных в Задании
    /// </summary>
    class NoDataException : Exception
    {
        public NoDataException (string msg) : base(msg) { }

        public override string ToString()
        {
            return Message;
        }
    }

    class FactDateInFutureException:Exception
    {
        public FactDateInFutureException(string msg) : base(msg) { }

        public override string ToString()
        {
            return Message;
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
