namespace MusicBoxServer.Models
{
    public class Concert
    {
        public int ConcertID { get; set; }
        public int ArtistID { get; set; }
        public string Venue { get; set; }
        public DateTime Date { get; set; }
        public string TicketLink { get; set; }
    }
}
