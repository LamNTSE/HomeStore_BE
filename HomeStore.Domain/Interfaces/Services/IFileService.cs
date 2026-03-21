using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HomeStore.Domain.Interfaces.Services
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}