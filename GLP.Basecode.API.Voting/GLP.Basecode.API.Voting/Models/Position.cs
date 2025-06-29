using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class Position
{
    public long PositionId { get; set; }

    public string PositionName { get; set; } = null!;

    public int SequenceNumber { get; set; }

    public long CandidateId { get; set; }

    public long SyId { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual SchoolYear Sy { get; set; } = null!;
}
