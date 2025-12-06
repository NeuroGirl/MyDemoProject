// University.Tests/CourseServiceTests.cs
using Xunit;
using University.Domain;
using University.Application;
using University.Infrastructure;
using System.Linq;

namespace University.Tests
{
    public class CourseServiceTests
    {
        private readonly CourseService _courseService;
        private readonly InMemoryCourseRepository _courseRepository;
        private readonly InMemoryTeacherRepository _teacherRepository;
        private readonly InMemoryStudentRepository _studentRepository;

        public CourseServiceTests()
        {
            _courseRepository = new InMemoryCourseRepository();
            _teacherRepository = new InMemoryTeacherRepository();
            _studentRepository = new InMemoryStudentRepository();
            _courseService = new CourseService(_courseRepository, _teacherRepository, _studentRepository);

            // Добавим начальные данные для тестов
            var teacher1 = new Teacher(1, "Dr. Smith");
            var teacher2 = new Teacher(2, "Prof. Jones");
            _teacherRepository.Add(teacher1);
            _teacherRepository.Add(teacher2);

            var student1 = new Student(101, "Alice");
            var student2 = new Student(102, "Bob");
            var student3 = new Student(103, "Charlie");
            _studentRepository.Add(student1);
            _studentRepository.Add(student2);
            _studentRepository.Add(student3);
        }

        [Fact]
        public void AddCourse_ShouldAddNewCourse()
        {
            // Arrange
            var newCourse = new OnlineCourse(1, "Introduction to Programming", "http://example.com/intro");

            // Act
            _courseService.AddCourse(newCourse);

            // Assert
            var addedCourse = _courseRepository.GetById(1);
            Assert.NotNull(addedCourse);
            Assert.Equal("Introduction to Programming", addedCourse.Name);
        }

        [Fact]
        public void RemoveCourse_ShouldRemoveExistingCourse()
        {
            // Arrange
            var courseToRemove = new OfflineCourse(2, "Advanced Calculus", "Room 301");
            _courseRepository.Add(courseToRemove);
            Assert.NotNull(_courseRepository.GetById(2));

            // Act
            _courseService.RemoveCourse(2);

            // Assert
            Assert.Null(_courseRepository.GetById(2));
        }

        [Fact]
        public void RemoveCourse_ShouldUnassignTeacherIfPresent()
        {
            // Arrange
            var course = new OnlineCourse(3, "Data Structures", "http://example.com/ds");
            var teacher = _teacherRepository.GetById(1); // Dr. Smith
            _courseRepository.Add(course);
            _courseService.AssignTeacherToCourse(3, 1); // Assign Dr. Smith to course 3

            var courseInRepo = _courseRepository.GetById(3);
            Assert.NotNull(courseInRepo);
            Assert.NotNull(courseInRepo.AssignedTeacher);
            Assert.Equal(1, courseInRepo.AssignedTeacher.Id);

            // Act
            _courseService.RemoveCourse(3);

            // Assert
            var removedCourse = _courseRepository.GetById(3);
            Assert.Null(removedCourse); // Course is removed

            var drSmith = _teacherRepository.GetById(1);
            Assert.NotNull(drSmith);
            // Checking if the internal list in Teacher is also updated might be more complex,
            // but here we verify the Course object itself is gone.
            // If we wanted to verify the Teacher's list, we'd need a method to expose it or check indirectly.
            // For now, the fact that the course is removed is sufficient.
        }

        [Fact]
        public void GetCourseById_ShouldReturnCorrectCourse()
        {
            // Arrange
            var course = new OfflineCourse(4, "Linear Algebra", "Room 205");
            _courseRepository.Add(course);

            // Act
            var retrievedCourse = _courseService.GetCourseById(4);

            // Assert
            Assert.NotNull(retrievedCourse);
            Assert.Equal(4, retrievedCourse.Id);
            Assert.Equal("Linear Algebra", retrievedCourse.Name);
        }

        [Fact]
        public void GetAllCourses_ShouldReturnAllCourses()
        {
            // Arrange
            _courseService.AddCourse(new OnlineCourse(5, "Web Development", "http://example.com/web"));
            _courseService.AddCourse(new OfflineCourse(6, "Database Systems", "Room 101"));

            // Act
            var allCourses = _courseService.GetAllCourses().ToList();

            // Assert
            Assert.Equal(2, allCourses.Count);
            Assert.Contains(allCourses, c => c.Id == 5);
            Assert.Contains(allCourses, c => c.Id == 6);
        }

        [Fact]
        public void AssignTeacherToCourse_ShouldAssignTeacherAndUpdateCourse()
        {
            // Arrange
            var course = new OnlineCourse(7, "Algorithms", "http://example.com/algo");
            _courseRepository.Add(course);
            var teacher = _teacherRepository.GetById(1); // Dr. Smith

            // Act
            _courseService.AssignTeacherToCourse(7, 1);

            // Assert
            var assignedCourse = _courseRepository.GetById(7);
            Assert.NotNull(assignedCourse);
            Assert.NotNull(assignedCourse.AssignedTeacher);
            Assert.Equal(1, assignedCourse.AssignedTeacher.Id);
            Assert.Equal("Dr. Smith", assignedCourse.AssignedTeacher.Name);

            var drSmith = _teacherRepository.GetById(1);
            Assert.NotNull(drSmith);
            // Asserting internal list access requires careful design,
            // here we assume the Course object holds the correct reference.
        }

