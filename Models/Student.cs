using System.ComponentModel.DataAnnotations;

namespace Project_10.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enrollment number is required.")]
        public string EnrollmentNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Student name is required.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Branch is required.")]
        public string Branch { get; set; } = string.Empty;

        [Range(1, 8, ErrorMessage = "Semester must be between 1 and 8.")]
        public int Semester { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;
    }
}