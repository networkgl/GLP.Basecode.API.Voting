using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLP.Basecode.API.Helper
{
    public class MaskEmailHeper
    {
        public static string Mask(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                return email;

            var parts = email.Split('@');
            var local = parts[0];
            var domain = parts[1];

            if (local.Length <= 2)
                return new string('*', local.Length) + "@" + domain;

            var firstChar = local[0];
            var lastChar = local[^1];
            var mask = new string('*', local.Length - 2);

            return $"{firstChar}{mask}{lastChar}@{domain}";
        }
    }
}
