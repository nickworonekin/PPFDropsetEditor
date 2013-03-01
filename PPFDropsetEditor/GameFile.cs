using System;
using System.IO;
using System.Windows.Forms;

namespace PPFDropsetEditor
{
    public class GameFile
    {
        // File Sizes for each version
        private const int
            V100 = 4316606,
            V105 = 4320702,
            V107 = 4140490,
            V108 = V107,
            V109 = 4152778,
            V110 = V109,
            V112 = 983040,
            V200 = 1028096,
            V202 = 1052672,
            PPF2 = 1365424;

        // Offsets
        public int OffsetStart;

        // Game
        public byte[] Data;
        public Game selectedGame;
        public enum Game // Used to determine which game this is (PPF1 or PPF2)
        {
            PPF1, // Game is PPF1
            PPF2, // Game is PPF2
        };

        // File
        string file;

        public GameFile()
        {
            SelectFile();
        }

        private void SelectFile()
        {
            // We're not even going to bother to check to see if the game is installed
            // since it supports PPF PC and PPF2 PS2.
            // Just display the load dialog.
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Game Files (*.exe; SLPM_661.04)|*.exe; SLPM_661.04|All Files (*.*)|*.*";
                ofd.Title = "Select either a PPF PC executable or a PPF2 PS2 executable";
                ofd.AddExtension = true;
                ofd.RestoreDirectory = true;
                ofd.CheckFileExists = true;

                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK) // Attempt to load the file
                {
                    Load(ofd.FileName);
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }

        private void Load(string fname)
        {
            // Get the version. Returns true if successful or false if unsuccessful
            // Data will be set in this function.
            bool success = CheckVersion(fname);
            if (!success)
            {
                DialogResult result = MessageBox.Show("Unknown or unsupported version of PPF PC or PPF2 PS2 selected.\nPress \"Retry\" to load a supported file or \"Cancel\" to exit the program",
                    "Unknown File", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (result == DialogResult.Retry)
                    SelectFile();
                else
                    Environment.Exit(0);
            }
            else // Good file, load it!
            {

                Data = File.ReadAllBytes(fname);
                file = fname;

                // First, check to see if this game contains copy protection.
                if (IsCopyProtected(Data.Length))
                {
                    Data = null;
                    file = String.Empty;

                    MessageBox.Show("This copy of Puyo Puyo Fever appears to contain copy protection.\n" +
                        "All versions up to and including version 1.10 include copy protection.\n" +
                        "As a result the fever data is encrypted and cannot be edited.\n\nYou will need to do one of the following:\n" +
                        "- Upgrade to version 1.12 or greater. These versions do not contain copy protection.\n" +
                        "- Download Safedisc 2 Cleaner. This will allow you to remove the copy protection.\n\n" +
                        "Press OK to exit.", "Copy Protection Detected", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    Environment.Exit(0);
                }

                // Next, check to see if there is write permissions in the folder the game is in.
                if (!HasWritePermissions(fname))
                {
                    Data = null;
                    file = String.Empty;

                    MessageBox.Show("The game was loaded successfully, however the directory and/or file does not have write permissions. " +
                        "If you are running this program under Windows Vista or higher, you may need to run this program as an administrator.",
                        "No Write Permissions", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Environment.Exit(0);
                }

                // All good, now we can ask the user if they would like to create a backup
                if (!File.Exists(fname + ".bak"))
                {
                    DialogResult result = MessageBox.Show("Would you like to create a backup of " + Path.GetFileName(fname) + "?", "Create a Backup", MessageBoxButtons.YesNo, MessageBoxIcon.None);
                    if (result == DialogResult.Yes)
                    {
                        File.Copy(fname, fname + ".bak");
                        MessageBox.Show("Backup created with the filename " + Path.GetFileName(fname) + ".bak", "Backup Created");
                    }
                }
            }
        }

        // Check which version of PPF we have
        private bool CheckVersion(string fname)
        {
            // We're lazy, so we'll just check to see which version we have
            // by its filesize.
            switch (new FileInfo(fname).Length)
            {
                // Version 1.00
                case V100:
                    selectedGame = Game.PPF1;
                    OffsetStart = 0xD0228;
                    return true;

                // Version 1.05
                case V105:
                    selectedGame = Game.PPF1;
                    OffsetStart = 0xD1228;
                    return true;

                // Version 1.07 & 1.08
                case V107:
                    selectedGame = Game.PPF1;
                    OffsetStart = 0xD3238;
                    return true;

                // Version 1.09 & 1.10
                case V109:
                    selectedGame = Game.PPF1;
                    OffsetStart = 0xD6248;
                    return true;

                // Version 1.12
                case V112:
                    selectedGame = Game.PPF1;
                    OffsetStart = 0xDCA00;
                    return true;

                // Version 2.00
                case V200:
                    selectedGame = Game.PPF1;
                    OffsetStart = 0xEB298;
                    return true;

                // Version 2.0.2011060114
                case V202:
                    selectedGame = Game.PPF1;
                    OffsetStart = 0xF12A8;
                    return true;

                // PPF2 PS2
                case PPF2:
                    selectedGame = Game.PPF2;
                    OffsetStart = 0x11C220;
                    return true;
            }

            // I don't know which version this is!
            return false;
        }

        // Check to see if the EXE is copy protected (because the fever data is encrypted).
        private bool IsCopyProtected(int size)
        {
            if (size == V100 && Data[278] != 4) return true;
            if (size == V105 && Data[270] != 4) return true;
            if (size == V107 && Data[270] != 4) return true;
            if (size == V109 && Data[262] != 4 && Data[278] != 4) return true;

            return false;
        }

        // Save the file
        public void Save()
        {
            File.WriteAllBytes(file, Data);
        }

        // Checks to see if we have write permissions to a directory or the file
        private bool HasWritePermissions(string fname)
        {
            // We can do this simply by opening the file for writing
            try
            {
                using (FileStream outStream = new FileStream(fname, FileMode.Open, FileAccess.Write)) { }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}