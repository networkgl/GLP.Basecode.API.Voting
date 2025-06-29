using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class FilePath
{
    public long FilePathId { get; set; }

    public string Path { get; set; } = null!;

    public long? UserId { get; set; }

    public long? CandidateId { get; set; }

    public long? PartyListId { get; set; }

    public virtual Candidate? Candidate { get; set; }

    public virtual PartyList? PartyList { get; set; }

    public virtual User? User { get; set; }
}
