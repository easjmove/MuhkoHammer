using System.Text.Json.Serialization;

namespace MuhkoHammerApi.ModelClasses
{
    public class UnitImage
    {
        public int Id { get; set; }
        public DateTime UploadedDate { get; set; }
        [JsonIgnore]
        public byte[] Data { get; set; }
        public string Url { get
            {
                return "/api/Images/" + Id;
            } 
        }
        public string ThumbUrl
        {
            get
            {
                return "/api/Thumbnail/" + Id;
            }
        }
    }
}
