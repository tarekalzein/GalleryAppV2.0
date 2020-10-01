using BusinessLayer;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataAccess
{
    /// <summary>
    /// Static class that takes care of serialization/deserialization of data to save/retrieve it from data.bin file
    /// </summary>
    public static class SerializationHelper
    {
        private static string targetFile = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + "/data.bin";

        /// <summary>
        /// Serializer Method that takes an instance of album manager and serialize its content to a file data.bin
        /// </summary>
        /// <param name="albumManager">Instance of AlbumManager that contains the data to save</param>
        /// <returns>returns bool (true on success)</returns>
        public static bool Serialize(AlbumManager albumManager)
        {
            FileStream fileStream = null;
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                fileStream = new FileStream(targetFile, FileMode.Create);
                binaryFormatter.Serialize(fileStream, albumManager);
            }
            catch
            {
                return false;
            }
            finally
            {
                if(fileStream!=null)
                {
                    fileStream.Close();
                }
            }
            return true;
        }
        /// <summary>
        /// Deserializer method that reads the data.bin file in root (if exists) and retrieve its data.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>instance of retrieved ablum manager</returns>
        public static AlbumManager Deserialize(out string errorMessage)
        {
            FileStream fileStream = null;
            errorMessage = null;
            AlbumManager albumManager = new AlbumManager();

            try
            {
                if (File.Exists(targetFile))
                {
                    fileStream = new FileStream(targetFile, FileMode.Open);
                    BinaryFormatter b = new BinaryFormatter();
                    if(new FileInfo(targetFile).Length>0)
                    {
                        albumManager = (AlbumManager)b.Deserialize(fileStream);
                    }
                }
            }
            catch(SerializationException e)
            {
                errorMessage = "Error loading saved data .\n" + e.Message;
            }
            finally
            {
                if(fileStream!=null)
                {
                    fileStream.Close();
                }
            }
            return albumManager;
        }
    }
}
