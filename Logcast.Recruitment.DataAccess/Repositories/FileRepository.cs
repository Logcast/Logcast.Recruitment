using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Entities;
using Logcast.Recruitment.DataAccess.Exceptions;
using Logcast.Recruitment.DataAccess.Factories;
using Logcast.Recruitment.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Logcast.Recruitment.DataAccess.Repositories
{
    public interface IFileRepository
    {
        Task<Tuple<int, string, string>> AddFileAsync(string firstName, string email);
        void UpdateFileDetailsAsync(File file);
        Task<File> GetFileAsync(int fileId);
        Task<List<File>> GetFilesAsync();
    }

    public class FileRepository : IFileRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public FileRepository(IDbContextFactory dbContextFactory)
        {
            _applicationDbContext = dbContextFactory.Create();
        }

        public async Task<Tuple<int, string, string>> AddFileAsync(string name, string path)
        {
            var newFile = await _applicationDbContext.Files.AddAsync(new File(name, path));
            await _applicationDbContext.SaveChangesAsync();

            return Tuple.Create(newFile.Entity.Id, newFile.Entity.Name, newFile.Entity.Path);
        }

        public async Task<File> GetFileAsync(int fileId)
        {
            var file = await _applicationDbContext.Files.FirstOrDefaultAsync(s => s.Id == fileId);
            if (file == null) throw new FileNotFoundException();
            return file;
        }

        public async Task<List<File>> GetFilesAsync()
        {
            return await _applicationDbContext.Files.ToListAsync();
        }

        public void UpdateFileDetailsAsync(File file)
        {
            _applicationDbContext.Files.Update(file);
            _applicationDbContext.SaveChanges();
        }
    }
}
