using University.Application; 
using University.Domain;
using University.Infrastructure; 

namespace Tests.Tests 
{
    public class CourseServiceTests
    {

        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            var courseRepository = new InMemoryCourseRepository();
            var teacherRepository = new InMemoryTeacherRepository();
            var studentRepository = new InMemoryStudentRepository();

            _courseService = new CourseService(courseRepository, teacherRepository, studentRepository);

        }

        [Fact] 
        public void AddCourse_WhenValidCourse_AddsCourseSuccessfully()
        {
            // Arrange 
            var courseName = "New Programming Course";
            var platformUrl = "http://example.com/programming";
            int expectedCourseCount = _courseService.GetAllCourses().Count() + 1; 

            // Act 
            var newCourse = new OnlineCourse(2, courseName, platformUrl); 
            _courseService.AddCourse(newCourse);

            // Assert 
            var allCourses = _courseService.GetAllCourses().ToList();
            Assert.Equal(expectedCourseCount, allCourses.Count); 
            var addedCourse = allCourses.FirstOrDefault(c => c.Name == courseName);
            Assert.NotNull(addedCourse); 
            Assert.Equal(courseName, addedCourse.Name); 
            Assert.IsType<OnlineCourse>(addedCourse); 
            Assert.Equal(platformUrl, ((OnlineCourse)addedCourse).PlatformUrl); 
        }

        
        [Fact]
        public void RemoveCourse_WhenValidId_RemovesCourseSuccessfully()
        {
            // Arrange
            var courseName = "New Programming Course";
            var platformUrl = "http://example.com/programming";
            var newCourse = new OnlineCourse(1, courseName, platformUrl); 
            _courseService.AddCourse(newCourse);
            int courseIdToRemove = 1; 
            int expectedCourseCount = _courseService.GetAllCourses().Count() - 1;

            // Act
            _courseService.RemoveCourse(courseIdToRemove);

            // Assert
            var allCourses = _courseService.GetAllCourses().ToList();
            Assert.Equal(expectedCourseCount, allCourses.Count);
            Assert.Null(_courseService.GetCourseById(courseIdToRemove)); 
        }

        [Fact]
        public void AssignTeacherToCourse_WhenValidIds_AssignsTeacherSuccessfully()
        {

            // Arrange
            int courseId = 1;
            int teacherId = 1; 
            var courseName = "New Programming Course";
            var platformUrl = "http://example.com/programming";
            var newCourse = new OnlineCourse(1, courseName, platformUrl); 
            var teacher1 = new Teacher(1, "Dr. Smith");
            _courseService.AddTeacher(teacher1);
            _courseService.AddCourse(newCourse);
            // Act
            _courseService.AssignTeacherToCourse(courseId, teacherId);

            // Assert
            var course = _courseService.GetCourseById(courseId);
            Assert.NotNull(course);
            Assert.NotNull(course.AssignedTeacher);
            Assert.Equal(teacherId, course.AssignedTeacher.Id);
        }

    }
}
