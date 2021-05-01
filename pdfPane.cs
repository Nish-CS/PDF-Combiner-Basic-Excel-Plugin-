using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using iTextSharp;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace PDF_Combiner
{
    public partial class pdfPane : UserControl
    {
        public pdfPane()
        {
            InitializeComponent();
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select PDF Files";
            openFileDialog.DefaultExt = "pdf";
            openFileDialog.Filter = "pdf files (*.pdf)|*.pdf";
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Multiselect = true;

            DialogResult dr = openFileDialog.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                foreach (String file in openFileDialog.FileNames)
                {
                    try
                    {
                        string ext = Path.GetExtension(file);
                        if (ext == ".pdf")
                        {
                            //only allow pdf files ... will cause issues otherwise
                            listView1.Items.Add(file.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }

            listView1.Refresh();
        }

        private void btnSaveLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.  
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtSaveLocation.Text = folderBrowserDialog.SelectedPath;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listView1.Refresh();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(listView1.Items.Count == 0 )
            {
                MessageBox.Show(this, "No files selected", "Save Error");
                return;
            }

            if (listView1.Items.Count == 1)
            {
                MessageBox.Show(this, "Requires more than one file to be merged", "Save Error");
                return;
            }

            var name = txtSaveName.Text.Split('.');
            int count = name.Length;

            String filename = name[0];
            String fileext = name[count - 1];

            if (count > 1)
            {
                if (fileext != "pdf")
                {
                    filename = filename + ".pdf";
                }
            }
            else
            {
                filename = filename + ".pdf";
            }

            try
            {
                string path = Path.GetDirectoryName(txtSaveLocation.Text.Trim());
                if(path.Trim() == "" || path.Trim().Length == 0)
                {
                    MessageBox.Show(this, "Invalid save path", "Save Error");
                    return;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, "Invalid save path", "Save Error");
                return;
            }

            if (!HasWritePermissions(txtSaveLocation.Text.Trim()))
            {
                MessageBox.Show(this, "User does not have permission to write in selected folder", "Permissions Error");
                return;
            }

            string fullpath = txtSaveLocation.Text.Trim() + "\\" + filename;

            

            using(FileStream stream = new FileStream(fullpath,FileMode.Create))
            {
                Document pdf = new Document();
                PdfSmartCopy copy = new PdfSmartCopy(pdf, stream);
                PdfReader reader = null;

                try
                {
                    pdf.Open();
                    foreach(ListViewItem item in listView1.Items)
                    {
                        string t = item.SubItems[0].Text;
                        reader = new PdfReader(t);
                        copy.AddDocument(reader);
                        reader.Close();
                    }
                }
                catch(Exception ex)
                {
                    if(reader != null)
                    {
                        reader.Close();
                    }
                    MessageBox.Show(this, ex.Message, "Merge Error");
                    return;
                }
                finally
                {
                    if(pdf != null && pdf.IsOpen() == true)
                    {
                        pdf.Close();
                    }
                }
                MessageBox.Show(this, "PDFs Successfully Merged", "Merge Success");
            }
        }
            

           

        private bool HasWritePermissions(string path)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                DirectorySecurity acl = di.GetAccessControl();
                AuthorizationRuleCollection rules = acl.GetAccessRules(true, true, typeof(NTAccount));

                WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(currentUser);
                foreach (AuthorizationRule rule in rules)
                {
                    FileSystemAccessRule fsAccessRule = rule as FileSystemAccessRule;
                    if (fsAccessRule == null)
                        continue;

                    if ((fsAccessRule.FileSystemRights & FileSystemRights.WriteData) > 0)
                    {
                        NTAccount ntAccount = rule.IdentityReference as NTAccount;
                        if (ntAccount == null)
                        {
                            continue;
                        }

                        if (principal.IsInRole(ntAccount.Value))
                        {
                            return true;

                        }
                    }
                }
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
