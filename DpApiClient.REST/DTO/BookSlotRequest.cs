using RestSharp.Serializers;

namespace DpApiClient.REST.DTO
{
    public class BookSlotRequest
    {
        public string DoctorServiceId { get; set; }
        public bool IsReturning { get; set; }
        public Patient Patient { get; set; }
    }
}