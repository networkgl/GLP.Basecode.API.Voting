using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class PartyList
{
    public long PartyListId { get; set; }

    public string PartyListName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool? IsCompleted { get; set; }

    public long FilePathId { get; set; }

    public virtual ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();

    public virtual FilePath FilePath { get; set; } = null!;
}
