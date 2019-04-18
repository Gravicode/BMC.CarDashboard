using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace BMC.CarDashboard
{
    public class VideoItem
    {
        public StorageFile MediaFile { get; set; }
        public string VideoID { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        private ImageSource mainImage;
        public ImageSource Image
        {
            get { return mainImage; }
            set
            {
                if (mainImage != value)
                {
                    mainImage = value;

                }
            }
        }
    }
}
