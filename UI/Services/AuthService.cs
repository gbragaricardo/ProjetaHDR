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
            if (username == "Hidro" &&  password == "Pjt@2025")
                return true;

            else if (username == "Dev" && password == "eddevmode")
                return true;

            else
                return false;
        }
    }
}