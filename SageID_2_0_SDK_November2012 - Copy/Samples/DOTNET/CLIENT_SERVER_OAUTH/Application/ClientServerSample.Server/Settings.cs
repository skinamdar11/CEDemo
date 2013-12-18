using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ClientServerSample.Server
{
    public static class Settings
    {
        public static string ClientId { get { return ConfigurationManager.AppSettings["ClientId"]; } }
        public static string Scope { get { return ConfigurationManager.AppSettings["Scope"]; } }
    }
}
