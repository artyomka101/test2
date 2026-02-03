namespace EduPortal.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? CourseId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public override string ToString()
        {
            return $"{FullName} ({GroupName})";
        }
    }
}