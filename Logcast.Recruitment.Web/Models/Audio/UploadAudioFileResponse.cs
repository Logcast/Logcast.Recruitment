using Logcast.Recruitment.Shared.Models;

namespace Logcast.Recruitment.Web.Models.Audio
{
	public class UploadAudioFileResponse
	{
        public UploadAudioFileResponse(int fileId, string name, string path, string artist, string album, string trackTitle, string genre, string trackNumber)
        {
            Id = fileId;
            Name = name;
            Path = path;
            Artist = artist;
            Album = album;
            TrackTitle = trackTitle;
            Genre = genre;
            TrackNumber = trackNumber;
        }

        public UploadAudioFileResponse(FileModel fileModel)
        {
            Id = fileModel.Id;
            Name = fileModel.Name;
            Path = fileModel.Path;
            Artist = fileModel.Artist;
            Album = fileModel.Album;
            TrackTitle = fileModel.TrackTitle;
            Genre = fileModel.Genre;
            TrackNumber = fileModel.TrackNumber;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string TrackTitle { get; set; }
        public string Genre { get; set; }
        public string TrackNumber { get; set; }
    }
}
