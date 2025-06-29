using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class SchoolYear
{
    public long SyId { get; set; }

    public int FromSy { get; set; }

    public int ToSy { get; set; }

    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
