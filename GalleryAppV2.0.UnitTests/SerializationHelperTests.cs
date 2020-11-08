using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess;
using BusinessLayer;
using System.IO;
using System.Text;
using System;

namespace GalleryAppV2._0.UnitTests
{
    [TestClass]
    public class SerializationHelperTests
    {
        [TestMethod]
        public void Serialize_SuccessfulSerialization_ReturnsTrue()
        {
            //Arrange:
            string testFilePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + "/data.bin";
            AlbumManager albumManagerTest = new AlbumManager();

            //Act:
            var result = SerializationHelper.Serialize(albumManagerTest, testFilePath);

            //Assert:
            Assert.IsTrue(result);
            File.Delete(testFilePath);
        }

        [TestMethod]
        public void Serialize_FilePathProblem_ReturnsFalse()
        {
            string testFilePath = "";
            AlbumManager albumManagerTest = new AlbumManager();
            var result = SerializationHelper.Serialize(albumManagerTest, testFilePath);

            Assert.IsFalse(result);
        }
        [TestMethod]
        public void Deserialize_SuccessfulSerialization_ReturnsAlbumManagerInstance()
        {
            //Create a test binFile
            string testFilePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + "/SuccessfulSerializationTest.bin";
            AlbumManager albumManagerTest = new AlbumManager();
            SerializationHelper.Serialize(albumManagerTest, testFilePath);

            var result = SerializationHelper.Deserialize(testFilePath, out _);

            Assert.IsInstanceOfType(result, typeof(AlbumManager));
            Assert.IsNotNull(result);
            File.Delete(testFilePath);
        }
        [TestMethod]
        public void Deserialize_FileNotFound_ThrowsException()
        {
            //Scenario when user tries to open a file from "recent files" folder, but file has been already removed or something like that.
            Assert.ThrowsException<FileNotFoundException>(() =>
            SerializationHelper.Deserialize("", out _)
            );
        }
        [TestMethod]
        public void Deserialize_FileCantBeDesrialized_ErrorMessageShouldBeNotNull()
        {
            string testFilePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + "/FileCantBeDesrializedTest.bin";
            using (File.Create(testFilePath)) { } //create a bin file and add some text to it to corrupt it ( deserializer won't throw exception or errors if file is empty).

            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
            using (FileStream fs = File.Create(testFilePath))
            {
                Byte[] randomText = new UTF8Encoding(true).GetBytes("whatsoever");
                fs.Write(randomText, 0, randomText.Length);
            }

            string error;
            SerializationHelper.Deserialize(testFilePath, out error);
            Assert.IsNotNull(error);
            File.Delete(testFilePath);
       }
        [TestMethod]
        public void Deserialize_UnAuthorizedAccessToFile_ErrorMessageShouldBeNotNull()
        {
            //PS: To run this test: a .bin data file must be saved on a a folder that is accessible only on admin level, example: program files, windows or network protected drives.
            string testFilePath = @"C:/Program Files (x86)/data.bin";
            string error;
            var result = SerializationHelper.Deserialize(testFilePath, out error);
            Assert.IsNotNull(result);
        }

    }
}
