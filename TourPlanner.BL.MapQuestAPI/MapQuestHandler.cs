namespace TourPlanner.BL.MapQuestAPI
{
    public class MapQuestHandler
    {
        private readonly HttpClient _httpClient;

        public MapQuestHandler()
        {
            _httpClient = new HttpClient();
        }

        public object? GetMap(List<object> routePoints)
        {
            return null;
        }
    }
}