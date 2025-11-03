using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Manning.MyPhotoAlbum;
using Manning.MyPhotoControls;
using MyPhotoAlbum;

namespace MyPhotos
{
    public partial class MainForm : Form
    {
        //to hold the PixelDialogform object
        private PixelDialog _dlgPixel = null;
        private PixelDialog PixelForm
        {
            get { return _dlgPixel; }
            set { _dlgPixel = value; }
        }
        protected void menuPixelData_Click(object sender, EventArgs e)
        {
            if (PixelForm == null || PixelForm.IsDisposed)
            {
                PixelForm = new PixelDialog();
                PixelForm.Owner = this;
            }
            PixelForm.Show();
            Point p = pbxPhoto.PointToClient(Form.MousePosition);
            UpdatePixelDialog(p.X, p.X);
        }
        //update method if the PixelForm exists
        private void UpdatePixelDialog(int x, int y)
        {
            if (PixelForm != null && PixelForm.Visible)
            {
                Bitmap bmp = Manager.CurrentImage;

                PixelForm.Text = (Manager.Current == null ? "Pixel Data" : Manager.Current.Caption);
                if (bmp == null || !pbxPhoto.DisplayRectangle.Contains(x, y))
                    PixelForm.ClearPixelData();
                else
                    PixelForm.UpdatePixelData(x, y, bmp, pbxPhoto.DisplayRectangle,
                        new Rectangle(0, 0, bmp.Width,bmp.Height), 
                        pbxPhoto.SizeMode);
            }
        }

        //to hold an AlbumManager instance
        private AlbumManager _manager;

        //Add a private Manager Property to retrieve or assign the current Manager
        private AlbumManager Manager
        {
            get { return _manager; }
            set { _manager = value; }
        }


        //initialize 
        private void SetTitleBar()
        {
            Version ver = new Version(Application.ProductVersion);
            string name = Manager.FullName;
            this.Text = String.Format(
                "MyPhotos {0:0}.{1:0}", ver.Major, ver.Minor,
                string.IsNullOrEmpty(name) ? "Untitled" : name
                );
        }

        public MainForm()
        {

            InitializeComponent();
            NewAlbum();
            menuView.DropDown = ctxMenuPhoto;


        }
        //Exit button
        private void menuFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }


