namespace API.Entities
{
    public class Connections
    {
        public Connections()
        {
        }

        public Connections(string connectionId, string username)
        {
            ConnectionsId = connectionId;
            Username = username;
        }

        public string ConnectionsId { get; set; }
        public string Username { get; set; }
    }
}
