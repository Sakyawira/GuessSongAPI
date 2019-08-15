using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GuessAPI.Model;

namespace GuessAPI.DAL
{
    public class VideoRepository : IVideoRepository, IDisposable
    {
        private GuessContext _context;

        public VideoRepository(GuessContext context)
        {
            this._context = context;
        }

        public IEnumerable<Video> GetVideos()
        {
            return _context.Video.ToList();
        }

        public Video GetVideoById(int id)
        {
            return _context.Video.Find(id);
        }

        public void InsertVideo(Video video)
        {
            _context.Video.Add(video);
        }

        public void DeleteVideo(int videoId)
        {
            Video video = _context.Video.Find(videoId);
            _context.Video.Remove(video);
        }

        public void UpdateVideo(Video video)
        {
            _context.Entry(video).State = EntityState.Modified;
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
