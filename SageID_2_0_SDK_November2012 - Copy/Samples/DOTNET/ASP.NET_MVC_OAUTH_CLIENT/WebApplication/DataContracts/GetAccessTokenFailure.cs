using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace OAuthClientWebApp.DataContracts
{
    [DataContract]
    public class GetAccessTokenFailure
    {
        [DataMember(Name="error")]
        public string Error { get; set; }

        [DataMember(Name = "error_description")]
        public string ErrorDescription { get; set; }

        [DataMember(Name = "error_uri")]
        public string ErrorUri { get; set; }
    }
}