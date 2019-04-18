using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BMC.CarDashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayerPage : Page
    {
        public PlayerPage()
        {
            this.InitializeComponent();
            List1.IsItemClickEnabled = true;
            BtnExit.Click += (a, b) => { if (this.Frame.CanGoBack) this.Frame.GoBack(); else this.Frame.Navigate(typeof(MainPage)); };
            List1.ItemClick += async (a, b) =>
            {
                var item = b.ClickedItem as VideoItem;
                if (item != null)
                {
                    MediaPlayerHelper.CleanUpMediaPlayerSource(Player1.MediaPlayer);
                    //Player1.MediaPlayer.Source = new MediaItem(item.Url).MediaPlaybackItem;
                    Player1.MediaPlayer.Source = MediaSource.CreateFromStorageFile(item.MediaFile);
                    Player1.MediaPlayer.Play();
                }
            };
            BtnFind.Click += async (a, b) =>
            {
                Progress1.Visibility = Visibility.Visible;
                var Keyword = TxtSearch.Text;

                var datas = await SearchMedia(Keyword);
                List1.ItemsSource = datas;


                Progress1.Visibility = Visibility.Collapsed;
            };
        }



        async Task<ObservableCollection<VideoItem>> SearchMedia(string keyword = "")
        {
            try
            {
                keyword = keyword == null ? "" : keyword;
                ObservableCollection<VideoItem> datas = new ObservableCollection<VideoItem>();
                QueryOptions queryOption = new QueryOptions
           (CommonFileQuery.OrderByTitle, new string[] { ".mp3", ".mp4", ".mkv" });

                queryOption.FolderDepth = FolderDepth.Deep;

                Queue<IStorageFolder> folders = new Queue<IStorageFolder>();

                var files = await KnownFolders.MusicLibrary.CreateFileQueryWithOptions
                  (queryOption).GetFilesAsync();
                //int counter = 0;
                foreach (var file in files)
                {
                    if (file.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //counter++;
                        using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                        {

                            BitmapImage bitmapImage = Path.GetExtension(file.Name) == ".mp3" ? new BitmapImage(new Uri("ms-appx:///Assets/music.png")) : new BitmapImage(new Uri("ms-appx:///Assets/video.png"));

                            //Image1.Source = bitmapImage;
                            datas.Add(new VideoItem()
                            {
                                Image = bitmapImage,
                                Title = Path.GetFileNameWithoutExtension(file.DisplayName),
                                Desc = Path.GetExtension(file.Name) == ".mp3" ? "audio" : "Video",
                                MediaFile = file
                            });

                        }
                    }

                }
                return datas;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
