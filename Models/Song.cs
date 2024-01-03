namespace MusicBoxServer.Models
{
    public class Song
    {
        public int SongID { get; set; }
        public string Title { get; set; }
        public int AlbumID { get; set; }
        public TimeSpan Duration { get; set; }
        public string FilePath { get; set; }
        public string Genre { get; set; }
        public int BitRate { get; set; }
        public int ViewCount { get; set; }
    }
}
