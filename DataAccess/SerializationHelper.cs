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
        private static string DefaultTargetFile = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + "/data.bin";

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
                fileStream = new FileStream(DefaultTargetFile, FileMode.Create);
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
        /// Serializer Method that takes an instance of album manager and serialize its content to a file from SaveFileDialog
        /// </summary>
        /// <param name="albumManager"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool Serialize(AlbumManager albumManager,string filePath)
        {
            FileStream fileStream = null;
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                fileStream = new FileStream(filePath, FileMode.Create);
                binaryFormatter.Serialize(fileStream, albumManager);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (fileStream != null)
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
                if (File.Exists(DefaultTargetFile))
                {
                    fileStream = new FileStream(DefaultTargetFile, FileMode.Open);
                    BinaryFormatter b = new BinaryFormatter();
                    if(new FileInfo(DefaultTargetFile).Length>0)
                    {
                        try
                        {
                            albumManager = (AlbumManager)b.Deserialize(fileStream);
                        }
                        catch(System.Text.DecoderFallbackException e)
                        {
                            errorMessage = "Error in decoding the saved data. File is corrupt. \n" +e.Message;
                        }
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

        /// <summary>
        /// Deserializer method that reads a file from OpenFileDialog and retrieve its data.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static AlbumManager Deserialize(string filePath, out string errorMessage)
        {
            FileStream fileStream = null;
            errorMessage = null;
            AlbumManager albumManager = new AlbumManager();

            try
            {
                if (File.Exists(filePath))
                {
                    fileStream = new FileStream(filePath, FileMode.Open);
                    BinaryFormatter b = new BinaryFormatter();
                    if (new FileInfo(filePath).Length > 0)
                    {
                        try
                        {
                            albumManager = (AlbumManager)b.Deserialize(fileStream);
                        }
                        catch (System.Text.DecoderFallbackException e)
                        {
                            errorMessage = "Error in decoding the saved data. File is corrupt. \n" + e.Message;
                        }
                    }
                }
            }
            catch (SerializationException e)
            {
                errorMessage = "Error loading saved data .\n" + e.Message;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
            return albumManager;
        }
    }
}
