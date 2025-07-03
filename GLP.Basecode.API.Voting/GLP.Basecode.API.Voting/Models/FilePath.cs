using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class FilePath
{
    public long FilePathId { get; set; }

    public string Path { get; set; } = null!;

    public virtual ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();

    public virtual ICollection<PartyList> PartyLists { get; set; } = new List<PartyList>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
