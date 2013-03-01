using System;
using System.IO;
using System.Windows.Forms;

namespace PPFDropsetEditor
{
    public static class ImportExport
    {
        public static bool Import(out byte[] data)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Dropset Data|*.dat|All Files|*.*";
                ofd.Title = "Import Dropset Data";
                ofd.AddExtension = true;
                ofd.CheckFileExists = true;
                ofd.RestoreDirectory = true;
                DialogResult result = ofd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Make sure filesize is correct
                    long size = new FileInfo(ofd.FileName).Length;
                    if (size != 22)
                    {
                        MessageBox.Show("This is not a valid dropset data file.\n(File Size is incorrect.)", "Import Unsuccessful");
                        data = null;
                        return false;
                    }

                    // Let's read the data in now
                    data = File.ReadAllBytes(ofd.FileName);
                    if (!Compare(data, new byte[] { 0x50, 0x50, 0x46, 0x44, 0x53, 0x45 }, 0))
                    {
                        MessageBox.Show("This is not a valid dropset data file.\n(Header is incorrect.)", "Import Unsuccessful");
                        data = null;
                        return false;
                    }

                    return true;
                }

                data = null;
                return false;
            }
        }

        // Export data
        public static void Export(byte[] data)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Dropset Data|*.dat";
                sfd.Title = "Export Dropset Data";
                sfd.AddExtension = true;
                sfd.RestoreDirectory = true;
                sfd.OverwritePrompt = true;

                DialogResult result = sfd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    try
                    {
                        using (FileStream outstream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write))
                        {
                            outstream.Write(new byte[] { 0x50, 0x50, 0x46, 0x44, 0x53, 0x45 }, 0, 6);
                            outstream.Write(data, 0, data.Length);
                            outstream.Close();
                        }

                        MessageBox.Show("Dropset data exported successfully.", "Export Successful");
                    }
                    catch
                    {
                        MessageBox.Show("An error occured when writing the dropset data.", "Export Unsuccessful");
                    }
                }
            }
        }

        // Compares an array to another array
        private static bool Compare(byte[] a1, byte[] a2, int startIndex)
        {
            for (int i = 0; i < a2.Length; i++)
            {
                if (a1[startIndex + i] != a2[i])
                    return false;
            }

            return true;
        }
    }
}