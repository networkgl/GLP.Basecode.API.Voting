using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLP.Basecode.API.Model
{
    public class User
    {
        public long UserId { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? UserEmail { get; set; }

        public int? UserOtp { get; set; }

        public bool? IsVoted { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public long? StudentId { get; set; }

        public long? FilePathId { get; set; }

        public short RoleId { get; set; }

        public virtual FilePath? FilePath { get; set; }

        public virtual Role Role { get; set; } = null!;

        public virtual Student? Student { get; set; }

        public virtual ICollection<UserVote> UserVotes { get; set; } = new List<UserVote>();
    }

}


