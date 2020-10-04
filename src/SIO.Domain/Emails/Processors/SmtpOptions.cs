using System;
using System.Collections.Generic;
using System.Text;

namespace SIO.Domain.Emails.Processors
{
    public class SmtpOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromAddress { get; set; }
    }
}
