using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TimelineForms.Converters
{
    public class ImageSourceConverter : IValueConverter
    {
        private static HttpClient client;

        static ImageSourceConverter()
        {
            client = new HttpClient();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                try
                {
                    var buffer = client.GetByteArrayAsync(value.ToString()).Result;
                    return ImageSource.FromStream(() => new MemoryStream(buffer));
                }
                catch { }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
