using System.Text.Json.Serialization;

namespace MuhkoHammer.ModelClasses
{
    public class UnitImage
    {
        public int Id { get; set; }
        public DateTime UploadedDate { get; set; }
        [JsonIgnore]
        public byte[] Data { get; set; }
    }
}
