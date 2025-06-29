using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class Course
{
    public long CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public string CourseAbbreviation { get; set; } = null!;

    public string DepartmentName { get; set; } = null!;

    public string DepartmentAbbreviation { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
