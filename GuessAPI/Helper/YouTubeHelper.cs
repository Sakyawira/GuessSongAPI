using Newtonsoft.Json;
using GuessAPI.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace GuessAPI.Helper
{
    class YouTubeHelper
    {
        //public static void TestProgram()
        //{
        //    Console.WriteLine(GetYouTubeIdFromUrl("=HELLO"));
        //    Console.ReadLine();
        //}

        // Get the Youtube video id from video's url
        public static String GetYouTubeIdFromUrl(String videoUrl) {

            // Get the string after the equal ('=') sign
            int indexOfFirstId = videoUrl.IndexOf("=", StringComparison.Ordinal) + 1;

            String youtubeId = videoUrl.Substring(indexOfFirstId);

            return youtubeId;
        }

        // Get video from video's youtube id
        public static Video GetVideoInfo(String youtubeId)
        {
            String apiKey = "AIzaSyBqwfIVpvsm_0sbGEGAfr08qivmYmKdXEQ";

            // create a youtube api link using the youtube video id and api key to get information of the video
            String YouTubeApiUrL = "https://www.googleapis.com/youtube/v3/videos?id=" + youtubeId + "&key=" + apiKey + "&part=snippet,contentDetails";

            // Grab the JSON string from the api link using an http client.
            String videoInfoJson = new WebClient().DownloadString(YouTubeApiUrL);

            // Using dynamic object helps us to more effciently extract infomation from a large JSON String.
            dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(videoInfoJson);

            // Extract information from the dynamic object.
            String title = jsonObj["items"][0]["snippet"]["title"];
            String thumbnailUrl = jsonObj["items"][0]["snippet"]["thumbnails"]["medium"]["url"];
            String durationString = jsonObj["items"][0]["contentDetails"]["duration"];
            String videoUrl = "https://www.youtube.com/watch?v=" + youtubeId;

            // duration is given in this format: PT4M17S, we need to use a simple parser to get the duration in seconds.
            TimeSpan videoDuration = XmlConvert.ToTimeSpan(durationString);
            int duration = (int)videoDuration.TotalSeconds;

            // Create a new Video Object from the model defined in the API.
            Video video = new Video
            {
                VideoTitle = title,
                WebUrl = videoUrl,
                VideoLength = duration,
                IsFavourite = false,
                ThumbnailUrl = thumbnailUrl
            };

            return video;
        }

        public static Boolean CanGetTranscriptions(String youtubeId)
        {
            if (GetTranscriptionLink(youtubeId) == null)
            {
                return false;
            }

            return true;
        }

        private static String GetTranscriptionLink(String youtubeId)
        {
            String youTubeVideoUrl = "https://www.youtube.com/watch?v=" + youtubeId;

            // Grab the JSON string from the api link using an http client.
            // Download the source code using a Web Client.
            String htmlSource = new WebClient().DownloadString(youTubeVideoUrl);

            // Use regular expression to find the link with the transcription
            String pattern = "timedtext.+?lang=";
            Match match = Regex.Match(htmlSource, pattern);
            if (match.ToString() != "")
            {
                String transcriptionLink = "https://www.youtube.com/api/" + match + "en";
                transcriptionLink = CleanLink(transcriptionLink);
                return transcriptionLink;
            }
            else
            {
                return null;
            }
        }

        // Get list of transcription from video id
        public static List<Transcription> GetTranscriptions(String videoId)
        {
            String transcriptionLink = GetTranscriptionLink(videoId);

            XmlDocument doc = new XmlDocument();


            // try to Use XmlDocument to load the transcription XML.
            try
            {
                doc.Load(transcriptionLink);
            }

            // load the US version of the transcription if fail
            catch
            {
                doc.Load(transcriptionLink + "-US");
            }
           
            XmlNode root = doc.ChildNodes[1];

            // Go through each tag and look for start time and phrase.
            List<Transcription> transcriptions = new List<Transcription>();
            if (root.HasChildNodes)
            {
                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    // Decode HTTP characters to text
                    // e.g. &#39; -> '
                    String phrase = root.ChildNodes[i].InnerText;
                    phrase = HttpUtility.HtmlDecode(phrase);

                    Transcription transcription = new Transcription
                    {
                        StartTime = (int)Convert.ToDouble(root.ChildNodes[i].Attributes["start"].Value),
                        Phrase = phrase
                    };

                    transcriptions.Add(transcription);
                }
            }
            return transcriptions;
        }
        
        private static String CleanLink(String transcriptionLink)
        {
            transcriptionLink = transcriptionLink.Replace("\\\\u0026", "&");
            transcriptionLink = transcriptionLink.Replace("\\", "");
            return (transcriptionLink);
        }

      
    }
}