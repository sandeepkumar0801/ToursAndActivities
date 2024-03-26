using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities
{
    public class Login
    {
        public string? username { get; set; }
     
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string? grant_type { get; set; }

    }

    public class LoginAuthorization
    {
        public string username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }


    }


    public class TokenResponseAuthorization
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        public List<string> Roles { get; set; }


    }
}
