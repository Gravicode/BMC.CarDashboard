using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BMC.CarDashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TwitterPage : Page
    {
        SpeechHelper speech;
        TwitterService TwitService;
        ObservableCollection<TweetData> Datas;
        public TwitterPage()
        {
            this.InitializeComponent();
            Setup();
        }

        async void Setup()
        {
            List1.IsItemClickEnabled = true;
            TwitService = new TwitterService();
            BtnExit.Click += (a, b) => { if (this.Frame.CanGoBack) this.Frame.GoBack(); else this.Frame.Navigate(typeof(MainPage)); };
            List1.ItemClick += async (a, b) =>
            {
                var item = b.ClickedItem as TweetData;
                if (item != null)
                {
                    await speech.Read(item.Tweet);
                }
            };
            BtnFind.Click += async(a, b) =>
            {
               
                var Keyword = TxtSearch.Text;

                await GetTwitterData(Keyword);

                
            };
            var keys = TxtSearch.Text;
            await GetTwitterData(keys);

        }
        /// <summary>
        /// Triggered when media element used to play synthesized speech messages is loaded.
        /// Initializes SpeechHelper and greets user.
        /// </summary>
        private void speechMediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (speech == null)
            {
                speech = new SpeechHelper(speechMediaElement);
            }
            else
            {
                // Prevents media element from re-greeting visitor
                speechMediaElement.AutoPlay = false;
            }
        }

        async Task GetTwitterData(string Keyword)
        {
            try
            {
                Progress1.Visibility = Visibility.Visible;
                if (Datas == null)
                {
                    Datas = new ObservableCollection<TweetData>();
                }
                else
                {
                    Datas.Clear();
                }
                var tweets = TwitService.GetTwittByHashtag(Keyword);
                foreach (var x in tweets)
                {
                    var y = await SentimentService.GetSentiment(x.Text);
                    if (y != null)
                    {
                        var sentiment = (y.documents[0].score < 0.5 ? "negative" : "positive");
                        Datas.Add(new TweetData() { Sentiment = sentiment, Tweet = x.Text, Created = x.CreatedAt, UrlEMbed = x.Url });
                        //await speech.Read(speaknow);
                    }
                }
                List1.ItemsSource = Datas;
                Progress1.Visibility = Visibility.Collapsed;
            }catch(Exception ex)
            {
                // Create the message dialog and set its content
                var messageDialog = new MessageDialog("Error Found:"+ex.Message);

                // Show the message dialog
                await messageDialog.ShowAsync();
            }
        }
    }
    public class TweetData
    {
        public string UrlEMbed { get; set; }
        public DateTime Created { get; set; }
        public string Sentiment { get; set; }
        public string Tweet { get; set; }

    }
}
