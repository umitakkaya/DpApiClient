using DpApiClient.REST.DTO;
using DpApiClient.REST.Utilities;
using RestSharp;
using RestSharp.Extensions.MonoHttp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DpApiClient.REST.Extensions;

namespace DpApiClient.REST.Client
{
    /// <summary>
    /// For explanation of endpoints please visit: http://znanylekarz.github.io
    /// </summary>
    public class DpApi
    {
        private const string TokenEndpoint = "oauth/v2/token";
        private const string Prefix = "api/v2/integration";

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string GrantType { get; set; } = "client_credentials";
        public string Scope { get; set; } = "integration";

        private RestClient _client;

        private Locale _locale;

        /// <summary>
        /// Initialize Docplanner REST Client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="locale"></param>
        public DpApi(string clientId, string clientSecret, Locale locale)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;

            _client = new RestClient(locale.ToString());
            _locale = locale;

            SetSerializerStrategy();
            GetToken();
        }


        /// <summary>
        /// Get all service list 
        /// Returned service list can be used to add DoctorService to a doctor.
        /// </summary>
        /// <returns></returns>
        /// <see cref="DoctorService"/>
        public List<Service> GetServices()
        {
            var request = new RestRequest($"{Prefix}/services", Method.GET);

            var response = _client.Execute<DPCollection<Service>>(request);

            return response.Data.Items;
        }

        public Notification GetNotification()
        {
            var request = new RestRequest($"{Prefix}/notifications", Method.GET);

            var response = _client.Execute<Notification>(request);

            return response.Data;
        }

        /// <summary>
        /// Get all facilities that you have access
        /// </summary>
        /// <returns></returns>
        public List<DPFacility> GetFacilities()
        {
            var request = new RestRequest($"{Prefix}/facilities", Method.GET);

            var response = _client.Execute<DPCollection<DPFacility>>(request);

            return response.Data.Items;
        }

