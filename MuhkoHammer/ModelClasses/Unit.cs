using System.Text.Json.Serialization;

namespace MuhkoHammer.ModelClasses
{
    public class Unit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public List<UnitImage> Images { get; set; }

        public Unit()
        {
            Images = new List<UnitImage>();
        }
    }
}
