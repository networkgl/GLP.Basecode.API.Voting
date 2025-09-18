using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Model
{
    public class ElectedCandidate
    {
        public long ElecCanId { get; set; }

        public string CandidateName { get; set; } = null!;

        public string PositionName { get; set; } = null!;

        public string CourseName { get; set; } = null!;

        public short CourseYear { get; set; }

        public int VoteCount { get; set; }

        public int FromSy { get; set; }

        public int ToSy { get; set; }
    }

}