        //method to perform the click of the drop down       
        private void menuImage_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ProcessImageClick(e);
        }

        private void ProcessImageClick(ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            string enumVal = item.Tag as string;
            if (enumVal != null)
            {
                pbxPhoto.SizeMode = (PictureBoxSizeMode)
                    Enum.Parse(typeof(PictureBoxSizeMode), enumVal);
            }
        }

        private void menuImage_DropDownOpening(object sender, EventArgs e)
        {
            ProcessImageOpening(sender as ToolStripDropDownItem);
        }

        private void ProcessImageOpening(ToolStripDropDownItem parent)
        {
            if (parent != null)
            {
                string enumVal = pbxPhoto.SizeMode.ToString();

                //loop through menu items
                foreach (ToolStripDropDownItem item in parent.DropDownItems)
                {
                    //check if this menu items matches to current size mode
                    item.Enabled = (pbxPhoto.Image != null);

                    //item.Checked = item.Tag.Equals(enumVal);
                    if (item is ToolStripMenuItem menuItem)
                    {
                        menuItem.Checked = menuItem.Tag != null && menuItem.Tag.Equals(enumVal);
                    }

                }
            }
        }

        private void setStatusStrip()
        {
            if (pbxPhoto.Image != null)
            {
                statusInfo.Text = Manager.Current.Caption;
                statusImageSize.Text = string.Format("{0:#}x{1:#}",
                    pbxPhoto.Image.Width,
                    pbxPhoto.Image.Height
                    );
                statusAlbumPos.Text = string.Format("{0:0}/{1:0} ", Manager.Index + 1, Manager.Album.Count);
            } else
            {
                statusInfo.Text = null;
                statusImageSize.Text = null;
                statusAlbumPos.Text = null;
            }
        }

        private void DisplayAlbum()
        {
            pbxPhoto.Image = Manager.CurrentImage;
            setStatusStrip();
            SetTitleBar();

            Point p = pbxPhoto.PointToClient(
                Form.MousePosition
                );
            UpdatePixelDialog(p.X, p.Y);
        }

        private void NewAlbum()
        {
            if (Manager == null || SaveAndCloseAlbum())
            {
                //Album closed, create a new one
                Manager = new AlbumManager();
                DisplayAlbum();
            }
        }

        private void menuFileNew_Click(object sender, EventArgs e)
        {
            NewAlbum();
        }

        private void menuFileOpen_Click(object sender, EventArgs e)
        {
            //Allow user to select a new album
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Title = "Open Album";
            dlg.Filter = "Album files (*.abm)|*.abm|" + "All files(*.*)|*.*";
            dlg.InitialDirectory = AlbumManager.DefaultPath;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string path = dlg.FileName;

                //Close any existing album
                if (!SaveAndCloseAlbum())
                    return; //close canceled

                //TOD: save any existing album.
                try
                {
                    //open the new album
                    //TODO: handle invalid album file
                    Manager = new AlbumManager(path);
                }
                catch (AlbumStorageException aex)
                {
                    string msg = string.Format(
                        "Unable to open album file {0}\n({1})",
                        path, aex.Message);
                    MessageBox.Show(msg, "Unable to open");
                    Manager = new AlbumManager();
                }
                DisplayAlbum();
            }
            dlg.Dispose();
        }
        //save menu item
        private void SaveAlbum(string name)
        {
            try
            {
                Manager.Save(name, true);
            }
            catch (AlbumStorageException aex)
            {
                string msg = string.Format(
                    "Unable to save album {0} ({1})\n\n"
                    + "Do you wish to save the album "
                    + "under an alternate name?",
                    name, aex.Message
                    );
                DialogResult result = MessageBox.Show(
                    msg,
                    "unable to solve",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    SaveAsAlbum();
                }
            }
        }
        private void SaveAlbum()
        {
            if (String.IsNullOrEmpty(Manager.FullName))
                SaveAlbum(); //Force user to select name
            else
            {
                //save the album under the existing name
                SaveAlbum(Manager.FullName);
            }
        }

        private void menuFileSave_Click(object sender, EventArgs e)
        {
            SaveAlbum();
        }

        private void SaveAsAlbum()
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Title = "Save Album";
                dlg.DefaultExt = "abm";
                dlg.Filter = "Album files (*.abm | *.abm|" + "All files|*.*";
                dlg.InitialDirectory = AlbumManager.DefaultPath;
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    SaveAlbum(dlg.FileName);

                    //update title bar to include new name 
                    SetTitleBar();
                }
                dlg.Dispose();
            }
        }
        private void menuFileSaveAs_Click(object sender, EventArgs e)
        {
            SaveAsAlbum();
        }

        //Method to add photos into my photo project 
        private void menuEditAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Title = "Add Photos";
            dlg.Multiselect = true;
            dlg.Filter = "Image Files (JPEG, GIF, BMP, etc.)|"
                + "*.jpg;*.jpeg;*.gif;*.bmp;"
                + "*.tif;*.tiff;*.png|"
                + "JPEG files (*.jpg;*.jpeg)|*.jpg;*.jpeg|"
                + "GIF files (*.gif)|*.gif|"
                + "BMP files (*.bmp)|*.bmp|"
                + "TIFF files (*.tif;*.tiff)|*.tif;*.tiff|"
                + "PNG files (*.png)|*.png|"
                + "All files (*.*)|*.*";
            dlg.InitialDirectory = Environment.CurrentDirectory;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string[] files = dlg.FileNames;
                int index = 0;
                foreach (string s in files)
                {
                    Photograph photo = new Photograph(s);

                    //add the file (if not already present)
                    index = Manager.Album.IndexOf(photo);
                    if (index < 0)
                        Manager.Album.Add(photo);
                    else
                        photo.Dispose(); //photo already there
                }
                Manager.Index = Manager.Album.Count - 1;
            }

            dlg.Dispose();
            DisplayAlbum(); 
        }
        
        //Method to Remove photos in each Album
        private void menuEditRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (Manager.Album.Count > 0)
                {
                    string photoName = Manager.Current != null ? (string.IsNullOrEmpty(Manager.Current.Caption) ? Manager.Current.FileName : Manager.Current.Caption) : "current photo";

                    string msg = string.Format("Do you want to remove the photo \"{0}\" from the album?", photoName);
                    DialogResult result = MessageBox.Show(
                        this,
                        msg,
                        "Confirm Remove",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2
                        );
                    if (result == DialogResult.Yes)
                    {
                        Manager.Album.RemoveAt(Manager.Index);
                        DisplayAlbum();
                    }
                }
            } catch(AlbumStorageException aex)
            {
                //Error message box say try again to delete the photo
                MessageBox.Show(
                    "Unable to delete the photo" + aex
                    );
                DisplayAlbum();
            }
            
        }

        private void menuNext_Click(object sender, EventArgs e)
        {
            if (Manager.Index < Manager.Album.Count - 1)
            {
                Manager.Index++;
                DisplayAlbum();
            }
        }

        private void menuPrevious_Click(object sender, EventArgs e)
        {
            if (Manager.Index > 0)
            {
                Manager.Index--;
                DisplayAlbum();
            }
        }

        private void ctxMenuPhoto_Opening(object sender, CancelEventArgs e)
        {
            menuNext.Enabled = (Manager.Index < Manager.Album.Count - 1);
            menuPrevious.Enabled = (Manager.Index > 0);
        }

        private bool SaveAndCloseAlbum()
        {
            if (Manager.Album.HasChanged)
            {
                //offer to save the current album
                string msg;
                if (String.IsNullOrEmpty(Manager.FullName))
                    msg = "Do you wish to save your changes?";
                else
                    msg = String.Format("Do you wish to "
                        + "Save your changes to \n{0}?",
                        Manager.FullName);
                DialogResult results = MessageBox.Show(this, msg, "Save Changes?",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                if (results == DialogResult.Cancel)
                    SaveAlbum();
                else if (results == DialogResult.Cancel)
                    return false;
            }
            //close the album and return true
            if (Manager.Album != null)
                Manager.Album.Dispose();

            Manager = new AlbumManager();
            SetTitleBar();
            return true;

        }
        //calls the SaveAndCloseAlbum() method..
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (SaveAndCloseAlbum() == false)
                e.Cancel = true;
            else
                e.Cancel = false;
            base.OnFormClosing(e);
        }

        private void pbxPhoto_MouseMove(object sender, MouseEventArgs e)
        {
            UpdatePixelDialog(e.X, e.Y);
        }
    }
}
