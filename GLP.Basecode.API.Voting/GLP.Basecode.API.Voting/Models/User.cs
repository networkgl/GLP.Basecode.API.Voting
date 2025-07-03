using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class User
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
