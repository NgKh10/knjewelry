using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewelry.Common.Auth
{
    public class RoleConstant
    {

            // Vai trò trong database
            public const string Admin = "quan_tri";
            public const string Customer = "khach_hang";

            // Claim cho JWT
            public const string ClaimTypeRole = "role";
        
    }

}
