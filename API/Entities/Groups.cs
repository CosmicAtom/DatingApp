using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Groups
    {
        public Groups()
        {
        }

        public Groups(string name)
        {
            Name = name;
        }

        [Key]
        public string Name { get; set; }

        public ICollection<Connections> Connections { get; set; } = new List<Connections>();
    }
}
