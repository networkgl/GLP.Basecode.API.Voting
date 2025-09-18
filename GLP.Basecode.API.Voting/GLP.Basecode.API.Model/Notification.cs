using System;
using System.Collections.Generic;

namespace GLP.Basecode.API.Model
{
    public class Notification
    {
        public long NotifId { get; set; }

        public long StudentId { get; set; }

        public string Message { get; set; } = null!;

        public virtual Student Student { get; set; } = null!;
    }

}
