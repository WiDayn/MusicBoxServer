namespace MusicBoxServer.Dtos
{
    public class SearchSong
    {
        public int SongID { get; set; }
        public string Title { get; set; }
        public int AlbumID { get; set; }

        public int ArtistID { get; set; }
        public TimeSpan Duration { get; set; }
        public string Genre { get; set; }
        public int BitRate { get; set; }
        public int ViewCount { get; set; }
        public int TrackNumber { get; set; }
        public string AlbumTitle { get; set; }
        public string ArtistName { get; set; }
        // 其他字段根据需要添加
    }
}
