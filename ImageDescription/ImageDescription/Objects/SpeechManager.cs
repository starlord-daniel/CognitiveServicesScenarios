using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Windows.Storage.Streams;
using System.Net.Http;
using ImageDescription.Objects;

namespace ImageDescription.TextToSpeech
{
    class SpeechManager
    {
        public Voice Voice { get; internal set; }

        private string _token;

        internal async Task<Stream> TextToSpeechAsync(string textToSpeak)
        {
            // Get token for Bing Speech
            _token = await RequestSpeechToken();

            // Get audio response
            var audioResponse = await GetSpeechFromText(textToSpeak);
            return audioResponse;
        }

        private async Task<Stream> GetSpeechFromText(string textToSpeak)
        {
            var requestUri = "https://speech.platform.bing.com/synthesize";
            Dictionary<string, string> data = GetRequestXML(textToSpeak);

            Dictionary<string, string> inputOptions = new Dictionary<string, string>
            {
                ["X-Microsoft-OutputFormat"] = "audio-16khz-128kbitrate-mono-mp3",
                ["Authorization"] = "Bearer " + _token,
                ["X-Search-AppId"] = "07D3234E49CE426DAA29772419F436CA",
                ["X-Search-ClientID"] = "1ECFAE91408841A480F00935DC390960",
                ["User-Agent"] = "TTSClient"
            };

            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);

            foreach (var key in inputOptions.Keys)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(key, inputOptions[key]);
            }

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(data["xml"])
            };

            var httpTask = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            Stream httpStream = await httpTask.Content.ReadAsStreamAsync();

            return httpStream;
        }

        private Dictionary<string, string> GetRequestXML(string textToSpeak)
        {
            string languageCode;
            string gender;
            string voiceName;

            switch (Voice.voiceLanguage)
            {
                case VoiceLanguage.English:
                    {
                        languageCode = "en-US";
                        switch (Voice.voiceGender)
                        {
                            case VoiceGender.Female:
                                gender = "Female";
                                voiceName = "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)";
                                break;
                            case VoiceGender.Male:
                                gender = "Male";
                                voiceName = "Microsoft Server Speech Text to Speech Voice (en-US, BenjaminRUS)";
                                break;
                            default:
                                gender = "Female";
                                voiceName = "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)";
                                break;
                        }
                    }
                    break;
                case VoiceLanguage.German:
                    languageCode = "de-DE";
                    switch (Voice.voiceGender)
                    {
                        case VoiceGender.Female:
                            gender = "Female";
                            voiceName = "Microsoft Server Speech Text to Speech Voice (de-DE, Hedda)";
                            break;
                        case VoiceGender.Male:
                            gender = "Male";
                            voiceName = "Microsoft Server Speech Text to Speech Voice (de-DE, Stefan, Apollo)";
                            break;
                        default:
                            gender = "Female";
                            voiceName = "Microsoft Server Speech Text to Speech Voice (de-DE, Hedda)";
                            break;
                    }
                    break;
                default:
                    languageCode = "en-US";
                    gender = "Female";
                    voiceName = "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)";
                    break;
            }

            var xml = $"<speak version='1.0' xml:lang='{languageCode}'><voice xml:lang='{languageCode}' xml:gender='{gender}' name='{voiceName}'>{textToSpeak}</voice></speak>";
            return new Dictionary<string, string>
            {
                ["gender"] = gender,
                ["language"] = languageCode,
                ["voiceName"] = voiceName,
                ["xml"] = xml
            };
        }

        private async Task<string> RequestSpeechToken()
        {
            WebRequest request = WebRequest.Create("https://api.cognitive.microsoft.com/sts/v1.0/issueToken");

            request.Method = "POST";

            request.Headers = new WebHeaderCollection
            {
                ["Ocp-Apim-Subscription-Key"] = Credentials.SPEECH_KEY
            };

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string token = reader.ReadToEnd();

                // Cleanup the streams and the response.
                reader.Dispose();
                dataStream.Dispose();
                response.Dispose();

                return token;
            }
            else
            {
                return "-1";
            }

            
        }
    }
}
