namespace TourPlanner.Server.BL.MapQuestAPI
{
    public abstract class MapQuestBase
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _apiKey;

        protected MapQuestBase(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }
    }
}
