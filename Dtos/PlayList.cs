using MusicBoxServer.Models;

namespace MusicBoxServer.Dtos
{
    public class PlayListDetails
    {
        public PlayList PlayList { get; set; }
        public List<Song> Songs { get; set; }
        public string Username { get; set; }
    }
}
