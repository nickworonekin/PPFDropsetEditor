using System;
using System.Drawing;
using System.Windows.Forms;

namespace PPFDropsetEditor
{
    public class MainWindow : Form
    {
        // Game
        GameFile gameFile;

        // Character Names
        private string[] charNames; // This will point to either CharNamesPPF1 or CharNamesPPF2
        private readonly string[] CharNamesPPF1 = new string[] {
            "Amitie",
            "Oshare Bones",
            "Klug",
            "Dongurigaeru",
            "Rider",
            "Onion Pixy",
            "Ocean Prince",
            "Raffine",
            "Yu",
            "Tarutaru",
            "Hohow Bird",
            "Ms. Accord",
            "Frankensteins",
            "Arle",
            "Popoi",
            "Carbuncle"
        };
        private readonly string[] CharNamesPPF2 = new string[] {
            "Amitie",
            "Oshare Bones",
            "Klug",
            "Dongurigaeru",
            "Rider",
            "Onion Pixy",
            "Ocean Prince",
            "Raffine",
            "Yu",
            "Tarutaru",
            "Hohow Bird",
            "Ms. Accord",
            "Frankensteins",
            "Arle",
            "Sig",
            "Lemres",
            "Feli",
            "Baldanders",
            "Gogotte",
            "Akuma",
            "Strange Klug",
        };

        ComboBoxEx[] dropsetBox = new ComboBoxEx[16];
        PictureBox[] dropsetImage = new PictureBox[16];

        public MainWindow()
        {
            this.ClientSize = new Size(576, 292);
            this.MinimumSize = this.Size;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = PPFDropsetEditor.ProgramName + " v" + PPFDropsetEditor.ProgramVersion;
            this.Icon = Resources.ProgramIcon;
            this.Show();
            this.Enabled = false;

            // Display the disclaimer
            Disclaimer.Display();

            // Before we do anything else, we need to load the game file
            gameFile = new GameFile();

            // Set charNames to the correct array
            if (gameFile.selectedGame == GameFile.Game.PPF1)
                charNames = CharNamesPPF1;
            else if (gameFile.selectedGame == GameFile.Game.PPF2)
                charNames = CharNamesPPF2;

            // Create the Character Selection Box
            ComboBox charSelectBox = new ComboBox();
            charSelectBox.DropDownStyle = ComboBoxStyle.DropDownList;
            charSelectBox.Location = new Point(10, 10);
            charSelectBox.Size = new Size(200, 21);
            charSelectBox.Items.AddRange(charNames);
            charSelectBox.MaxDropDownItems = charNames.Length;
            charSelectBox.SelectedIndex = 0;
            charSelectBox.SelectedIndexChanged += delegate(object sender, EventArgs e)
            {
                GetDropset(charSelectBox.SelectedIndex);
            };

            Button aboutButton = new Button();
            aboutButton.UseVisualStyleBackColor = true;
            aboutButton.Text = "About";
            aboutButton.Location = new Point(this.ClientSize.Width - 74, 8);
            aboutButton.Size = new Size(64, 24);
            aboutButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            aboutButton.Click += delegate(object sender, EventArgs e)
            {
                new About();
            };

            Panel charSelectPanel = new Panel();
            charSelectPanel.BackColor = SystemColors.Window;
            charSelectPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            charSelectPanel.Location = new Point(0, 0);
            charSelectPanel.Size = new Size(this.ClientSize.Width, 40);
            charSelectPanel.Controls.Add(charSelectBox);
            charSelectPanel.Controls.Add(aboutButton);
            this.Controls.Add(charSelectPanel);

            // Add the content panel
            Panel contentPanel = new Panel();
            contentPanel.BackColor = SystemColors.Window;
            contentPanel.BorderStyle = BorderStyle.FixedSingle;
            contentPanel.Location = new Point(8, 48);
            contentPanel.Size = new Size(560, 204);
            contentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            this.Controls.Add(contentPanel);

            // Show the drop sets & their comboboxes
            ImageList dropSetImageList = new ImageList();
            dropSetImageList.ImageSize = new Size(32, 32);
            dropSetImageList.Images.Add(Resources.dropset_2);
            dropSetImageList.Images.Add(Resources.dropset_3);
            dropSetImageList.Images.Add(Resources.dropset_B);
            dropSetImageList.Images.Add(Resources.dropset_4);
            for (int i = 0; i < 16; i++)
            {
                dropsetImage[i] = new PictureBox();
                dropsetImage[i].Location = new Point(8 + (i * 34), 8);
                dropsetImage[i].Size = new Size(32, 32);
                contentPanel.Controls.Add(dropsetImage[i]);

                Label label = new Label();
                label.Text = (i + 1).ToString();
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Location = new Point(8 + ((i % 8) * 68), 72 + ((int)(i / 8) * 66));
                label.Size = new Size(64, 16);
                contentPanel.Controls.Add(label);

                dropsetBox[i] = new ComboBoxEx();
                dropsetBox[i].DropDownStyle = ComboBoxStyle.DropDownList;
                dropsetBox[i].Location = new Point(8 + ((i % 8) * 68), 88 + ((int)(i / 8) * 66));
                dropsetBox[i].Size = new Size(64, 34);
                dropsetBox[i].ItemHeight = 34;
                dropsetBox[i].ImageList = dropSetImageList;
                dropsetBox[i].Name = i.ToString();
                dropsetBox[i].Items.Add(new ComboBoxExItem("", 0));
                dropsetBox[i].Items.Add(new ComboBoxExItem("", 1));
                dropsetBox[i].Items.Add(new ComboBoxExItem("", 2));
                dropsetBox[i].Items.Add(new ComboBoxExItem("", 3));
                dropsetBox[i].SelectedIndexChanged += SetDropsetImage;
                contentPanel.Controls.Add(dropsetBox[i]);
            }

            // Bottom Panel
            Panel bottomPanel = new Panel();
            bottomPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            bottomPanel.Location = new Point((this.ClientSize.Width / 2) - 104, 260);
            bottomPanel.Size = new Size(208, 24);
            bottomPanel.SizeChanged += delegate(object sender, EventArgs e)
            {
                bottomPanel.Left = (this.ClientSize.Width / 2) - 104;
            };
            this.Controls.Add(bottomPanel);

            // Let's add the buttons at the bottom.
            Button importButton = new Button();
            importButton.Text = "Import";
            importButton.Location = new Point(0, 0);
            importButton.Size = new Size(64, 24);
            bottomPanel.Controls.Add(importButton);
            importButton.Click += ImportData;

            Button saveButton = new Button();
            saveButton.Text = "Save";
            saveButton.Location = new Point(72, 0);
            saveButton.Size = new Size(64, 24);
            saveButton.Click += delegate(object sender, EventArgs e)
            {
                SetDropset(charSelectBox.SelectedIndex);
                gameFile.Save();
                MessageBox.Show("Dropset data saved successfully.", "Saved");
            };
            bottomPanel.Controls.Add(saveButton);

            Button exportButton = new Button();
            exportButton.Text = "Export";
            exportButton.Location = new Point(144, 0);
            exportButton.Size = new Size(64, 24);
            bottomPanel.Controls.Add(exportButton);
            exportButton.Click += ExportData;

            GetDropset(charSelectBox.SelectedIndex);

            this.Enabled = true;
        }

