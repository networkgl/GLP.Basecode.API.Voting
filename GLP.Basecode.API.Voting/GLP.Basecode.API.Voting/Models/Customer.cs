using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Voting.Models;

public partial class Customer
{
    public string CustCode { get; set; } = null!;

    public string? CustName { get; set; }

    public string? CustCity { get; set; }

    public string? WorkingArea { get; set; }

    public string? CustCountry { get; set; }

    public string? Grade { get; set; }

    public string? OpeningAmt { get; set; }

    public string? RecieveAmt { get; set; }

    public string? PaymentAmt { get; set; }

    public string? OutstandingAmt { get; set; }
}
