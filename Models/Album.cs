namespace MusicBoxServer.Models
{
    public class Album
    {
        public int AlbumID { get; set; }
        public string Title { get; set; }
        public int ArtistID { get; set; }
        public string Bio { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string CoverImagePath { get; set; }
        public string Distributor { get; set; }
    }

}
