using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Model
{
    public class VwGetCandidateOrPartylistDetail
    {
        public long PartyListId { get; set; }

        public long? CandidateId { get; set; }

        public long? PositionId { get; set; }

        public long? StudentId { get; set; }

        public string CandidateName { get; set; } = null!;

        public string PartyListName { get; set; } = null!;

        public string? PositionName { get; set; }

        public long? FilePathId { get; set; }

        public string? CandidateImgPath { get; set; }

        public string PartyListImgPath { get; set; } = null!;
    }

}

