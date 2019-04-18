using MyToolkit.Multimedia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Web;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using YoutubeSearch;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BMC.CarDashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class YoutubePage : Page
    {
        public YoutubePage()
        {
            this.InitializeComponent();
            List1.IsItemClickEnabled = true;
            BtnExit.Click += (a, b) => { if (this.Frame.CanGoBack) this.Frame.GoBack(); else this.Frame.Navigate(typeof(MainPage)); };
            List1.ItemClick += async(a,b)=> {
                var item = b.ClickedItem as VideoItem;
                if (item != null)
                {
                    var url = await YouTube.GetVideoUriAsync(item.VideoID, YouTubeQuality.QualityLow);
                    MediaPlayerHelper.CleanUpMediaPlayerSource(Player1.MediaPlayer);
                    Player1.MediaPlayer.Source = new MediaItem(url.Uri.ToString()).MediaPlaybackItem;
                    Player1.MediaPlayer.Play();
                }
            };
            BtnFind.Click += async(a, b) => {
                Progress1.Visibility = Visibility.Visible;
                var Keyword = TxtSearch.Text;
                if (!string.IsNullOrEmpty(Keyword))
                {
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    var list = await SearchVideo(Keyword);
                    if (list != null)
                    {
                        var datas = new ObservableCollection<VideoItem>();
                        foreach (var item in list)
                        {

                            System.Net.Http.HttpResponseMessage imageResponse = await client.GetAsync(item.Thumbnail);

                            // A memory stream where write the image data
                            Windows.Storage.Streams.InMemoryRandomAccessStream randomAccess =
                            new Windows.Storage.Streams.InMemoryRandomAccessStream();

                            Windows.Storage.Streams.DataWriter writer =
                            new Windows.Storage.Streams.DataWriter(randomAccess.GetOutputStreamAt(0));

                            // Write and save the data into the stream
                            writer.WriteBytes(await imageResponse.Content.ReadAsByteArrayAsync());
                            await writer.StoreAsync();

                            // Create a Bitmap and assign it to the target Image control
                            Windows.UI.Xaml.Media.Imaging.BitmapImage bm =
                            new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                            await bm.SetSourceAsync(randomAccess);
                       
                            var uri = new Uri(item.Url);

                            // you can check host here => uri.Host <= "www.youtube.com"

                            var query = HttpUtility.ParseQueryString(uri.Query);
                            var videoId = query["v"];
                            datas.Add(new VideoItem() { Title = item.Title, Desc = item.Description, Image = bm, VideoID=videoId });
                        }
                        List1.ItemsSource = datas;
                    }
                }
                Progress1.Visibility = Visibility.Collapsed;
            };
        }

 

        async Task<List<VideoInformation>> SearchVideo(string querystring = "Stratovarius")
        {
            try
            {
                // Keyword


                // Number of result pages
                int querypages = 1;

                // Offset value for querypages
                //int querypagesOffset = 2;

                var items = new VideoSearch();

                foreach (var item in items.SearchQuery(querystring, querypages))
                {
                    Console.WriteLine(item.Title);
                }

                //For asynchronous execution use:
                var result = await items.SearchQueryTaskAsync(querystring, querypages);
                return result;


                //This will query from page 3 to 4
                //var offsetResult = items.SearchQuery(querystring, querypages, querypagesOffset);}ca
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
   
   
   
}
