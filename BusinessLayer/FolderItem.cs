using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BusinessLayer
{
    public class FolderItem
    {
        public string FolderName { get; set; }
        public AlbumManager AlbumManager { get; }
        public ObservableCollection<FolderItem> SubFolders { get; }
        public FolderItem(string fName)
        {
            FolderName = fName;
            AlbumManager = new AlbumManager();
            SubFolders = new ObservableCollection<FolderItem>();
       }
        public void AddSubFolder()
        {

            SubFolders.Add(new FolderItem("test"));
        }
    }
}
