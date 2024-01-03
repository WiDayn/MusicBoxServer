namespace MusicBoxServer.Models
{
    public class Artist
    {
        public int ArtistID { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string INSLink { get; set; }
        public string TwitterLink { get; set; }
        public string FacebookLink { get; set; }
        public int ListenerNum { get; set; }
    }
}
