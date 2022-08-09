namespace MSA2022.Phase2.Backend.Models
{
    public class CataasApiResponse
    {
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public string[] tags { get; set; }
        public string url { get; set; }
    }
}
