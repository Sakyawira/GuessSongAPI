using System;
using System.Collections.Generic;
using GuessAPI.Model;

namespace GuessAPI.DAL
{
    public interface IVideoRepository : IDisposable
    {
        IEnumerable<Video> GetVideos();
        Video GetVideoById(int videoId);
        void InsertVideo(Video video);
        void DeleteVideo(int videoId);
        void UpdateVideo(Video video);
        void Save();
    }
}
