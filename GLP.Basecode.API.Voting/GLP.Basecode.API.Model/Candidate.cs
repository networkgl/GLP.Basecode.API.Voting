using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Model
{
    public class Candidate
    {
        public long CandidateId { get; set; }

        public long PartyListId { get; set; }

        public long FilePathId { get; set; }

        public long StudentId { get; set; }

        public virtual ICollection<CandidatePosition> CandidatePositions { get; set; } = new List<CandidatePosition>();

        public virtual FilePath FilePath { get; set; } = null!;

        public virtual PartyList PartyList { get; set; } = null!;

        public virtual Student Student { get; set; } = null!;

        public virtual ICollection<UserVote> UserVotes { get; set; } = new List<UserVote>();
    }

}

