namespace EduPortal.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public int Credits { get; set; }
        public int Semester { get; set; }
        public string Description { get; set; } = string.Empty;
        
        // Для отображения в интерфейсе
        public string TeacherName { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{CourseName} ({Credits} кредитов, {Duration})";
        }
    }
}