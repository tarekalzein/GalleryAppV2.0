using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BusinessLayer
{
    [Serializable]
    /// <summary>
    /// Class to represent a media file and its attributes.
    /// </summary>
    public class MediaFile
    {
        [Key]
        public int FileID { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string FileThumbnail { get; set; }
        public int Time { get; set; }
        public bool PlayEnabled { get; set; }
        public int AlbumID { get; set; }
        public MediaFile(string fileName, string description, string filePath)
        {
            FileName = fileName;
            Description = description;
            FilePath = filePath;
            PlayEnabled = true;
        }
        public MediaFile()
        {

        }
    }
}
