using University.Domain;
using University.Application;

namespace ConsoleСontroller
{
    public static class ConsoleCommands
    {
        private static CourseService _courseService;

        public static void SetCourseService(CourseService courseService)
        {
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
        }


    internal static void AddCourse()
    {
        Console.WriteLine("\n--- Добавление нового курса ---");
        Console.Write("Введите название курса: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Тип курса (online/offline): ");
        string type = Console.ReadLine()?.Trim().ToLower() ?? "";

        Course? newCourse = null;
        int nextId = _courseService.GetAllCourses().DefaultIfEmpty(new OfflineCourse(0, "тест", "тест")).Max(c => c.Id) + 1;

        try
        {
            if (type == "online")
            {
                Console.Write("Введите URL платформы: ");
                string platformUrl = Console.ReadLine() ?? "";
                newCourse = new OnlineCourse(nextId, name, platformUrl);
            }
            else if (type == "offline")
            {
                Console.Write("Введите местоположение (аудитория): ");
                string location = Console.ReadLine() ?? "";
                newCourse = new OfflineCourse(nextId, name, location);
            }
            else
            {
                Console.WriteLine("Неверный тип курса. Пожалуйста, введите 'online' или 'offline'.");
                return;
            }

            _courseService.AddCourse(newCourse);
            Console.WriteLine($"Курс '{name}' ({type}) успешно добавлен с ID: {newCourse.Id}");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"Ошибка: Не все поля были заполнены. {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    internal static void RemoveCourse()
    {
        Console.WriteLine("\n--- Удаление курса ---");
        Console.Write("Введите ID курса для удаления: ");
        if (int.TryParse(Console.ReadLine(), out int courseId))
        {
            var course = _courseService.GetCourseById(courseId);
            if (course != null)
            {
                _courseService.RemoveCourse(courseId);
                Console.WriteLine($"Курс '{course.Name}' (ID: {courseId}) успешно удален.");
            }
            else
            {
                Console.WriteLine($"Курс с ID {courseId} не найден.");
            }
        }
        else
        {
            Console.WriteLine("Неверный формат ID. Пожалуйста, введите число.");
        }
    }

    internal static void AssignTeacherToCourse()
    {
        Console.WriteLine("\n--- Назначение преподавателя на курс ---");
        Console.Write("Введите ID курса: ");
        if (int.TryParse(Console.ReadLine(), out int courseId))
        {
            Console.Write("Введите ID преподавателя: ");
            if (int.TryParse(Console.ReadLine(), out int teacherId))
            {
                try
                {
                    _courseService.AssignTeacherToCourse(courseId, teacherId);
                    var course = _courseService.GetCourseById(courseId);
                    var teacher = _courseService.GetTeacherById(teacherId); 
                    Console.WriteLine($"Преподаватель '{teacher?.Name}' успешно назначен на курс '{course?.Name}'.");
                }
                catch (KeyNotFoundException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат ID преподавателя.");
            }
        }
        else
        {
            Console.WriteLine("Неверный формат ID курса.");
        }
    }

    internal static void UnassignTeacherFromCourse()
    {
        Console.WriteLine("\n--- Снятие преподавателя с курса ---");
        Console.Write("Введите ID курса: ");
        if (int.TryParse(Console.ReadLine(), out int courseId))
        {
            try
            {
                var course = _courseService.GetCourseById(courseId);
                if (course != null && course.AssignedTeacher != null)
                {
                    var teacherName = course.AssignedTeacher.Name;
                    _courseService.UnassignTeacherFromCourse(courseId);
                    Console.WriteLine($"Преподаватель '{teacherName}' снят с курса '{course.Name}'.");
                }
                else if (course == null)
                {
                    Console.WriteLine($"Курс с ID {courseId} не найден.");
                }
                else
                {
                    Console.WriteLine($"Курс '{course.Name}' не имеет назначенного преподавателя.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Неверный формат ID курса.");
        }
    }

    internal static void AddStudentToCourse()
    {
        Console.WriteLine("\n--- Запись студента на курс ---");
        Console.Write("Введите ID курса: ");
        if (int.TryParse(Console.ReadLine(), out int courseId))
        {
            Console.Write("Введите ID студента: ");
            if (int.TryParse(Console.ReadLine(), out int studentId))
            {
                try
                {
                    _courseService.AddStudentToCourse(courseId, studentId);
                    var course = _courseService.GetCourseById(courseId);
                    var student = _courseService.GetStudentById(studentId); 
                    Console.WriteLine($"Студент '{student?.Name}' успешно добавлен на курс '{course?.Name}'.");
                }
                catch (KeyNotFoundException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат ID студента.");
            }
        }
        else
        {
            Console.WriteLine("Неверный формат ID курса.");
        }
    }

    internal static void RemoveStudentFromCourse()
    {
        Console.WriteLine("\n--- Удаление студента с курса ---");
        Console.Write("Введите ID курса: ");
        if (int.TryParse(Console.ReadLine(), out int courseId))
        {
            Console.Write("Введите ID студента: ");
            if (int.TryParse(Console.ReadLine(), out int studentId))
            {
                try
                {
                    var course = _courseService.GetCourseById(courseId);
                    var student = _courseService.GetStudentById(studentId); 
                    if (course != null && student != null)
                    {
                        if (course.Students.Any(s => s.Id == studentId))
                        {
                            _courseService.RemoveStudentFromCourse(courseId, studentId);
                            Console.WriteLine($"Студент '{student.Name}' успешно удален с курса '{course.Name}'.");
                        }
                        else
                        {
                            Console.WriteLine($"Студент '{student.Name}' не записан на курс '{course.Name}'.");
                        }
                    }
                    else if (course == null)
                    {
                        Console.WriteLine($"Курс с ID {courseId} не найден.");
                    }
                    else
                    {
                        Console.WriteLine($"Студент с ID {studentId} не найден.");
                    }
                }
                catch (KeyNotFoundException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат ID студента.");
            }
        }
        else
        {
            Console.WriteLine("Неверный формат ID курса.");
        }
    }

    internal static void GetAllCourses()
    {
        Console.WriteLine("\n--- Список всех курсов ---");
        var courses = _courseService.GetAllCourses().ToList();
        if (courses.Any())
        {
            foreach (var course in courses)
            {
                Console.WriteLine($"- ID: {course.Id}, Название: {course.Name}, Тип: {course.GetType().Name}, Преподаватель: {(course.AssignedTeacher?.Name ?? "Нет")}, Студентов: {course.Students.Count}");
            }
        }
        else
        {
            Console.WriteLine("В системе нет ни одного курса.");
        }
    }

    internal static void GetCoursesByTeacher()
    {
        Console.WriteLine("\n--- Поиск курсов по преподавателю ---");
        Console.Write("Введите ID преподавателя: ");
        if (int.TryParse(Console.ReadLine(), out int teacherId))
        {
            try
            {
                var courses = _courseService.GetCoursesByTeacher(teacherId).ToList();
                if (courses.Any())
                {
                    var teacher = _courseService.GetTeacherById(teacherId); 
                    Console.WriteLine($"Курсы, которые ведет {teacher?.Name ?? $"преподаватель с ID {teacherId}"}:");
                    foreach (var course in courses)
                    {
                        Console.WriteLine($"- {course.Name} (ID: {course.Id})");
                    }
                }
                else
                {
                    Console.WriteLine($"Преподаватель с ID {teacherId} не ведет ни одного курса.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Неверный формат ID преподавателя.");
        }
    }

    internal static void GetStudentsByCourse()
    {
        Console.WriteLine("\n--- Поиск студентов по курсу ---");
        Console.Write("Введите ID курса: ");
        if (int.TryParse(Console.ReadLine(), out int courseId))
        {
            try
            {
                var students = _courseService.GetStudentsByCourse(courseId).ToList();
                if (students.Any())
                {
                    var course = _courseService.GetCourseById(courseId);
                    Console.WriteLine($"Студенты на курсе '{course?.Name ?? $"курс с ID {courseId}"}':");
                    foreach (var student in students)
                    {
                        Console.WriteLine($"- {student.Name} (ID: {student.Id})");
                    }
                }
                else
                {
                    var course = _courseService.GetCourseById(courseId);
                    if (course != null)
                    {
                        Console.WriteLine($"На курс '{course.Name}' пока никто не записан.");
                    }
                    else
                    {
                        Console.WriteLine($"Курс с ID {courseId} не найден.");
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Неверный формат ID курса.");
        }
    }

    internal static void AddTeacher()
    {
        Console.WriteLine("\n--- Добавление преподавателя ---");
        Console.Write("Введите имя преподавателя: ");
        string name = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Имя преподавателя не может быть пустым.");
            return;
        }

        int nextId = _courseService.GetAllTeachers().DefaultIfEmpty(new Teacher(0, "тест")).Max(t => t.Id) + 1; 

        try
        {
            var newTeacher = new Teacher(nextId, name);
            _courseService.AddTeacher(newTeacher);
            Console.WriteLine($"Преподаватель '{name}' успешно добавлен с ID: {newTeacher.Id}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    internal static void AddStudent()
    {
        Console.WriteLine("\n--- Добавление студента ---");
        Console.Write("Введите имя студента: ");
        string name = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Имя студента не может быть пустым.");
            return;
        }

        int nextId = _courseService.GetAllStudents().DefaultIfEmpty(new Student(0, "dummy")).Max(s => s.Id) + 1; // Простой способ получить следующий ID

        try
        {
            var newStudent = new Student(nextId, name);
            _courseService.AddStudent(newStudent);
            Console.WriteLine($"Студент '{name}' успешно добавлен с ID: {newStudent.Id}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    internal static List<Teacher> GetAllTeachers()
    {
        return (List<Teacher>)_courseService.GetAllCourses().SelectMany(c => Enumerable.Empty<Teacher>()).Concat(_courseService.GetAllCourses().Where(c => c.AssignedTeacher != null).Select(c => c.AssignedTeacher!)).DistinctBy(t => t.Id).Concat(_courseService.GetAllTeachers());
    }

    internal static List<Student> GetAllStudents()
    {
        return (List<Student>)_courseService.GetAllCourses().SelectMany(c => c.Students).Concat(_courseService.GetAllStudents()).DistinctBy(s => s.Id);
    }
    }
}