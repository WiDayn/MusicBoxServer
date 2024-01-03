using MusicBoxServer.Models;

namespace MusicBoxServer.Dtos
{
    public class AlbumDetails
    {
        public Album Album { get; set; }
        public List<Song> Songs { get; set; }
        public string ArtistName { get; set; }

        public AlbumDetails()
        {
            Songs = new List<Song>();
        }
    }
}
