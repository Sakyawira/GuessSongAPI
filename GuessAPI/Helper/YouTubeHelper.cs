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

            // Create a youtube api link using the youtube video id and api key to get information of the video
            String YouTubeApiUrL = "https://www.googleapis.com/youtube/v3/videos?id=" + youtubeId + "&key=" + apiKey + "&part=snippet,contentDetails";

            // Grab the JSON string from the api link using an http client.
            String videoInfoJson = new WebClient().DownloadString(YouTubeApiUrL);

            // Extract information from the JSON String using dynamic object.
            dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(videoInfoJson);

            // Extract information from the dynamic object.
            String title = jsonObj["items"][0]["snippet"]["title"];
            String thumbnailUrl = jsonObj["items"][0]["snippet"]["thumbnails"]["medium"]["url"];
            String durationString = jsonObj["items"][0]["contentDetails"]["duration"];
            String videoUrl = "https://www.youtube.com/watch?v=" + youtubeId;

            // Get the duration of video using a parser 
            TimeSpan videoDuration = XmlConvert.ToTimeSpan(durationString);
            int duration = (int)videoDuration.TotalSeconds;

            // Create a new Video Object using the API's Video model definition.
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

        // Get list of transcription from video id
        public static List<Transcription> GetTranscriptions(String videoId)
        {
            String transcriptionLink = GetTranscriptionLink(videoId);

            XmlDocument doc = new XmlDocument();


            // Try to Use XmlDocument to load the transcription XML.
            try
            {
                doc.Load(transcriptionLink);
            }

            // Load the US version of the transcription if fail
            catch
            {
                doc.Load(transcriptionLink + "-US");
            }
           
            // Set the root node to the first Child Node
            XmlNode root = doc.ChildNodes[1];

            // Go through each tag and look for start time and phrase.
            List<Transcription> transcriptions = new List<Transcription>();
            if (root.HasChildNodes)
            {
                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    // Decode HTTP characters to text using HTMLDecode
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

        // Get the Transcription Link of a Youtube Video using its Youtube Video ID
        private static String GetTranscriptionLink(String youtubeId)
        {
            String youTubeVideoUrl = "https://www.youtube.com/watch?v=" + youtubeId;

            // Download the source code of the youtube video using a Web Client.
            String htmlSource = new WebClient().DownloadString(youTubeVideoUrl);

            // Find strings with the link to the transcription
            // by matching a string pattern
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

        // Clean the Transcription Link from unreadable characters
        private static String CleanLink(String transcriptionLink)
        {
            transcriptionLink = transcriptionLink.Replace("\\\\u0026", "&");
            transcriptionLink = transcriptionLink.Replace("\\", "");
            return (transcriptionLink);
        }

      
    }
}