using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GalleryAppV2._0
{
    /// <summary>
    /// Interaction logic for NewDialog.xaml
    /// </summary>
    public partial class NewDialog : Window
    {
        public NewDialog()
        {
            InitializeComponent();
        }
        public NewDialog(string oldName,string oldDescription)
        {
            InitializeComponent();
            AlbumName_txtbox.Text = oldName;
            AlbumDescription_txtbox.Text = oldDescription;

        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(AlbumName_txtbox.Text))
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Folder name can't be empty");
            }

        }
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public string GetFolderName()
        {
            return AlbumName_txtbox.Text;
        }
        public string GetFolderDescription()
        {
            if (!string.IsNullOrEmpty(AlbumDescription_txtbox.Text))
                return AlbumDescription_txtbox.Text;
            else
                return ""; //empty description
        }
    }
}
