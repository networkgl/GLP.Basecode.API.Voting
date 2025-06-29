using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class UserVote
{
    public long VoteId { get; set; }

    public long? UserId { get; set; }

    public long? CandidateId { get; set; }

    public DateTime? VotedAt { get; set; }

    public virtual Candidate? Candidate { get; set; }

    public virtual User? User { get; set; }
}
