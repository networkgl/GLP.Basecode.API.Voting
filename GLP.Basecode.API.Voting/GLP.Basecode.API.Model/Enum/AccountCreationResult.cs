using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLP.Basecode.API.Model.Enum
{
    public enum AccountCreationResult
    {
        Success,
        DuplicateIdNumber,
        DuplicateEmail,
        Error
    }
}
