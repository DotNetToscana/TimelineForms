using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelineForms.Common.Models
{
    public class Comment
    {
        [JsonProperty("id")]
        public string CommentId { get; set; }

        public User Sender { get; set; }

        public string Text { get; set; }

        public DateTimeOffset SentDate { get; set; }

        public string PostId { get; set; }
    }
}
