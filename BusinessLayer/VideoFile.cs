using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer
{
    [Serializable]
    public class VideoFile : MediaFile
    {
        public VideoFile(string fileName, string description, string filePath) : base(fileName, description, filePath)
        {
            FileThumbnail = "Assets/video_icon.png";
            //Todo: Create a method to extract a thumbnail to this video and make its as FileThumbnail.
        }
    }
}
