using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelineForms.Common.Models
{
    public class Post : ObservableObject
    {
        [JsonProperty("id")]
        public string PostId { get; set; }

        public string SenderId { get; set; }

        public User Sender { get; set; }

        public string Text { get; set; }

        public DateTimeOffset SentDate { get; set; }

        private ICollection<Comment> comments = new ObservableCollection<Comment>();
        public ICollection<Comment> Comments
        {
            get { return comments; }
            set { this.Set(ref comments, value); }
        }

        public bool PublishOnFacebook { get; set; }
    }
}
