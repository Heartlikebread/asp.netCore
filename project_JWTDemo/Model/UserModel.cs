using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace JWTDemo.Model
{
    public class UserModel
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }

        public string EmailAddress { get; set; }
    }
}
