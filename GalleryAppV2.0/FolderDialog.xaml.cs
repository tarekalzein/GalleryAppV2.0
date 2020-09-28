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
    /// Interaction logic for FolderDialog.xaml
    /// </summary>
    public partial class FolderDialog : Window
    {
        public FolderDialog()
        {
            InitializeComponent();
        }
        public FolderDialog(string oldName)
        {
            InitializeComponent();
            FolderName_txtbox.Text = oldName;
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(FolderName_txtbox.Text))
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
            return FolderName_txtbox.Text;
        }
    }
}
