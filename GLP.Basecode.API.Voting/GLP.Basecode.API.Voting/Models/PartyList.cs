using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class PartyList
{
    public long PartyListId { get; set; }

    public string PartyListName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool? IsCompleted { get; set; }

    public virtual ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();

    public virtual ICollection<FilePath> FilePaths { get; set; } = new List<FilePath>();
}
