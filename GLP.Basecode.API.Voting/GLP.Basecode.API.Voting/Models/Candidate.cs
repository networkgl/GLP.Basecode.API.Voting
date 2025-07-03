using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class Candidate
{
    public long CandidateId { get; set; }

    public long PartyListId { get; set; }

    public long? FilePathId { get; set; }

    public virtual FilePath? FilePath { get; set; }

    public virtual PartyList PartyList { get; set; } = null!;

    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<UserVote> UserVotes { get; set; } = new List<UserVote>();
}
