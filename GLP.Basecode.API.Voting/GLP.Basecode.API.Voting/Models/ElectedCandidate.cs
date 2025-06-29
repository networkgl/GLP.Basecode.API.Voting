using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class ElectedCandidate
{
    public long ElecCanId { get; set; }

    public long CandidateName { get; set; }

    public string PositionName { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public short CourseYear { get; set; }

    public int VoteCount { get; set; }

    public int FromSy { get; set; }

    public int ToSy { get; set; }
}
