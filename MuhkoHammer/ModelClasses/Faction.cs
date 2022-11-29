using System.Text.Json.Serialization;

namespace MuhkoHammer.ModelClasses
{
    public class Faction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public List<Unit> Units { get; set; }

        public Faction()
        {
            Units = new List<Unit>();
        }
    }
}
