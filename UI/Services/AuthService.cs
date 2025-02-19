using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.UI.Services
{
    public class AuthService
    {
        public bool Authenticate(string username, string password)
        {
            return username == "admin" && password == "1234";
        }
    }
}
