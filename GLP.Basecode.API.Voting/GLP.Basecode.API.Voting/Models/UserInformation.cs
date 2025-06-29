using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class UserInformation
{
    public int UserInfoId { get; set; }

    public string UserInfLname { get; set; } = null!;

    public string UserInfFname { get; set; } = null!;

    public string UserInfAddress { get; set; } = null!;

    public string? UserInfEmail { get; set; }
}
