using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TimelineForms.Converters
{
    public class RelativeTimeConverter : IValueConverter
    {
        /// <summary>
        /// A minute defined in seconds.
        /// </summary>
        private const double Minute = 60.0;

        /// <summary>
        /// An hour defined in seconds.
        /// </summary>
        private const double Hour = 60.0 * Minute;

        /// <summary>
        /// A day defined in seconds.
        /// </summary>
        private const double Day = 24 * Hour;

        /// <summary>
        /// A month defined in seconds.
        /// </summary>
        private const double Month = 30.5 * Day;

        /// <summary>
        /// A year defined in seconds.
        /// </summary>
        private const double Year = 365 * Day;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            DateTime given;

            if (value is DateTime)
                given = ((DateTime)value).ToLocalTime();
            else if (value is DateTimeOffset)
                given = ((DateTimeOffset)value).LocalDateTime;
            else
                return null;

            string result = null;

            var current = DateTime.Now;
            var difference = current - given;

            if (difference.TotalSeconds > Year)
                result = given.Year.ToString();
            else if (difference.TotalSeconds > Day)
                result = given.ToString("d");
            else if (difference.TotalSeconds > Hour)
                result = $"{difference.Hours} hour{(difference.Hours == 1 ? null:"s")} ago";
            else if (difference.TotalSeconds > Minute)
                result = $"{difference.Minutes} minute{(difference.Minutes == 1 ? null : "s")} ago";
            else if (difference.Seconds > 0)
                result = $"{difference.Seconds} second{(difference.Seconds == 1 ? null : "s")} ago";
            else
                result = "just now";

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
