using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

namespace catalog_manager
{
    public partial class Form1 : Form
    {
        string currentPath;
        bool driveInfo = true;

        public Form1()
        {
            InitializeComponent();
            currentPath = "D:\\";

            treeView1.Nodes.Clear();
            FillDrive();

        }

        private TreeNode createTree(DirectoryInfo dir)
        {
            TreeNode dirNode = new TreeNode(dir.Name);
            try
            {
                foreach (DirectoryInfo d in dir.GetDirectories())
                {
                    if(d.Exists)
                        dirNode.Nodes.Add(createTree(d));
                }
                foreach (FileInfo d in dir.GetFiles())
                {
                    if(d.Exists)
                        dirNode.Nodes.Add(new TreeNode(d.Name));
                }
            }
            catch(Exception ex)
            { }
            return dirNode;
        }

        private void FillNodes(string path)
        {
            treeView1.Nodes.Clear();
            DirectoryInfo d = new DirectoryInfo(path);
            treeView1.Nodes.Add(createTree(d));
        }

        private void FillDrive()
        {
	    // to read all drives 
            //    try
            //    {
            //        foreach (DriveInfo drive in DriveInfo.GetDrives())
            //        {
            //            FillDirectory(drive.Name);
            //        }
            //    }
            //    catch (Exception ex) { }

            FillNodes("D:\\");

        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            currentPath = e.Node.FullPath;
            try
            {
                if (File.Exists(currentPath))
                {
                    System.Diagnostics.Process.Start(currentPath);
                }
            }
            catch (Exception ex) { }
        }

        //rename
        private void buttonRename_Click(object sender, EventArgs e)
        {
            try
            {
                currentPath = treeView1.SelectedNode.FullPath;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            string newName;
            Form2 f = new Form2();
            f.ShowDialog();
            newName = f.textBox1.Text;
            if (Directory.Exists(currentPath))
            {
                try
                {
                    Directory.Move(currentPath, treeView1.SelectedNode.Parent.FullPath + '\\' + newName);
                    treeView1.SelectedNode.Text = newName;
                    return;
                }catch(IOException ex)
                {
                    MessageBox.Show("Folder cannot be renamed. Try again later.\n" + ex.Message);
                    return;
                }
            }
            if (File.Exists(currentPath))
            {
                try
                {
                    File.Move(currentPath, treeView1.SelectedNode.Parent.FullPath + '\\' + newName + currentPath.Substring(currentPath.LastIndexOf('.')));
                    treeView1.SelectedNode.Text = newName + currentPath.Substring(currentPath.LastIndexOf('.'));
                    return;
                }catch(IOException ex)
                {
                    MessageBox.Show("File cannot be renamed. Try again later.\n" + ex.Message);
                    return;
                }
            }
        }

        //delete
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                currentPath = treeView1.SelectedNode.FullPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            if (Directory.Exists(currentPath))
            {
                Directory.Delete(currentPath, true);
                treeView1.SelectedNode.Remove();
                return;
            }

            if (File.Exists(currentPath))
            {
                File.Delete(currentPath);
                treeView1.SelectedNode.Remove();
                return;
            }
        }

        //about
        private void buttonAbout_Click(object sender, EventArgs e)
        {
            try
            {
                currentPath = treeView1.SelectedNode.FullPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            if (Directory.Exists(currentPath))
            {
                DirectoryInfo dir = new DirectoryInfo(currentPath);
                MessageBox.Show("Directory name: "+dir.Name+"\nFull dir name: "+
                    dir.FullName+"\nCreation time: " +dir.CreationTime+"\nRoot directory: "+dir.Root, "About");
                return;
            }
            if (File.Exists(currentPath))
            {
                FileInfo file = new FileInfo(currentPath);
                MessageBox.Show("File name: " + file.Name + "\nFull name: " + file.FullName +
                    "\nLength: " + file.Length + "\nExtension: " + file.Extension + "\nPath: " + file.DirectoryName, "About");
                return;
            }

        }

        //create folder
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                currentPath = treeView1.SelectedNode.FullPath;

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            if (Directory.Exists(currentPath))
            {
                Form2 f = new Form2();
                f.ShowDialog();
                if (!Directory.Exists(currentPath + '\\' + f.textBox1.Text))
                {
                    Directory.CreateDirectory(currentPath + '\\' + f.textBox1.Text);
                    treeView1.SelectedNode.Nodes.Add(new TreeNode(f.textBox1.Text));
                    return;
                }
                else
                    MessageBox.Show("Directory with this name already exists", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //create file
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                currentPath = treeView1.SelectedNode.FullPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            if (Directory.Exists(currentPath))
            {
                Form2 f = new Form2();
                f.ShowDialog();
                if (!File.Exists(currentPath + '\\' + f.textBox1.Text))
                {
                    File.Create(currentPath + '\\' + f.textBox1.Text);
                    treeView1.SelectedNode.Nodes.Add(new TreeNode(f.textBox1.Text));
                    return;
                }
                else
                    MessageBox.Show("File with this name already exists", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //drive info
        private void button4_Click(object sender, EventArgs e)
        {
            if (driveInfo)
            {
                try
                {
                    foreach (DriveInfo drive in DriveInfo.GetDrives())
                    {
                        label3.Text += "Name: " + drive.Name + "\nType: " + drive.DriveType + "\nTotal size: " + drive.TotalSize +
                            "\nFree space: " + drive.TotalFreeSpace + "\nLabel: " + drive.VolumeLabel + "\n\n";
                    }
                }
                catch (Exception ex) { }

                driveInfo = false;
            }
            else
            {
                label3.Text = "";
                driveInfo = true;
            }
        }
    }
}
