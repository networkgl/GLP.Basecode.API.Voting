using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class VwGetCandidateDetail
{
    public long CandidateId { get; set; }

    public long PositionId { get; set; }

    public long StudentId { get; set; }

    public string CandidateName { get; set; } = null!;

    public string PartyListName { get; set; } = null!;

    public string PositionName { get; set; } = null!;

    public long FilePathId { get; set; }

    public string ImgPath { get; set; } = null!;
}
