using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer
{
    /// <summary>
    /// Class that inherits from MediaFile and has one additional property "FileThumbnail" which is, in case of ImageFiles, similar to the file path.
    /// </summary>
    [Serializable]
    public class ImageFile : MediaFile
    {
        public ImageFile()
        {

        }
        public ImageFile(string fileName, string description, string filePath) : base(fileName, description, filePath)
        {
            FileThumbnail = filePath;
        }
    }
}
