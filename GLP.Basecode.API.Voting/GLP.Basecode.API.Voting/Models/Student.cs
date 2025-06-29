using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class Student
{
    public long StudentId { get; set; }

    public long IdNumber { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public long? CandidateId { get; set; }

    public long SyId { get; set; }

    public long CourseId { get; set; }

    public virtual Candidate? Candidate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual SchoolYear Sy { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
