namespace P01_StudentSystem.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }


        [StringLength(10, MinimumLength =10)]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime RegisteredOn
        { get; set; }

        public DateTime? Birthday { get; set; }

        public ICollection<StudentCourse> CourseEnrollments { get; set; }

        public ICollection<Homework> HomeworkSubmissions { get; set; }
    }
}
