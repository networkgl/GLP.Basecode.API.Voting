using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class Role
{
    public short RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
