using University.Domain;
using University.Application;

namespace University.Infrastructure
{
    public class InMemoryCourseRepository : ICourseRepository
    {
        private readonly Dictionary<int, Course> _courses = new Dictionary<int, Course>();

        public void Add(Course course)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            if (_courses.ContainsKey(course.Id)) throw new ArgumentException($"Course with ID {course.Id} already exists.");
            _courses[course.Id] = course;
        }

        public void Remove(int courseId)
        {
            _courses.Remove(courseId);
        }

        public Course? GetById(int courseId)
        {
            return _courses.TryGetValue(courseId, out var course) ? course : null;
        }

        public List<Course> GetAll()
        {
            return _courses.Values.ToList();
        }

        public List<Course> GetCoursesByTeacherId(int teacherId)
        {
            return _courses.Values.Where(c => c.AssignedTeacher?.Id == teacherId).ToList();
        }
    }

    public class InMemoryTeacherRepository : ITeacherRepository
    {
        private readonly Dictionary<int, Teacher> _teachers = new Dictionary<int, Teacher>();

        public void Add(Teacher teacher)
        {
            if (teacher == null) throw new ArgumentNullException(nameof(teacher));
            if (_teachers.ContainsKey(teacher.Id)) throw new ArgumentException($"Teacher with ID {teacher.Id} already exists.");
            _teachers[teacher.Id] = teacher;
        }

        public void Remove(int teacherId)
        {
            _teachers.Remove(teacherId);
        }

        public Teacher? GetById(int teacherId)
        {
            return _teachers.TryGetValue(teacherId, out var teacher) ? teacher : null;
        }

        public List<Teacher> GetAll()
        {
            return _teachers.Values.ToList();
        }
    }

    public class InMemoryStudentRepository : IStudentRepository
    {
        private readonly Dictionary<int, Student> _students = new Dictionary<int, Student>();

        public void Add(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            if (_students.ContainsKey(student.Id)) throw new ArgumentException($"Студент с номером {student.Id} уже существует.");
            _students[student.Id] = student;
        }

        public void Remove(int studentId)
        {
            _students.Remove(studentId);
        }

        public Student? GetById(int studentId)
        {
            return _students.TryGetValue(studentId, out var student) ? student : null;
        }

        public List<Student> GetAll()
        {
            return _students.Values.ToList();
        }
    }
}