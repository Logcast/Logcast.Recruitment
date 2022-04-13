using System;
using System.IO;
using System.Threading.Tasks;

namespace Logcast.Recruitment.DataAccess.Services
{
    public interface IFileStorage
    {
        public Task<byte[]> GetAsync(string fileName);
        public Task SaveAsync(string fileName, byte[] content);
        public Task DeleteAsync(string fileName);
    }

    public class FileStorage : IFileStorage
    {
        private readonly string _contentFolder;

        public FileStorage()
        {
            _contentFolder = AppContext.BaseDirectory;
        }

        public async Task<byte[]> GetAsync(string fileName)
        {
            string path = Path.Combine(_contentFolder, fileName);
            return await File.ReadAllBytesAsync(path).ConfigureAwait(false);
        }

        public async Task SaveAsync(string fileName, byte[] content)
        {
            string path = Path.Combine(_contentFolder, fileName);
            await File.WriteAllBytesAsync(path, content).ConfigureAwait(false);
        }

        public Task DeleteAsync(string fileName)
        {
            string path = Path.Combine(_contentFolder, fileName);
            File.Delete(path);
            return Task.CompletedTask;
        }
    }
}
