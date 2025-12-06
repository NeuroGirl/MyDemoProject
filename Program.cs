using University.Application;
using University.Infrastructure;
using ConsoleСontroller;
public class Program
{
public static void Main(string[] args)
    {
        CourseService courseService = InitializeSystem();

        if (courseService == null)
        {
            Console.WriteLine("Критическая ошибка инициализации. Программа завершает работу.");
            return;
        }

        ConsoleCommands.SetCourseService(courseService);
        RunConsoleInterface();}

   private static CourseService InitializeSystem()
    {
        Console.WriteLine("--- Инициализация системы управления курсами и преподавателями ---");

        var courseRepository = new InMemoryCourseRepository();
        var teacherRepository = new InMemoryTeacherRepository();
        var studentRepository = new InMemoryStudentRepository();

        var courseService = new CourseService(courseRepository, teacherRepository, studentRepository);

        Console.WriteLine("--- Система инициализирована ---");
        return courseService;
    }

    private static void RunConsoleInterface()
        {
            string command;
            do
            {
                ShowMenu();
                command = Console.ReadLine()?.Trim().ToLower() ?? "";

                switch (command)
                {
                    case "1": ConsoleCommands.AddCourse(); break;
                    case "2": ConsoleCommands.RemoveCourse(); break;
                    case "3": ConsoleCommands.AssignTeacherToCourse(); break;
                    case "4": ConsoleCommands.UnassignTeacherFromCourse(); break;
                    case "5": ConsoleCommands.AddStudentToCourse(); break;
                    case "6": ConsoleCommands.RemoveStudentFromCourse(); break;
                    case "7": ConsoleCommands.GetAllCourses(); break;
                    case "8": ConsoleCommands.GetCoursesByTeacher(); break;
                    case "9": ConsoleCommands.GetStudentsByCourse(); break;
                    case "10": ConsoleCommands.AddTeacher(); break;
                    case "11": ConsoleCommands.AddStudent(); break;
                    case "exit":
                    case "quit":
                    case "q":
                        Console.WriteLine("Выход из системы...");
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда. Пожалуйста, выберите из меню.");
                        break;
                }

                if (command != "exit" && command != "quit" && command != "q")
                {
                    Console.WriteLine("\nНажмите Enter для продолжения...");
                    Console.ReadLine();
                    Console.Clear();
                }

            } while (command != "exit" && command != "quit" && command != "q");
        }


    private static void ShowMenu()
    {
        Console.WriteLine("\n===== Меню управления системой =====");
        Console.WriteLine("--- Курсы ---");
        Console.WriteLine("1. Добавить новый курс (онлайн/офлайн)");
        Console.WriteLine("2. Удалить курс");
        Console.WriteLine("3. Назначить преподавателя на курс");
        Console.WriteLine("4. Снять преподавателя с курса");
        Console.WriteLine("5. Добавить студента на курс");
        Console.WriteLine("6. Удалить студента с курса");
        Console.WriteLine("7. Показать все курсы");
        Console.WriteLine("--- Преподаватели и Студенты ---");
        Console.WriteLine("8. Показать курсы преподавателя");
        Console.WriteLine("9. Показать студентов курса");
        Console.WriteLine("10. Добавить преподавателя");
        Console.WriteLine("11. Добавить студента");
        Console.WriteLine("---------------------------------");
        Console.WriteLine("exit/quit/q - Выйти из программы");
        Console.Write("Введите номер команды: ");
    }

}