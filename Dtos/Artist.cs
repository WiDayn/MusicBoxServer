using MusicBoxServer.Models;

namespace MusicBoxServer.Dtos
{
    public class ArtistDetails
    {
        public Artist Artist { get; set; }
        public List<Album> Albums { get; set; }
        public List<Song> TopSongs { get; set; }

        public ArtistDetails()
        {
            Albums = new List<Album>();
            TopSongs = new List<Song>();
        }
    }
}
