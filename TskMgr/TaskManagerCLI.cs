using System;
using System.Collections.Generic;
using System.Text;

namespace TskMgr
{
    public class TaskManagerCLI
    {
        TaskManager taskManager;
        public void Run()
        {
            taskManager = new TaskManager();


            while (true) 
            {
                Console.Write("TskCmd: ");
                var input = Console.ReadLine();

                var inputList = input.Split(' ');


                switch (inputList[0]) 
                {
                    case "add":
                        break;
                    case "list":
                        break;
                    case "delete":
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Не известаня комманда");
                        break;
                }


            }
        }

        void AddTask()
        {
            Console.Write("Введите название задачи: ");
            string taskName = Console.ReadLine();

            if (taskName == null || taskName == string.Empty) 
            {
                Console.WriteLine("Ошибка создания задачи: Необходимо имя задачи");
                return;
            }

            Console.Write("Введите описание задачи: ");
            string taskDescription = Console.ReadLine();

            TaskPriority priority = SelectPriority();

            TaskStatus status = TaskStatus.NotStart;

            DateTime createDate = DateTime.Now;



        }

        DateTime? GetDeadline() 
        {
            Console.Write("Задать дедлайн? (y/n): ");
            var ch = Console.ReadLine();
            if (ch != "y") return null;

            

            Console.Write("Введите дату в формате дд.мм.гггг: ");

            var dateInput = Console.ReadLine();
            var dateSplit = dateInput.Split('.');
           


            DateTime deadline = new DateTime();


            return null;
        }

        TaskPriority SelectPriority()
        {
            Console.WriteLine("Выбирете приоретет задачи: ");
            Console.WriteLine("1) Низкий");
            Console.WriteLine("2) Средний");
            Console.WriteLine("3) Высокий");

            Console.Write("> ");
            var ch = Console.ReadLine();

            if (int.TryParse(ch, out int result))
            {
                if (result >= 1 && result <= 3) 
                {
                    return (TaskPriority)result;
                }
            }

            Console.WriteLine("Ошибка выбора приоритета задачи. Автоматичсески выбрано: Низкий");
            return TaskPriority.Low;
        }
    }
}
