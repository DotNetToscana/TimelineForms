using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelineForms.Common.Models
{
    public class User
    {
        [JsonProperty("id")]
        public string UserId { get; set; }

        public string FacebookUserId { get; set; }

        public string EMail { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();

        public string ImageUrl { get; set; }
    }
}
