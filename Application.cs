using University.Domain;

namespace University.Application
{
    public interface ICourseRepository
    {
        void Add(Course course);
        void Remove(int courseId);
        Course? GetById(int courseId);
        List<Course> GetAll();
        IEnumerable<Course> GetCoursesByTeacherId(int teacherId);
    }

    public interface ITeacherRepository
    {
        void Add(Teacher teacher);
        void Remove(int teacherId);
        Teacher? GetById(int teacherId);
        IEnumerable<Teacher> GetAll();
    }

    public interface IStudentRepository
    {
        void Add(Student student);
        void Remove(int studentId);
        Student? GetById(int studentId);
        IEnumerable<Student> GetAll();
    }

    public class CourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;

        public CourseService(ICourseRepository courseRepository, ITeacherRepository teacherRepository, IStudentRepository studentRepository)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        }

        public void AddCourse(Course course)
        {
            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            _courseRepository.Add(course);
        }

        public void RemoveCourse(int courseId)
        {
            var course = _courseRepository.GetById(courseId);
            if (course != null)
            {
                if (course.AssignedTeacher != null)
                {
                    var teacher = _teacherRepository.GetById(course.AssignedTeacher.Id);
                    if (teacher != null)
                    {
                        teacher.RemoveCourseFromTaughtList(course);
                    }
                }
                _courseRepository.Remove(courseId);
            }
        }

        public Course? GetCourseById(int courseId)
        {
            return _courseRepository.GetById(courseId);
        }

        public IEnumerable<Course> GetAllCourses()
        {
            return _courseRepository.GetAll();
        }

        public void AssignTeacherToCourse(int courseId, int teacherId)
        {
            var course = _courseRepository.GetById(courseId);
            var teacher = _teacherRepository.GetById(teacherId);

            if (course == null)
            {
                throw new KeyNotFoundException($"Курс с ID {courseId} не найден.");
            }
            if (teacher == null)
            {
                throw new KeyNotFoundException($"Преподаватель с ID {teacherId} не найден.");
            }

            if (course.AssignedTeacher != null && course.AssignedTeacher.Id != teacherId)
            {
                var oldTeacher = _teacherRepository.GetById(course.AssignedTeacher.Id);
                oldTeacher?.RemoveCourseFromTaughtList(course);
            }

            course.AssignTeacher(teacher);
            teacher.AddCourseToTaughtList(course); 
        }

        public void UnassignTeacherFromCourse(int courseId)
        {
            var course = _courseRepository.GetById(courseId);
            if (course == null)
            {
                throw new KeyNotFoundException($"Курс с ID {courseId} не найден.");
            }

            if (course.AssignedTeacher != null)
            {
                var teacher = _teacherRepository.GetById(course.AssignedTeacher.Id);
                teacher?.RemoveCourseFromTaughtList(course);
                course.UnassignTeacher();
            }
        }

        public void AddStudentToCourse(int courseId, int studentId)
        {
            var course = _courseRepository.GetById(courseId);
            var student = _studentRepository.GetById(studentId);

            if (course == null)
            {
                throw new KeyNotFoundException($"Курс с ID {courseId} не найден.");
            }
            if (student == null)
            {
                throw new KeyNotFoundException($"Студент с ID {studentId} не найден.");
            }

            course.AddStudent(student);
        }

        public void RemoveStudentFromCourse(int courseId, int studentId)
        {
            var course = _courseRepository.GetById(courseId);
            var student = _studentRepository.GetById(studentId);

            if (course == null)
            {
                throw new KeyNotFoundException($"Курс с ID {courseId} не найден.");
            }
            if (student == null)
            {
                throw new KeyNotFoundException($"Студент с ID {studentId} не найден.");
            }

            course.RemoveStudent(student);
        }

        public IEnumerable<Course> GetCoursesByTeacher(int teacherId)
        {
            if (_teacherRepository.GetById(teacherId) == null)
            {
                throw new KeyNotFoundException($"Преподаватель с ID {teacherId} не найден.");
            }
            return _courseRepository.GetCoursesByTeacherId(teacherId);
        }

        public IEnumerable<Student> GetStudentsByCourse(int courseId)
        {
            var course = _courseRepository.GetById(courseId);
            if (course == null)
            {
                throw new KeyNotFoundException($"Курс с ID {courseId} не найден.");
            }
            return course.Students;
        }

        public void AddTeacher(Teacher teacher)
        {
            if (teacher == null)
            {
                throw new ArgumentNullException(nameof(teacher));
            }
            _teacherRepository.Add(teacher);
        }

        public void AddStudent(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student));
            }
            _studentRepository.Add(student);
        }

        public Teacher? GetTeacherById(int teacherId)
        {
             return _teacherRepository.GetById(teacherId);
        }

        public Student? GetStudentById(int studentId)
        {
            return _studentRepository.GetById(studentId);
        }
        public IEnumerable<Teacher> GetAllTeachers()
        {
            return _teacherRepository.GetAll();
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return _studentRepository.GetAll();
        }
    }
}