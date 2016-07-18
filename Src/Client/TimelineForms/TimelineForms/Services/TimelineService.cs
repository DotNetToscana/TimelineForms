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

namespace TimelineForms.Services
{
    public class TimelineService : ITimelineService
    {
        private readonly IMobileServiceClient client;
        private readonly IUserService userService;

        public bool IsLoaded => Posts.Count > 0;

        public ObservableCollection<TimelinePost> Posts { get; } = new ObservableCollection<TimelinePost>();

        public TimelineService(IMobileServiceClient client, IUserService userService)
        {
            this.client = client;
            this.userService = userService;
        }

        public async Task LoadAsync()
        {
            // Loads the user's timeline.
            var posts = await client.InvokeApiAsync<IEnumerable<TimelinePost>>("timeline", HttpMethod.Get, null);

            Posts.Clear();
            posts.ForEach(p => Posts.Add(p));
        }

        public async Task SendPostAsync(string message, bool publishOnFacebook)
        {
            // Sends the post to the mobile app.
            var post = new Post
            {
                Text = message,
                SentDate = DateTimeOffset.Now,
                PublishOnFacebook = publishOnFacebook
            };

            await client.GetTable<Post>().InsertAsync(post);

            // Adds the post to the current timeline.
            Posts.Insert(0, new TimelinePost
            {
                PostId = post.PostId,
                Sender = userService.CurrentUser,
                Text = post.Text,
                PublishOnFacebook = post.PublishOnFacebook,
                SentDate = post.SentDate
            });
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

        public async Task LoadCommentsOfAsync(Post post)
        {
            // Asks the service for the comments list.
            var servicePost = await client.GetTable<Post>().LookupAsync(post.PostId);
            post.Comments = new ObservableCollection<Comment>(servicePost.Comments.OrderBy(c => c.SentDate));
        }

        public async Task SendCommentAsync(Post post, string message)
        {
            // Sends the comment to the mobile app.
            var comment = new Comment
            {
                Text = message,
                SentDate = DateTimeOffset.Now,
                PostId = post.PostId
            };

            await client.GetTable<Comment>().InsertAsync(comment);

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
        }
    }
}
