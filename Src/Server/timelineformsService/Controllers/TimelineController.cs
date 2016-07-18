using timelineformsService.Models;
using Microsoft.Azure.Mobile.Server.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using TimelineForms.Common.Models;

namespace timelineformsService.Controllers
{
    [MobileAppController]
    [Authorize]
    public class TimelineController : ApiController
    {
        public async Task<IHttpActionResult> GetTimeline()
        {
            using (var db = new timelineformsContext())
            {
                var posts = db.Posts.OrderByDescending(p => p.SentDate).Take(50)
                    .Select(p => new TimelinePost
                    {
                        PostId = p.Id,
                        SentDate = p.SentDate,
                        Text = p.Text,
                        SenderId = p.SenderId,
                        Sender = new User
                        {
                            UserId = p.Sender.Id,
                            EMail = p.Sender.EMail,
                            FirstName = p.Sender.FirstName,
                            LastName = p.Sender.LastName,
                            ImageUrl = p.Sender.ImageUrl
                        },
                        Comments = p.Comments.OrderByDescending(c => c.SentDate).Take(1)
                        .Select(c => new Comment
                        {
                            CommentId = c.Id,
                            SentDate = c.SentDate,
                            Text = c.Text,
                            Sender = new User
                            {
                                UserId = c.Sender.Id,
                                EMail = c.Sender.EMail,
                                FirstName = c.Sender.FirstName,
                                LastName = c.Sender.LastName,
                                ImageUrl = c.Sender.ImageUrl
                            },
                        }).ToList(),
                        TotalCommentsCount = p.Comments.Count,
                        PublishOnFacebook = p.PublishOnFacebook
                    });

                var result = await posts.ToListAsync();
                return Ok(result);
            }
        }
    }
}
