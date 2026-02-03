namespace EduPortal.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public override string ToString()
        {
            return $"{FullName} - {Subject}";
        }
    }
}