        /// <summary>
        /// Get a specific facility
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public DPFacility GetFacility(int facilityId)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}", Method.GET);

            var response = _client.Execute<DPFacility>(request);

            return response.Data;
        }

        /// <summary>
        /// Get all doctors in a facility
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public List<DPDoctor> GetDoctors(string facilityId, bool shouldIncludeSpecializations = false)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors", Method.GET);

            if (shouldIncludeSpecializations)
            {
                request.AddParameter("with", "doctor.specializations", ParameterType.QueryString);
            }

            var response = _client.Execute<DPCollection<DPDoctor>>(request);

            //remove duplicates
            //When two address for the same facility assigned to the same doctor, duplication happens.
            var items = response.Data.Items.GroupBy(d => d.Id).Select(d => d.First()).ToList();

            return items;
        }

        /// <summary>
        /// Get a specific doctor
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        public DPDoctor GetDoctor(string facilityId, string doctorId)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors/{doctorId}", Method.GET);

            var response = _client.Execute<DPDoctor>(request);

            return response.Data;
        }

        /// <summary>
        /// Get all addresses of a doctor
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        public List<Address> GetAddresses(string facilityId, string doctorId)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors/{doctorId}/addresses", Method.GET);

            var response = _client.Execute<DPCollection<Address>>(request);

            return response.Data.Items;
        }

        /// <summary>
        /// Get all addresses of a doctor
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        public Address GetAddress(string facilityId, string doctorId, string addressId)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors/{doctorId}/addresses/{addressId}", Method.GET);

            var response = _client.Execute<Address>(request);

            return response.Data;
        }

        /// <summary>
        /// Get all DoctorServices of a doctor
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        public List<DoctorService> GetDoctorServices(string facilityId, string doctorId)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors/{doctorId}/services", Method.GET);

            var response = _client.Execute<DPCollection<DoctorService>>(request);

            return response.Data.Items;
        }

        /// <summary>
        /// Get DoctorServices for a specific slot on a specific address of a doctor
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="doctorId"></param>
        /// <param name="addressId"></param>
        /// <param name="start">DateTime of the slot</param>
        /// <returns></returns>
        public List<DoctorService> GetDoctorServices(string facilityId, string doctorId, string addressId, DateTime start)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors/{doctorId}/services", Method.GET);


            request.AddQueryParameter("address_id", addressId);
            request.AddQueryParameter("start", EncodeUniversalString(start, false));

            var response = _client.Execute<DPCollection<DoctorService>>(request);

            return response.Data.Items;
        }

        /// <summary>
        /// Delete a doctor service
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="doctorId"></param>
        /// <param name="doctorServiceId"></param>
        /// <returns>true if deletion is successfull</returns>
        public bool DeleteDoctorService(string facilityId, string doctorId, int doctorServiceId)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors/{doctorId}/services/{doctorServiceId}", Method.DELETE);

            var response = _client.Execute(request);

            return response.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Create new doctor service, possible services can be found at "/services" endpoint
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="doctorId"></param>
        /// <param name="serviceId"></param>
        /// <param name="priceMin"></param>
        /// <param name="priceMax"></param>
        /// <returns></returns>
        public DoctorService CreateDoctorService(string facilityId, string doctorId, string serviceId, double? priceMin, double? priceMax)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors/{doctorId}/services", Method.POST);

            request.AddParameter("service_id", serviceId, ParameterType.RequestBody);
            request.AddParameter("price_min", priceMin, ParameterType.RequestBody);
            request.AddParameter("price_max", priceMax, ParameterType.RequestBody);

            var response = _client.Execute<DoctorService>(request);

            return response.Data;
        }

        /// <summary>
        /// Update prices of a DoctorService
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="doctorId"></param>
        /// <param name="doctorServiceId"></param>
        /// <param name="priceMin"></param>
        /// <param name="priceMax"></param>
        /// <returns></returns>
        public DoctorService UpdateDoctorService(string facilityId, string doctorId, string doctorServiceId, double? priceMin, double? priceMax)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors/{doctorId}/services/{doctorServiceId}", Method.PATCH);

            request.AddParameter("price_min", priceMin, ParameterType.RequestBody);
            request.AddParameter("price_max", priceMax, ParameterType.RequestBody);

            var response = _client.Execute<DoctorService>(request);

            return response.Data;
        }

        /// <summary>
        /// Get free slots on a specific address for a doctor between start and end dates
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="doctorId"></param>
        /// <param name="addressId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Slot> GetSlots(string facilityId, string doctorId, string addressId, DateTime start, DateTime end)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors/{doctorId}/addresses/{addressId}/slots", Method.GET);

            request.AddParameter("start", EncodeUniversalString(start, false), ParameterType.QueryString);
            request.AddParameter("end", EncodeUniversalString(end, false), ParameterType.QueryString);

            var response = _client.Execute<DPCollection<Slot>>(request);



            return response.Data.Items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="doctorId"></param>
        /// <param name="addressId"></param>
        /// <param name="start"></param>
        /// <param name="bookSlotRequest"></param>
        /// <returns></returns>
        public Booking BookSlot(string facilityId, string doctorId, string addressId, DateTime start, BookSlotRequest bookSlotRequest)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors/{doctorId}/addresses/{addressId}/slots/{EncodeUniversalString(start)}/book", Method.POST);
            request.AddJsonBody(bookSlotRequest);


            var response = _client.Execute<Booking>(request);

            return response.Data;
        }

        /// <summary>
        /// Override schedules for the specified dates
        /// For detailed explanation visit: http://znanylekarz.github.io/integrations-api-docs/master/#slots-replace-slots
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="doctorId"></param>
        /// <param name="addressId"></param>
        /// <param name="putSlotsRequest"></param>
        /// <returns></returns>
        public bool PutSlots(string facilityId, string doctorId, string addressId, PutSlotsRequest putSlotsRequest)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors/{doctorId}/addresses/{addressId}/slots", Method.PUT);

            request.AddJsonBody(putSlotsRequest);

            var response = _client.Execute<DPResponse>(request);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return true;
            }

            return false;

        }

        /// <summary>
        /// Deletes slots in a single day
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="doctorId"></param>
        /// <param name="addressId"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public bool DeleteSlots(string facilityId, string doctorId, string addressId, DateTime start)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors/{doctorId}/addresses/{addressId}/slots/{start.ToString("yyyy-MM-dd")}", Method.DELETE);

            var response = _client.Execute(request);

            return response.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Get all bookings that has been made
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="doctorId"></param>
        /// <param name="addressId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Booking> GetBookings(string facilityId, string doctorId, string addressId, DateTime start, DateTime end)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors/{doctorId}/addresses/{addressId}/bookings", Method.GET);

            request.AddParameter("start", EncodeUniversalString(start, false), ParameterType.QueryString);
            request.AddParameter("end", EncodeUniversalString(end, false), ParameterType.QueryString);

            var response = _client.Execute<DPCollection<Booking>>(request);

            return response.Data.Items;
        }

        /// <summary>
        /// Cancels booking 
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="doctorId"></param>
        /// <param name="addressId"></param>
        /// <param name="bookingId"></param>
        /// <returns>true if the booking was canceled successfully</returns>
        public bool CancelBooking(string facilityId, string doctorId, string addressId, string bookingId)
        {
            var request = new RestRequest($"{Prefix}/facilities/{facilityId}/doctors/{doctorId}/addresses/{addressId}/bookings/{bookingId}", Method.DELETE);

            var response = _client.Execute(request);

            return response.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// URL Encode the datetime
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string EncodeUniversalString(DateTime dt, bool urlEncode = true)
        {
            string timeZone = ((TimeZones)_locale).ToString();

            if (urlEncode)
            {
                return HttpUtility.UrlEncode(dt.SetOffset(timeZone).ToString("yyyy-MM-ddTHH:mm:sszzz"));
            }
            else
            {
                return dt.SetOffset(timeZone).ToString("yyyy-MM-ddTHH:mm:sszzz");
            }
        }


        /// <summary>
        /// Tells the serializer to use snake_case when serializing JSON
        /// </summary>
        private void SetSerializerStrategy()
        {
            SimpleJson.CurrentJsonSerializerStrategy = new SnakeJsonSerializerStrategy();
        }

        /// <summary>
        /// Get access token
        /// </summary>
        /// <returns></returns>
        private AuthorizationToken GetToken()
        {
            var token = Globals.GetToken(ClientId);
            if (token == null || token.ExpiresAt <= DateTime.Now)
            {
                var request = new RestRequest(TokenEndpoint, Method.POST);

                request.AddParameter("client_id", ClientId);
                request.AddParameter("client_secret", ClientSecret);
                request.AddParameter("grant_type", GrantType);
                request.AddParameter("scope", Scope);

                var tokenResponse = _client.Post<AuthorizationToken>(request);

                token = tokenResponse.Data;
                token.ExpiresAt = DateTime.Now.AddSeconds(token.ExpiresIn);

                Globals.SetToken(ClientId, token);
            }

            _client.Authenticator = new DPAuthenticator(token);
            return token;
        }

        /// <summary>
        /// Adds default headers to the client
        /// </summary>
        private void AddDefaultHeaders()
        {
            _client.AddDefaultHeader("Content-Type", "application/json; charset=utf-8");
            _client.AddDefaultHeader("Accept", "application/json");
        }

    }
}
