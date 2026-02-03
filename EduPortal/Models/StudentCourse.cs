namespace EduPortal.Models
{
    public class StudentCourse
    {
        public int Id { get; set; }
        public int StudentCourseId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public decimal? Grade { get; set; }
        public string GradeType { get; set; } = "exam";

        // Для отображения в интерфейсе
        public string StudentName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
    }
}