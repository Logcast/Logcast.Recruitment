using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Logcast.Recruitment.Shared.Models;

namespace Logcast.Recruitment.DataAccess.Entities
{
    public class MetaData
    {
        public MetaData()
        {
        }

        public MetaData(string title, string artist, string album, string albumArtists, string genre, int bitrate, TimeSpan duration, uint trackNumber, string filepath)
        {
            Title = title;
            Artist = artist;
            Album = album;
            AlbumArtists = albumArtists;
            Genre = genre;
            Bitrate = bitrate;
            Duration = duration;
            TrackNumber = trackNumber;
            FilePath = filepath;
        }

        [Key] public int Id { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string Artist { get; set; }

        [MaxLength(200)]
        public string Album { get; set; }

        [MaxLength(200)]
        public string AlbumArtists { get; set; }

        [MaxLength(200)]
        public string Genre { get; set; }

        public int Bitrate { get; set; }

        public TimeSpan Duration { get; set; }

        public uint TrackNumber { get; set; }
        public string FilePath { get; set; }



        public MetaDataModel ToDomainModel()
        {
            return new MetaDataModel()
            {
                Id = Id,
                Title = Title,
                Artist = Artist,
                Album = Album,
                AlbumArtists = AlbumArtists,
                Genre = Genre,
                Bitrate = Bitrate,
                Duration = Duration,
                TrackNumber = TrackNumber,
                FilePath = FilePath
            };
        }
    }
}