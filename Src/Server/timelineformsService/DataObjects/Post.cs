using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace timelineformsService.DataObjects
{
    public class Post : EntityData
    {
        [StringLength(50)]
        public string SenderId { get; set; }

        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTimeOffset SentDate { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public bool PublishOnFacebook { get; set; }
    }
}