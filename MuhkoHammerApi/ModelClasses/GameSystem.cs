using System.Text.Json.Serialization;

namespace MuhkoHammerApi.ModelClasses
{
    public class GameSystem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public List<Faction> Factions { get; set; }

        public GameSystem()
        {
            Factions = new List<Faction>();
        }

        public Faction GetFaction(int factionID)
        {
            return Factions.Find(faction => faction.Id == factionID);
        }
    }
}
