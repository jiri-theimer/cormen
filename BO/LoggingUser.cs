using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class LoggingUser
    {
        [Display(Name ="Přihlašovací jméno (login)")]
        public string Login { get; set; }
        [Display(Name ="Heslo")]
        public string Password { get; set; }
        public int CookieExpiresInHours { get; set; } = 1;

        public string Message { get; set; }
    }
}
