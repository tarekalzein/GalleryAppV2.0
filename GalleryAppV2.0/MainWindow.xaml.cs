using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BusinessLayer;


namespace GalleryAppV2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FolderManager folderManager;
        public MainWindow()
        {
            InitializeComponent();
            folderManager = new FolderManager();

            #region For testing:
            folderManager.AddNewFolder("Test1");
            folderManager.AddNewFolder("Test2");
            folderManager.AddNewFolder("Test3");
            folderManager.AddNewFolder("Test4");
            folderManager.AddNewFolder("Test5");


            folderManager.GetFolderAtIndex(1).AddSubFolder();
            folderManager.GetFolderAtIndex(1).SubFolders[0].AddSubFolder();
            folderManager.GetFolderAtIndex(1).SubFolders[0].SubFolders[0].AddSubFolder();

            folderManager.GetFolderAtIndex(0).AlbumManager.AddNewAlbum(new Album("Test Album", "Some Description"));
            #endregion


            InitializeGUI();
        }

        private void InitializeGUI()
        {
            LoadTreeViewItems();
        }

        private void LoadTreeViewItems()
        {
            ContentTreeView.Items.Clear();
            foreach (FolderItem folder in folderManager.GetApplicationFolders())
            {
                ContentTreeView.Items.Add(CreateTreeItem(folder));
            }
        }

        private TreeViewItem CreateTreeItem(object o)
        {
            TreeViewItem item = new TreeViewItem();

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            Image img = new Image();
            img.Width = 16;
            img.Height = 16;

            Label lbl = new Label();

            if (o is FolderItem)
            {
                //item.Header = (o as FolderItem).FolderName;
                lbl.Content = (o as FolderItem).FolderName;
                img.Source = new BitmapImage(new Uri("Assets/icons8-folder-48.png", UriKind.RelativeOrAbsolute));
                stackPanel.Children.Add(img);
                stackPanel.Children.Add(lbl);

                item.Header = stackPanel;

                if ((o as FolderItem).SubFolders != null && (o as FolderItem).SubFolders.Count > 0)
                {
                    foreach (FolderItem subfolder in (o as FolderItem).SubFolders)
                    {
                        item.Items.Add(CreateTreeItem(subfolder)); //Recurring to same method to do all check again (add subfolders in subfolders).
                    }
                }
                if ((o as FolderItem).AlbumManager.GetAlbums().Count > 0)
                {
                    foreach (Album album in (o as FolderItem).AlbumManager.GetAlbums())
                    {
                        item.Items.Add(CreateTreeItem(album));
                    }
                }
            }
            if (o is Album)
            {
                lbl.Content = (o as Album).AlbumTitle;
                img.Source = new BitmapImage(new Uri("Assets/icons8-photo-gallery-100.png",UriKind.RelativeOrAbsolute));
                stackPanel.Children.Add(img);
                stackPanel.Children.Add(lbl);

                item.Header = stackPanel;

            }
            item.Tag = o;
            return item;
        }

        private void ContentTreeView_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            if(item.Tag is Album)
            {
                MessageBox.Show("Selected item is an Album");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(ContentTreeView.SelectedItem==null)
            {
                folderManager.AddNewFolder("new folder");
                LoadTreeViewItems();                
            }
            else
            {
                

            }

        }
    }
}