        private void GetDropset(int character)
        {
            int offset = gameFile.OffsetStart + (character * 0x10);
            for (int i = 0; i < 16; i++)
            {
                switch (gameFile.Data[offset + i])
                {
                    case 0x1E: dropsetBox[i].SelectedIndex = 1; break;
                    case 0x28: dropsetBox[i].SelectedIndex = 2; break;
                    case 0x29: dropsetBox[i].SelectedIndex = 3; break;
                    default: dropsetBox[i].SelectedIndex = 0; break;
                }

                // Dropset Image changes as part of "SelectedIndexChanged".
            }
        }

        private void SetDropset(int character)
        {
            int offset = gameFile.OffsetStart + (character * 0x10);
            for (int i = 0; i < 16; i++)
            {
                switch (dropsetBox[i].SelectedIndex)
                {
                    case 1: gameFile.Data[offset + i] = 0x1E; break;
                    case 2: gameFile.Data[offset + i] = 0x28; break;
                    case 3: gameFile.Data[offset + i] = 0x29; break;
                    default: gameFile.Data[offset + i] = 0x14; break;
                }
            }
        }

        private void SetDropsetImage(object sender, EventArgs e)
        {
            int i = int.Parse((sender as ComboBoxEx).Name);
            switch ((sender as ComboBoxEx).SelectedIndex)
            {
                case 1: dropsetImage[i].Image = Resources.dropset_3; break;
                case 2: dropsetImage[i].Image = Resources.dropset_B; break;
                case 3: dropsetImage[i].Image = Resources.dropset_4; break;
                default: dropsetImage[i].Image = Resources.dropset_2; break;
            }
        }

        // Import Data
        private void ImportData(object sender, EventArgs e)
        {
            byte[] data;
            bool success = ImportExport.Import(out data);

            if (!success)
                return;

            // Read in the dropsets
            int pos = 6;
            for (int i = 0; i < 16; i++)
            {
                switch (data[pos])
                {
                    case 0x1E: dropsetBox[i].SelectedIndex = 1; break;
                    case 0x28: dropsetBox[i].SelectedIndex = 2; break;
                    case 0x29: dropsetBox[i].SelectedIndex = 3; break;
                    default: dropsetBox[i].SelectedIndex = 0; break;
                }

                pos++;
            }
        }

        // Export Data
        private void ExportData(object sender, EventArgs e)
        {
            byte[] data = new byte[16]; // Such a tiny file!
            int pos = 0;

            // Write the dropsets
            for (int i = 0; i < 16; i++)
            {
                switch (dropsetBox[i].SelectedIndex)
                {
                    case 1: data[pos] = 0x1E; break;
                    case 2: data[pos] = 0x28; break;
                    case 3: data[pos] = 0x29; break;
                    default: data[pos] = 0x14; break;
                }
                pos++;
            }

            // Now we can export it
            ImportExport.Export(data);
        }
    }
}