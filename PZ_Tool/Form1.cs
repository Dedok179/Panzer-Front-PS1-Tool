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
using Kaitai;

namespace PZ_Tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            backgroundTerrainProcessing.WorkerReportsProgress = true;
            backgroundTerrainProcessing.WorkerSupportsCancellation = true;

            backgroundTransportProcessing.WorkerReportsProgress = true;
            backgroundTransportProcessing.WorkerSupportsCancellation = true;

            backgroundTerrainProcessing.DoWork += new DoWorkEventHandler(backgroundTerrainProcessing_DoWork);
            backgroundTerrainProcessing.ProgressChanged += new ProgressChangedEventHandler(backgroundProcessing_ProgressChanged);
            backgroundTerrainProcessing.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundProcessing_RunWorkerCompleted);

            backgroundTransportProcessing.DoWork += new DoWorkEventHandler(backgroundTransportProcessing_DoWork);
            backgroundTransportProcessing.ProgressChanged += new ProgressChangedEventHandler(backgroundProcessing_ProgressChanged);
            backgroundTransportProcessing.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundProcessing_RunWorkerCompleted);
        }

        class FileInfoEntity
        {
            public FileInfoEntity(string n, uint l, int s) { FILENAME = n; LBA = l; SIZE = s; }

            public string FILENAME;
            public uint LBA;
            public int SIZE;
        }

        List<FileInfoEntity> gameDataInfo;

        byte[] transportData;
        byte[] minimapData;
        List<byte[]> levelGeoData;
        List<byte[]> levelTexData;

        bool transportFlag = false;

        string terrainDir;
        string transportDir;

        private BackgroundWorker backgroundTerrainProcessing = new BackgroundWorker();
        private BackgroundWorker backgroundTransportProcessing = new BackgroundWorker();

        private void backgroundTerrainProcessing_DoWork(object sender, DoWorkEventArgs e)
        {
            labelProgress.Text = "Data processing...";
            PZ_Map.LoadResources(levelTexData[comboBoxLevelPack.SelectedIndex], levelGeoData[comboBoxLevelPack.SelectedIndex], comboBoxLevel.SelectedIndex);
            backgroundTerrainProcessing.ReportProgress(20);

            labelProgress.Text = "Terrain processing...";
            PZ_Map.DrawTerrain(10);
            backgroundTerrainProcessing.ReportProgress(40);

            labelProgress.Text = "Export terrain data...";
            PZ_Map.ExportMap(terrainDir + Path.DirectorySeparatorChar + $"{PZ.LEVEL_NAME_TABLE[comboBoxLevel.SelectedIndex]}_Terrain.dae");           
            backgroundTerrainProcessing.ReportProgress(60);

            labelProgress.Text = "Export objects data...";
            PZ_Map.ExportObjects(terrainDir + Path.DirectorySeparatorChar + $"{PZ.LEVEL_NAME_TABLE[comboBoxLevel.SelectedIndex]}_Objects.dae");
            backgroundTerrainProcessing.ReportProgress(80);

            labelProgress.Text = "Export textures data...";
            PZ_Texture.SaveVRAM(terrainDir);
            backgroundTerrainProcessing.ReportProgress(100);

            labelProgress.Text = "Extraction complete!";
            System.Threading.Thread.Sleep(1000);
        }

        private void backgroundProcessing_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarProcess.Value = e.ProgressPercentage;
        }

        private void backgroundProcessing_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            panelProgress.Visible = false;
            progressBarProcess.Value = 0;
        }

        private void backgroundTransportProcessing_DoWork(object sender, DoWorkEventArgs e)
        {
            labelProgress.Text = "Export data...";
            PZ_Model.GetTankModels(transportData, transportDir, transportFlag, comboBoxTank.SelectedIndex);
            backgroundTransportProcessing.ReportProgress(100);

            labelProgress.Text = "Extraction complete!";
            System.Threading.Thread.Sleep(1000);
        }

        private uint PS1ToFileOffset(uint ofs)
        {
            return (ofs - 0x80010000) + 0x800;
        }

        private void GetInfoFromIso(string filepath)
        {
            var bin = Iso9660.FromFile(filepath);

            List<Iso9660.DirEntry> rootFolder = null, dataFolder = null, tankFolder = null, levelFolder = null;

            gameDataInfo = new List<FileInfoEntity>();

            rootFolder = bin.PrimaryVolDesc.VolDescPrimary.RootDir.Body.ExtentAsDir.Entries;

            for (int i = 0; i < rootFolder.Count - 1; i++)
            {
                if (rootFolder[i].Body.FileName.Split(';')[0] == "D")
                {
                    dataFolder = rootFolder[i].Body.ExtentAsDir.Entries;
                }
            }

            for (int i = 0; i < dataFolder.Count - 1; i++)
            {
                if (dataFolder[i].Body.FileName.Split(';')[0] == "BOBJ")
                {
                    levelFolder = dataFolder[i].Body.ExtentAsDir.Entries;
                }

                if (dataFolder[i].Body.FileName.Split(';')[0] == "TK")
                {
                    tankFolder = dataFolder[i].Body.ExtentAsDir.Entries;
                }
            }

            for (int i = 0; i < levelFolder.Count - 1; i++)
            {
                if (levelFolder[i].Body.FileName.Split(';')[0].Contains("BA") | levelFolder[i].Body.FileName.Split(';')[0].Contains("BB"))
                {
                    gameDataInfo.Add(new FileInfoEntity(levelFolder[i].Body.FileName.Split(';')[0], levelFolder[i].Body.LbaExtent.Le, levelFolder[i].Body.ExtentAsFile.Length));
                }

                if (levelFolder[i].Body.FileName.Split(';')[0].Contains("SMAL"))
                {
                    gameDataInfo.Add(new FileInfoEntity(levelFolder[i].Body.FileName.Split(';')[0], levelFolder[i].Body.LbaExtent.Le, levelFolder[i].Body.ExtentAsFile.Length));
                }
            }

            for (int i = 0; i < tankFolder.Count - 1; i++)
            {
                if (tankFolder[i].Body.FileName.Split(';')[0] == "TK.D3")
                {
                    gameDataInfo.Add(new FileInfoEntity(tankFolder[i].Body.FileName.Split(';')[0], tankFolder[i].Body.LbaExtent.Le, tankFolder[i].Body.ExtentAsFile.Length));
                }
            }

            bin.M_Io.Close();
        }

        private int GetFilesFromIso(string filepath)
        {           
            try
            {
                GetInfoFromIso(filepath);              
            }
            catch
            {
                MessageBox.Show("Bin reading error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);

                return 1;
            }

            levelGeoData = new List<byte[]>();
            levelTexData = new List<byte[]>();

            using (BinaryReader r = new BinaryReader(File.Open(filepath, FileMode.Open)))
            {
                for (int i = 0; i < gameDataInfo.Count; i++)
                {
                    byte[] data = GetFileFromSectors(r, gameDataInfo[i].LBA, gameDataInfo[i].SIZE);

                    if (gameDataInfo[i].FILENAME.Contains("BA"))
                        levelTexData.Add(data);

                    if (gameDataInfo[i].FILENAME.Contains("BB"))
                        levelGeoData.Add(data);

                    if (gameDataInfo[i].FILENAME == "SMAL.DAT")
                        minimapData = data;

                    if (gameDataInfo[i].FILENAME == "TK.D3")
                        transportData = data;
                }
            }           

            return 0;
        }

        private byte[] GetFileFromSectors(BinaryReader r, uint lba, int size)
        {
            List<byte> buffer = new List<byte>();

            r.BaseStream.Seek(lba * 2352, SeekOrigin.Begin);

            for (int i = 0; i < (size / 2048) + 1; i++)
            {
                r.ReadBytes(24);
                buffer.AddRange(r.ReadBytes(2048));
                r.ReadBytes(280);
            }

            return buffer.ToArray();
        }

        private void openPanzerFrontbinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog() { Filter = "Panzer Front *.BIN image|*.bin" };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                if (GetFilesFromIso(openFile.FileName) == 0)
                {
                    comboBoxTank.Items.Clear();
                    comboBoxTank.Items.AddRange(PZ.TANK_NAME_TABLE);
                    comboBoxTank.SelectedIndex = 0;

                    comboBoxLevelPack.Items.Clear();
                    comboBoxLevelPack.Items.AddRange(new string[] { "1", "2", "3", "4" });
                    comboBoxLevelPack.SelectedIndex = 0;

                    comboBoxLevel.Items.Clear();
                    comboBoxLevel.Items.AddRange(PZ.LEVEL_NAME_TABLE);
                    comboBoxLevel.SelectedIndex = 0;

                    pictureBoxLevel.Image = PZ_Texture.LoadTIM(minimapData, 0);

                    extractToolStripMenuItem.Enabled = true;
                }                          
            }
        }

        private void selectedLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            terrainDir = $"TERRAIN{Path.DirectorySeparatorChar}{PZ.LEVEL_NAME_TABLE[comboBoxLevel.SelectedIndex]}_{comboBoxLevelPack.SelectedIndex + 1}";

            if (!Directory.Exists(terrainDir))
            {
                Directory.CreateDirectory(terrainDir);
            }

            panelProgress.Visible = true;

            if (!backgroundTerrainProcessing.IsBusy)
            {
                backgroundTerrainProcessing.RunWorkerAsync();
            }
        }

        private void comboBoxLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBoxLevel.Image = PZ_Texture.LoadTIM(minimapData, comboBoxLevel.SelectedIndex * PZ.MINIMAP_SZ);
        }

        private void selectedTankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportDir = "TRANSPORT";

            panelProgress.Visible = true;
            transportFlag = false;

            if (!backgroundTransportProcessing.IsBusy)
            {
                backgroundTransportProcessing.RunWorkerAsync();
            }
        }

        private void allTanksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportDir = "TRANSPORT";

            panelProgress.Visible = true;
            transportFlag = true;

            if (!backgroundTransportProcessing.IsBusy)
            {
                backgroundTransportProcessing.RunWorkerAsync();
            }
        }
    }
}
