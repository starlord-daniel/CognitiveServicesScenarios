using ImageDescription.Model;
using ImageDescription.Objects;
using ImageDescription.TextToSpeech;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace ImageDescription
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Select the desired visual features here. We just use the descriptions. See  
        VisualFeature[] visualFeatures = new VisualFeature[] { VisualFeature.Description };

        public MainPage()
        {
            this.InitializeComponent();
        }

        #region Web Search

        private void WebSearch_Click(object sender, RoutedEventArgs e)
        {
            WebImageSearch();

            AnalysisResultLabel.Text = "Image Analysis Result";
        }

        private void UrlInput_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                WebImageSearch();

                AnalysisResultLabel.Text = "Image Analysis Result";
            }
        }

        private async void WebImageSearch()
        {
            try
            {
                var client = new HttpClient();
                var input = UrlInput.Text != "" ? UrlInput.Text : "cat";
                var queryString = $"q={input}";

                // Request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Credentials.WEBSEARCH_KEY);

                // Request parameters
                var uri = "https://api.cognitive.microsoft.com/bing/v5.0/images/search?" + queryString;

                var response = await client.GetStringAsync(uri);
                var results = JsonConvert.DeserializeObject<WebImageSearchResult>(response);

                var imageList = new List<Image> { Image1, Image2, Image3, Image4 };

                Random r = new Random();

                foreach (var image in imageList)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.UriSource = new Uri(results.value[r.Next(0, results.value.Length)].contentUrl);

                    image.Source = bitmapImage;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        #endregion

        #region Image Analysis

        private async void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AnalysisResultLabel.Text = "Thinking...";

            Image image = (Image)sender;

            await GetAnalysisResult(((BitmapImage)image.Source).UriSource.AbsoluteUri);
        }

        private async Task GetAnalysisResult(string imageUrl)
        {
            AnalysisResult analysisResult;
            analysisResult = await AnalyzeInDomainUrl(imageUrl, visualFeatures);

            AnalysisResultLabel.Text = analysisResult.Description.Captions[0].Text;

            try
            {
                CreateAudioOutput(analysisResult.Description.Captions[0].Text);
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        private async Task<AnalysisResult> AnalyzeInDomainUrl(string imageUrl, VisualFeature[] domainModel)
        {
            VisionServiceClient VisionServiceClient = new VisionServiceClient(Credentials.VISION_KEY);
            AnalysisResult analysisResult = await VisionServiceClient.AnalyzeImageAsync(imageUrl, domainModel);
            return analysisResult;
        }

        #endregion

        #region Audio

        private async void CreateAudioOutput(string textToSpeak)
        {
            SpeechManager speech = new SpeechManager();
            Voice voice = new Voice(VoiceGender.Female, VoiceLanguage.English);
            speech.Voice = voice;

            var audio = await speech.TextToSpeechAsync(textToSpeak);

            var mediaSource = Windows.Media.Core.MediaSource.CreateFromStream(audio.AsRandomAccessStream(), "audio/mp3");
            AudioPlayer.SetPlaybackSource(mediaSource);

        }

        #endregion


    }
}
