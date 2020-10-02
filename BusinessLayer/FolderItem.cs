using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BusinessLayer
{
    /// <summary>
    /// This Class is not implemented yet.
    /// Todo: implement adding new folders in the treeview with Hierarchial data structure.
    /// </summary>
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
