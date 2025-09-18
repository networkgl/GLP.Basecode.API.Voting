using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Model
{
    public class CandidatePosition
    {
        public long CanposId { get; set; }

        public long PositionId { get; set; }

        public long CandidateId { get; set; }

        public virtual Candidate Candidate { get; set; } = null!;

        public virtual Position Position { get; set; } = null!;
    }

}

