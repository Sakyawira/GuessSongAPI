using AutoMapper;

namespace GuessAPI.Model
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<Video, VideoDto>();
            CreateMap<VideoDto, Video>();
        }
    }
}
