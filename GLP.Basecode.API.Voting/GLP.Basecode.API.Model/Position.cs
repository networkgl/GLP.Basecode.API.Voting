using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Model
{
    public class Position
    {
        public long PositionId { get; set; }

        public string PositionName { get; set; } = null!;

        public int SequenceNumber { get; set; }

        public long SyId { get; set; }

        public virtual ICollection<CandidatePosition> CandidatePositions { get; set; } = new List<CandidatePosition>();

        public virtual SchoolYear Sy { get; set; } = null!;
    }

}

