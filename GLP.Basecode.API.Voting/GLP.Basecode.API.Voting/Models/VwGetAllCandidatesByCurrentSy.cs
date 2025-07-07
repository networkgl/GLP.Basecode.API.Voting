using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class VwGetAllCandidatesByCurrentSy
{
    public long CandidateId { get; set; }

    public string CandidateName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public int SyFrom { get; set; }

    public int SyTo { get; set; }
}
