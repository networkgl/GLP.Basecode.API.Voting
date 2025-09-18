using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Model
{
    public class UserAccount
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = null!;

        public string UserPassword { get; set; } = null!;

        public string UserStatus { get; set; } = null!;
    }

}

