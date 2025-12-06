namespace University.Domain
{
    public abstract class Course
    {
        public int Id { get; }
        public string Name { get; set; }
        public Teacher? AssignedTeacher { get; private set; }
        private readonly List<Student> _students = new List<Student>();

        public IReadOnlyCollection<Student> Students => _students.AsReadOnly();

        protected Course(int id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void AssignTeacher(Teacher teacher)
        {
            if (teacher == null)
            {
                throw new ArgumentNullException(nameof(teacher));
            }
            AssignedTeacher = teacher;
        }

        public void UnassignTeacher()
        {
            AssignedTeacher = null;
        }

        public void AddStudent(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student));
            }
            if (!_students.Contains(student))
            {
                _students.Add(student);
            }
        }

        public void RemoveStudent(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student));
            }
            _students.Remove(student);
        }

        public override string ToString()
        {
            return $"ID курса: {Id}, Название: {Name}, Преподаваель: {(AssignedTeacher?.Name ?? "None")}, Студентов: {_students.Count}";
        }
    }

    public class OnlineCourse : Course
    {
        public string PlatformUrl { get; set; }

        public OnlineCourse(int id, string name, string platformUrl) : base(id, name)
        {
            PlatformUrl = platformUrl ?? throw new ArgumentNullException(nameof(platformUrl));
        }

        public override string ToString()
        {
            return $"{base.ToString()}, Ссылка на курс: {PlatformUrl}";
        }
    }

    public class OfflineCourse : Course
    {
        public string Location { get; set; }

        public OfflineCourse(int id, string name, string location) : base(id, name)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public override string ToString()
        {
            return $"{base.ToString()}, Помещение: {Location}";
        }
    }

    public class Teacher
    {
        public int Id { get; }
        public string Name { get; set; }
        private readonly List<Course> _coursesTaught = new List<Course>();

        public IReadOnlyCollection<Course> CoursesTaught => _coursesTaught.AsReadOnly();

        public Teacher(int id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        internal void AddCourseToTaughtList(Course course)
        {
            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            if (!_coursesTaught.Contains(course))
            {
                _coursesTaught.Add(course);
            }
        }

        internal void RemoveCourseFromTaughtList(Course course)
        {
            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            _coursesTaught.Remove(course);
        }

        public override string ToString()
        {
            return $"ID Преподавателя: {Id}, Имя: {Name}, Курсы: {_coursesTaught.Count}";
        }
    }

    public class Student
    {
        public int Id { get; }
        public string Name { get; set; }

        public Student(int id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override string ToString()
        {
            return $"ID Студента: {Id}, Имя: {Name}";
        }
    }
}