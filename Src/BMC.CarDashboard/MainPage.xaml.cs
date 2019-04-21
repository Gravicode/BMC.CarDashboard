using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using YoutubeSearch;

using System.Threading.Tasks;
using Windows.UI.Popups;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BMC.CarDashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static int change = 1;
        static long counter = 0;
        private DispatcherTimer timer;
        public MainPage()
        {
            this.InitializeComponent();
            Setup();
        }


        async void Setup()
        {
            ObservableCollection<ImageItem> datas = new ObservableCollection<ImageItem>();
            QueryOptions queryOption = new QueryOptions
       (CommonFileQuery.OrderByTitle, new string[] { ".bmp", ".jpg", ".jpeg" });

            queryOption.FolderDepth = FolderDepth.Deep;

            Queue<IStorageFolder> folders = new Queue<IStorageFolder>();

            var files = await KnownFolders.PicturesLibrary.CreateFileQueryWithOptions
              (queryOption).GetFilesAsync();
            //int counter = 0;
            foreach (var file in files)
            {
                //counter++;
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))

                {

                    BitmapImage bitmapImage = new BitmapImage();

                    //bitmapImage.DecodePixelHeight = decodePixelHeight.Text;

                    //bitmapImage.DecodePixelWidth = decodePixelWidth.Text;

                    await bitmapImage.SetSourceAsync(fileStream);

                    //Image1.Source = bitmapImage;
                    datas.Add(new ImageItem()
                    {
                        Image = bitmapImage,
                        Name = Path.GetFileNameWithoutExtension(file.DisplayName)
                    });

                }
                //if (counter > 3) break;
                // do something with the music files

            }

            flipView1.ItemsSource = datas;
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(1000);
            this.timer.Tick += this.OnTick;
            this.timer.Start();
        }
        private async void OnTick(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var today = DateTime.Now;
                //UI code here
                TxtJam.Text = today.ToString("HH:mm");
                TxtHari.Text = today.ToString("dddd");
                TxtTanggal.Text = today.ToString("MMMM, dd yyyy");
                counter++;
                if (counter % 2 == 0)
                {
                    // If we'd go out of bounds then reverse
                    int newIndex = flipView1.SelectedIndex + change;
                    if (newIndex >= flipView1.Items.Count || newIndex < 0)
                    {
                        change *= -1;
                    }

                    flipView1.SelectedIndex += change;
                }
            });
        }
        private async void Menu_Click(object sender, RoutedEventArgs e)
        {
            if(this.timer !=null )this.timer.Stop();
            switch ((sender as Button).Tag)
            {
                case "Youtube":
                    this.Frame.Navigate(typeof(YoutubePage));
                    break;
                case "Radio":
                    this.Frame.Navigate(typeof(Radio));
                    break;
                case "Home":
                    this.Frame.Navigate(typeof(HomeAuto));
                    break;
                case "Media":
                    this.Frame.Navigate(typeof(PlayerPage));
                    break;
                case "Maps":
                    this.Frame.Navigate(typeof(Maps));
                    break;
                case "Quran":
                    this.Frame.Navigate(typeof(QuranPage));
                    break;
                case "Twitter":
                    this.Frame.Navigate(typeof(TwitterPage));
                    break;
                case "Car":
                    // Create the message dialog and set its content
                    var messageDialog = new MessageDialog("You need to put OBD Reader device.");

                    // Show the message dialog
                    await messageDialog.ShowAsync();
                    break;
            }
        }
    }

    public class ImageItem
    {
        public string Name { get; set; }
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