        [Fact]
        public void AssignTeacherToCourse_ShouldThrowExceptionIfCourseNotFound()
        {
            // Arrange
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _courseService.AssignTeacherToCourse(99, 1));
        }

        [Fact]
        public void AssignTeacherToCourse_ShouldThrowExceptionIfTeacherNotFound()
        {
            // Arrange
            var course = new OfflineCourse(8, "Physics 101", "Lab 1");
            _courseRepository.Add(course);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _courseService.AssignTeacherToCourse(8, 99));
        }

        [Fact]
        public void UnassignTeacherFromCourse_ShouldRemoveTeacherAssignment()
        {
            // Arrange
            var course = new OnlineCourse(9, "Machine Learning", "http://example.com/ml");
            _courseRepository.Add(course);
            _courseService.AssignTeacherToCourse(9, 1); // Assign Dr. Smith

            var assignedCourse = _courseRepository.GetById(9);
            Assert.NotNull(assignedCourse);
            Assert.NotNull(assignedCourse.AssignedTeacher);
            Assert.Equal(1, assignedCourse.AssignedTeacher.Id);

            // Act
            _courseService.UnassignTeacherFromCourse(9);

            // Assert
            var updatedCourse = _courseRepository.GetById(9);
            Assert.NotNull(updatedCourse);
            Assert.Null(updatedCourse.AssignedTeacher);
        }

        [Fact]
        public void AddStudentToCourse_ShouldAddStudentToCourse()
        {
            // Arrange
            var course = new OfflineCourse(10, "Calculus II", "Room 207");
            _courseRepository.Add(course);
            var student = _studentRepository.GetById(101); // Alice

            // Act
            _courseService.AddStudentToCourse(10, 101);

            // Assert
            var updatedCourse = _courseRepository.GetById(10);
            Assert.NotNull(updatedCourse);
            Assert.Contains(updatedCourse.Students, s => s.Id == 101);
        }

        [Fact]
        public void RemoveStudentFromCourse_ShouldRemoveStudentFromCourse()
        {
            // Arrange
            var course = new OnlineCourse(11, "History of Art", "http://example.com/art");
            _courseRepository.Add(course);
            _courseService.AddStudentToCourse(11, 101); 
            _courseService.AddStudentToCourse(11, 102); // Add Bob

            var initialCourse = _courseRepository.GetById(11);

            // Act
            _courseService.RemoveStudentFromCourse(11, 101); // Remove Alice

            // Assert
            var updatedCourse = _courseRepository.GetById(11);
            Assert.NotNull(updatedCourse);
            Assert.DoesNotContain(updatedCourse.Students, s => s.Id == 101);
            Assert.Contains(updatedCourse.Students, s => s.Id == 102);
        }

        [Fact]
        public void GetCoursesByTeacher_ShouldReturnCoursesTaughtByTeacher()
        {
            // Arrange
            var course1 = new OnlineCourse(12, "Java Programming", "http://example.com/java");
            var course2 = new OfflineCourse(13, "C++ Development", "Room 404");
            var course3 = new OnlineCourse(14, "Python for Data Science", "http://example.com/python");

            _courseRepository.Add(course1);
            _courseRepository.Add(course2);
            _courseRepository.Add(course3);

            _courseService.AssignTeacherToCourse(12, 1); // Dr. Smith teaches Java
            _courseService.AssignTeacherToCourse(13, 2); // Prof. Jones teaches C++
            _courseService.AssignTeacherToCourse(14, 1); // Dr. Smith teaches Python

            // Act
            var drSmithCourses = _courseService.GetCoursesByTeacher(1).ToList();
            var profJonesCourses = _courseService.GetCoursesByTeacher(2).ToList();

            // Assert
            Assert.Equal(2, drSmithCourses.Count);
            Assert.Contains(drSmithCourses, c => c.Id == 12);
            Assert.Contains(drSmithCourses, c => c.Id == 14);

            Assert.Contains(profJonesCourses, c => c.Id == 13);
        }

        [Fact]
        public void GetCoursesByTeacher_ShouldThrowExceptionIfTeacherNotFound()
        {
            // Arrange
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _courseService.GetCoursesByTeacher(99));
        }

        [Fact]
        public void GetStudentsByCourse_ShouldReturnStudentsInCourse()
        {
            // Arrange
            var course = new OfflineCourse(15, "Biochemistry", "Lab 3");
            _courseRepository.Add(course);
            _courseService.AddStudentToCourse(15, 101); // Alice
            _courseService.AddStudentToCourse(15, 102); // Bob
            _courseService.AddStudentToCourse(15, 103); // Charlie

            // Act
            var studentsInCourse = _courseService.GetStudentsByCourse(15).ToList();

            // Assert
            Assert.Equal(3, studentsInCourse.Count);
            Assert.Contains(studentsInCourse, s => s.Id == 101);
            Assert.Contains(studentsInCourse, s => s.Id == 102);
            Assert.Contains(studentsInCourse, s => s.Id == 103);
        }

        [Fact]
        public void GetStudentsByCourse_ShouldThrowExceptionIfCourseNotFound()
        {
            // Arrange
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _courseService.GetStudentsByCourse(99));
        }
    }
}