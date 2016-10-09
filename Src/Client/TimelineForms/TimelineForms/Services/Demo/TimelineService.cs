using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineForms.Common.Models;
using Microsoft.WindowsAzure.MobileServices;
using System.Net.Http;
using TimelineForms.Extensions;

namespace TimelineForms.Services.Demo
{
    public class TimelineService : ITimelineService
    {
        private readonly IUserService userService;
        private readonly IEnumerable<User> demoUsers;

        public bool IsLoaded => Posts.Any();

        public ObservableCollection<TimelinePost> Posts { get; } = new ObservableCollection<TimelinePost>();

        public TimelineService(IUserService userService)
        {
            this.userService = userService;
            demoUsers = this.CreateUsers();
        }

        private IEnumerable<User> CreateUsers()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = "sid:0123456789",
                    FacebookUserId = "0123456789",
                    FirstName = "Marco",
                    LastName = "Minerva",
                    EMail = "mail@mail.com",
                    ImageUrl = "https://graph.facebook.com/10209897682705340/picture?width=150",
                    ProfileUrl = "https://www.facebook.com/app_scoped_user_id/10209897682705340/"
                },
                new User
                {
                    UserId = "sid:0123456789",
                    FacebookUserId = "0123456789",
                    FirstName = "Riccardo",
                    LastName = "Cappello",
                    EMail = "mail@mail.com",
                    ImageUrl = "https://graph.facebook.com/1379484041/picture?width=150",
                    ProfileUrl = "https://www.facebook.com/app_scoped_user_id/1379484041/"
                },
                new User
                {
                    UserId = "sid:0123456789",
                    FacebookUserId = "0123456789",
                    FirstName = "Armando",
                    LastName = "Sanremo",
                    EMail = "mail@mail.com",
                    ImageUrl = "https://graph.facebook.com/596120713895256/picture?width=150",
                    ProfileUrl = "https://www.facebook.com/app_scoped_user_id/596120713895256/"
                }
            };

            return users;
        }

        public Task LoadAsync()
        {
            if (!IsLoaded)
            {
                // Loads the user's timeline.
                var posts = new List<TimelinePost>
                {
                    new TimelinePost
                    {
                        PostId  = "1",
                        PublishOnFacebook = true,
                        Sender = demoUsers.ElementAt(0),
                        SenderId = demoUsers.ElementAt(0).UserId,
                        SentDate = DateTime.UtcNow.AddHours(-1),
                        Text = "Ciao a tutti!"
                    },
                    new TimelinePost
                    {
                        PostId  = "2",
                        PublishOnFacebook = false,
                        Sender = demoUsers.ElementAt(1),
                        SenderId = demoUsers.ElementAt(1).UserId,
                        SentDate = DateTime.UtcNow.AddMinutes(-75),
                        Text = "Mi dispiace un sacco non essere venuto all'evento :-("
                    },
                    new TimelinePost
                    {
                        PostId  = "3",
                        PublishOnFacebook = false,
                        Sender = demoUsers.ElementAt(2),
                        SenderId = demoUsers.ElementAt(2).UserId,
                        SentDate = DateTime.UtcNow.AddHours(-2),
                        Text = "Un saluto da Sanremo!",
                    }
                };

                posts.ForEach(p => Posts.Add(p));

                Posts.ForEach(async (p) =>
                {
                    await this.LoadCommentsOfAsync(p);
                    p.TotalCommentsCount = p.Comments.Count;
                });
            }

            return Task.Delay(500);
        }

        public Task SendPostAsync(string message, bool publishOnFacebook)
        {
            // Sends the post to the mobile app.
            var post = new Post
            {
                PostId = Guid.NewGuid().ToString(),
                Text = message,
                SentDate = DateTimeOffset.Now,
                PublishOnFacebook = publishOnFacebook
            };

            // Adds the post to the current timeline.
            Posts.Insert(0, new TimelinePost
            {
                PostId = post.PostId,
                Sender = userService.CurrentUser,
                Text = post.Text,
                PublishOnFacebook = post.PublishOnFacebook,
                SentDate = post.SentDate
            });

            return Task.FromResult<object>(null);
        }

        public async Task<TimelinePost> GetPostAsync(string postId)
        {
            // Gets the post and its comments.
            if (!this.IsLoaded)
                await this.LoadAsync();

            var post = Posts.FirstOrDefault(p => p.PostId == postId);

            if (post != null)
                await this.LoadCommentsOfAsync(post);

            return post;
        }

        public Task LoadCommentsOfAsync(Post post)
        {
            if (!post.Comments.Any())
            {
                var comments = new List<Comment>();
                for (int i = 1; i <= new Random((int)DateTime.Now.Ticks).Next(0, 6); i++)
                {
                    comments.Add(new Comment
                    {
                        CommentId = post.PostId,
                        PostId = (i % Posts.Count).ToString(),
                        Sender = demoUsers.ElementAt(i % Posts.Count),
                        SentDate = DateTime.UtcNow.AddMinutes(-i),
                        Text = $"Commento {i}."
                    });
                }

                post.Comments = new ObservableCollection<Comment>(comments.OrderBy(c => c.SentDate));
            }

            return Task.FromResult<object>(null);
        }

        public Task SendCommentAsync(Post post, string message)
        {
            // Sends the comment to the mobile app.
            var comment = new Comment
            {
                CommentId = Guid.NewGuid().ToString(),
                Text = message,
                SentDate = DateTimeOffset.Now,
                PostId = post.PostId
            };

            // Adds the comment to the post.
            post.Comments.Add(new Comment
            {
                CommentId = comment.CommentId,
                Sender = userService.CurrentUser,
                Text = comment.Text,
                SentDate = comment.SentDate
            });

            var timelinePost = post as TimelinePost;
            if (timelinePost != null)
                timelinePost.TotalCommentsCount++;

            return Task.FromResult<object>(null);
        }
    }
}
