using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelineForms.Common.Models
{
    public class TimelinePost : Post
    {
        public Comment LastComment => Comments?.OrderByDescending(d => d.SentDate).FirstOrDefault();

        private int totalCommentsCount;
        public int TotalCommentsCount
        {
            get { return totalCommentsCount; }
            set
            {
                if (this.Set(ref totalCommentsCount, value))
                    RaisePropertyChanged(() => LastComment);
            }
        }
    }
}
