using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BusinessLayer
{
    /// <summary>
    /// Not implemented yet. It will allow the user to organize the albums in folders and subfolders.
    /// </summary>
    public class FolderManager
    {
        ObservableCollection<FolderItem> applicationFolders;

        public FolderManager()
        {
            applicationFolders = new ObservableCollection<FolderItem>();
            FolderItem rootFolder = new FolderItem("Root");
            applicationFolders.Add(rootFolder);
        }

        public void AddNewFolder(string name)
        {
            if(!string.IsNullOrEmpty(name))
            {
                FolderItem newFolder = new FolderItem(name);
                applicationFolders.Add(newFolder);
            }
        }
        public FolderItem GetFolderAtIndex(int index)
        {
            if (ValidateIndex(index))
                return applicationFolders[index];
            else
                return null;
        }
        public void RemoveFolder(int index)
        {
            if (index != 0) //Prevent deleting root folder.
            {
                if (ValidateIndex(index))
                    applicationFolders.RemoveAt(index);
            }
        }
        public ObservableCollection<FolderItem> GetApplicationFolders()
        {
            return applicationFolders;
        }
        private bool ValidateIndex(int index)
        {
            if (index == 0 || index < applicationFolders.Count)
                return true;
            else
                return false;
        }
        
    }
}
