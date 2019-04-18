using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace BMC.CarDashboard
{
    public class MediaItem
    {
        public MediaPlaybackItem MediaPlaybackItem { get; private set; }
        public Uri Uri { set; get; }
        public MediaItem(string Url)
        {
            Uri = new Uri(Url);
            MediaPlaybackItem = new MediaPlaybackItem(MediaSource.CreateFromUri(Uri));
        }
    }
}
