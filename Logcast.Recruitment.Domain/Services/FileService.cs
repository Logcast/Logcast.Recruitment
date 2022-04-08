using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Repositories;
using Logcast.Recruitment.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace Logcast.Recruitment.Domain.Services
{
    public interface IFileService
    {
        Task<Tuple<int, string, string>> AddFileAsync(IFormFile audioFile);
        Task<FileModel> GetFileAsync(int fileId);
        Task<FileModel> ExtractMetadata(Tuple<int, string, string> fileData);
        Task<List<FileModel>> GetFilesAsync();
        Task<byte[]> GetAudioStream(int fileId);
        Task UpdateMetadata(int fileId, string artist, string album, string trackTitle, string genre, string trackNumber);
    }

    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly string _contentPath;

        public FileService(IFileRepository fileRepository)
        {
            _contentPath = AppDomain.CurrentDomain.BaseDirectory;
            _fileRepository = fileRepository;
        }

        public async Task<Tuple<int, string, string>> AddFileAsync(IFormFile audioFile)
        {
            string path = $"{_contentPath}\\TempFiles\\";
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }            

            string extension = Path.GetExtension(audioFile.FileName);

            string pattern = @"^.*\.(mp3|opus|flac|webm|weba|wav|ogg|m4a|mp3|oga|mid|amr|aiff|wma|au|aac)$";
            var regex = new Regex(pattern);

            if (!regex.IsMatch(extension.ToLower()))
            {
                throw new ArgumentException("Not possible to save not an audio file");
            }

            string fileName = $"{Guid.NewGuid()}_{DateTime.Now.ToString("MM.dd.yy_HH.mm.ss")}{extension}";

            Task result = CopyDocumentAsync(audioFile, $"{path}{fileName}");
            result.Wait();

            if (result.IsFaulted)
            {
                throw new IOException("Not possible to save the file");
            }

            return await _fileRepository.AddFileAsync(audioFile.FileName, $"{path}{fileName}");
        }

        static async Task CopyDocumentAsync(IFormFile audioFile, string pathToFile)
        {
            using (var fileStream = new FileStream(pathToFile, FileMode.Create))
            {
                await audioFile.CopyToAsync(fileStream);
            }
        }

        public async Task<FileModel> GetFileAsync(int fileId)
        {
            var file = await _fileRepository.GetFileAsync(fileId);
            return file.ToDomainModel();
        }

        public async Task<byte[]> GetAudioStream(int fileId)
        {
            var f = await _fileRepository.GetFileAsync(fileId);
            byte[] bytes;

            using (FileStream file = new FileStream(f.Path, FileMode.Open, System.IO.FileAccess.Read))
            {
                bytes = new byte[file.Length];
                file.Read(bytes, 0, bytes.Length);
            }

            return bytes;
        }

        public async Task<List<FileModel>> GetFilesAsync()
        {
            var files = await _fileRepository.GetFilesAsync();
            return files.Select(x => x.ToDomainModel()).ToList();
        }

        public async Task<FileModel> ExtractMetadata(Tuple<int, string, string> fileData)
        {
            var file = await _fileRepository.GetFileAsync(fileData.Item1);
            var tfile = TagLib.File.Create(file.Path);

            if (!string.IsNullOrEmpty(tfile.Tag.FirstPerformer))
            {
                file.Artist = tfile.Tag.FirstPerformer;
            }

            if (!string.IsNullOrEmpty(tfile.Tag.Album))
            {
                file.Album = tfile.Tag.Album;
            }

            if (!string.IsNullOrEmpty(tfile.Tag.Title))
            {
                file.TrackTitle = tfile.Tag.Title;
            }            

            if (!string.IsNullOrEmpty(tfile.Tag.FirstGenre))
            {
                file.Genre = tfile.Tag.FirstGenre;
            }

            file.Bitrate = tfile.Properties.AudioBitrate;

            file.Duration = tfile.Properties.Duration.TotalSeconds;

            file.Size = tfile.InvariantEndPosition;

            file.Type = tfile.MimeType.Split("/").Last();

            _fileRepository.UpdateFileDetailsAsync(file);

            return file.ToDomainModel();
        }

        public async Task UpdateMetadata(int fileId, string artist, string album, string trackTitle, string genre, string trackNumber)
        {
            var file = await _fileRepository.GetFileAsync(fileId);
            file.Artist = artist;
            file.Album = album;
            file.TrackTitle = trackTitle;
            file.Genre = genre;
            file.TrackNumber = trackNumber;

            _fileRepository.UpdateFileDetailsAsync(file);
        }
    }
}