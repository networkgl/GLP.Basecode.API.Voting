using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class RoomDetail
{
    public byte[]? RoomPhoto { get; set; }

    public string? RoomType { get; set; }

    public int? RoomDiscount { get; set; }
}
