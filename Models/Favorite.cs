using MusicBoxServer.Dtos;

namespace MusicBoxServer.Models
{
    public class FavoriteInfo
    {
        public List<SongInfo> SongInfos { get; set; }
        public List<FavoriteItem> ArtistInfos { get; set; }
        public List<FavoriteItem> AlbumInfos { get; set; }
        public List<FavoriteItem> PlayListInfos { get; set; }

        public FavoriteInfo()
        {
            SongInfos = new List<SongInfo>();
            ArtistInfos = new List<FavoriteItem>();
            AlbumInfos = new List<FavoriteItem>();
            PlayListInfos = new List<FavoriteItem>();
        }
    }

    public class FavoriteRequest
    {
        public int UserID { get; set; }
        public int ID { get; set; }
        public DateTime DateFavorited { get; set; }
    }

    public class FavoriteItem
    {
        public int ID { get; set; }
        public DateTime DateFavorited { get; set; }
    }

    public class SongInfo
    {
        public int SongID { get; set; }
        public string Title { get; set; }
        public int AlbumID { get; set; }
        public string AlbumTitle { get; set; }
        public int ArtistID { get; set; }
        public string ArtistName { get; set; }
        public int Duration { get; set; }
        public string Genre { get; set; }
        public int Bitrate { get; set; }
        public int ViewCount { get; set; }
        public int TrackNumber { get; set; }
        public DateTime DateFavorited { get; set; }
    }
}
