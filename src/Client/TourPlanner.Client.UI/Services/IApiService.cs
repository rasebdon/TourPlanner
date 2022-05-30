using System.Net;
using System.Threading.Tasks;

namespace TourPlanner.Client.UI.Services
{
    public interface IApiService
    {
        public Task<(string, HttpStatusCode)> GetStringAsync(string path);
        public Task<(byte[], HttpStatusCode)> GetBytesAsync(string path);

        public Task<(string, HttpStatusCode)> PostAsync(string path, object content);
        public Task<(string, HttpStatusCode)> PutAsync(string path, object content);
        public Task<HttpStatusCode> DeleteAsync(string path);
    }
}
