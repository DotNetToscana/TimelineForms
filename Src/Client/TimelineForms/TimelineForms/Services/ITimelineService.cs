using TimelineForms.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelineForms.Services
{
    public interface ITimelineService
    {
        bool IsLoaded { get; }

        ObservableCollection<TimelinePost> Posts { get; }

        Task LoadAsync();

        Task<TimelinePost> GetPostAsync(string postId);

        Task LoadCommentsOfAsync(Post post);

        Task SendPostAsync(string message, bool postOnFacebook);

        Task SendCommentAsync(Post post, string message);
    }
}
