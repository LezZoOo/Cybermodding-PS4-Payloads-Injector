using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading.Tasks;
using DevExpress;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;

namespace Cybm_PS4_Tool
{
    public partial class Main : DevExpress.XtraEditors.XtraForm
    {
        public Main()
        {
            InitializeComponent();
            DevExpress.Skins.SkinManager.EnableFormSkins();
        }

        #region definitions
        public static Socket sock;
        WebClient wc = new WebClient();
        private static bool Connected;
        private static int Language = 0;
        private static int Firm = 0;
        private static bool dotsrunning = false;
        private static bool secondstart = false;
        public static CancellationTokenSource tokenSource_0 = new CancellationTokenSource();
        public static CancellationToken token_0 = tokenSource_0.Token;
        private static string link455 = "https://www.cybermodding.it/PS4tool/455/";
        private static string link505 = "https://www.cybermodding.it/PS4tool/505/";
        private static string link672 = "https://www.cybermodding.it/PS4tool/672/";
        private static string pllink455 = "https://www.cybermodding.it/PS4tool/455/455-payloads-list.txt";
        private static string pllink505 = "https://www.cybermodding.it/PS4tool/505/505-payloads-list.txt";
        private static string pllink672 = "https://www.cybermodding.it/PS4tool/672/672-payloads-list.txt";
        private static string dllink455 = "https://www.cybermodding.it/PS4tool/455/455-downloads-list.txt";
        private static string dllink505 = "https://www.cybermodding.it/PS4tool/505/505-downloads-list.txt";
        private static string dllink672 = "https://www.cybermodding.it/PS4tool/672/672-downloads-list.txt";
        private static string[] payloads455;
        private static string[] payloads505;
        private static string[] payloads672;
        private static string[] downloads455;
        private static string[] downloads505;
        private static string[] downloads672;

        private bool Connect(string ip)
        {
            try
            {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.ReceiveTimeout = 3000;
                sock.SendTimeout = 3000;
                sock.Connect(new IPEndPoint(IPAddress.Parse(ip), 9020));
                Connected = true;
                return true;
            }
            catch (Exception ex)
            {
                Connected = false;
                if (Language == 0)
                    XtraMessageBox.Show("Something doesen't work. \nBe sure to launch the Exploit with the BIN Loader/Netcat function before trying to Connect the Tool. \nHere is the generated exception:" + "\n\n" +  ex.Message, "Error on Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (Language == 1)
                    XtraMessageBox.Show("Qualcosa non va. \nAssicurati di aver avviato l'Exploit con la funzione BIN Loader/Netcat prima di provare a Connettere il Tool. \nEcco l'eccezione generata:" + "\n\n" + ex.Message, "Errore nella Connessione", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void SendPayload(string file)
        {
            try
            {
                sock.SendFile(file);
                Disconnect();
                barStaticItem1.Caption = "Payload injected !!";
                pictureBox2.Visible = false;
                pictureBox1.Visible = true;
            }
            catch (Exception ex)
            {
                barStaticItem1.Caption = "Error while Injecting ...";
                pictureBox1.Visible = false;
                pictureBox2.Visible = true;
                XtraMessageBox.Show("Error while inecting the payload.\nCheck if the Exploit is still running on the PS4.\n\nHere is the exception generated:\n" + ex.Message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Disconnect()
        {
            Connected = false;
            sock.Close();
            secondstart = true;
        }

        delegate void SetPics(bool vis, PictureBox pic);

        private void SetPictures(bool vis, PictureBox pic)
        {
            if (tokenSource_0.IsCancellationRequested == false)
            {
                try
                {
                    if (pic.InvokeRequired)
                    {
                        SetPics d = new SetPics(SetPictures);
                        Invoke(d, new object[] { vis, pic });
                    }
                    else
                    {
                        pic.Visible = vis;
                    }
                }
                catch
                {

                }
            }
        }

        private void TaskPic(bool toggle)
        {
            if (toggle == false)
            {
                dotsrunning = false;
                SetPictures(false, pictureBox3);
                SetPictures(false, pictureBox4);
                SetPictures(false, pictureBox5);
                SetPictures(false, pictureBox6);
                tokenSource_0.Cancel();
                dotsrunning = false;
            }
            if (toggle == true)
            {
                if (tokenSource_0.IsCancellationRequested == true)
                {
                    tokenSource_0 = new CancellationTokenSource();
                    token_0 = tokenSource_0.Token;
                }
                Task t1 = Task.Factory.StartNew(() =>
                {
                    while (!token_0.IsCancellationRequested)
                    {
                        if (!dotsrunning)
                        {
                            dotsrunning = true;
                            SetPictures(false, pictureBox1);
                            SetPictures(false, pictureBox2);
                        }
                        
                        Thread.Sleep(750);
                        SetPictures(true, pictureBox3);
                        Thread.Sleep(750);
                        SetPictures(true, pictureBox4);
                        Thread.Sleep(750);
                        SetPictures(true, pictureBox5);
                        Thread.Sleep(750);
                        SetPictures(true, pictureBox6);
                        Thread.Sleep(750);
                        SetPictures(false, pictureBox3);
                        SetPictures(false, pictureBox4);
                        SetPictures(false, pictureBox5);
                        SetPictures(false, pictureBox6);
                    }
                }, token_0);
            }
        }

        private async Task DownloadPayload(string url, string flnm)
        {
            await wc.DownloadFileTaskAsync(url, flnm);
        }
        #endregion

        #region updates
        public void CheckforUpdates()
        {
            string URL = "https://www.cybermodding.it/PS4tool/";
            string Version = "Version.txt";
            string Change = "Changelog.txt";
            string Uploading = "Uploading.txt";

            try
            {
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == true)
                {
                    WebClient webC = new WebClient();
                    var UPL = webC.DownloadString(URL + Uploading);
                    var CH = webC.DownloadString(URL + Change);
                    var UI = webC.DownloadString(URL + Version);
                    if (UI.Contains("v1.0.2"))
                    {

                    }
                    else
                    {
                        if (UPL.Contains("No"))
                        {
                            if (XtraMessageBox.Show("A new Update is Available!! \nVersion: PS4 Payloads Injector " + UI + "\n \n" + CH + "\n \n" + "Would you like to Download the Update now?", "New Update", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                            {
                                this.Hide();
                                UpdateForm UpdFrm = new UpdateForm();
                                UpdFrm.Closed += (s, args) => this.Close();
                                UpdFrm.Show();
                            }
                            else
                            {

                            }
                        }
                        else
                        {

                        }
                    }
                }
            }
            catch
            {

            }
        }

        public void Check4Updates()
        {
            string URL = "https://www.cybermodding.it/PS4tool/";
            string ToolVersion = "Version.txt";
            string Change = "Changelog.txt";
            string Uploading = "Uploading.txt";

            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == true)
            {
                WebClient webC = new WebClient();
                var UPL = webC.DownloadString(URL + Uploading);
                var UI = webC.DownloadString(URL + ToolVersion);
                var CH = webC.DownloadString(URL + Change);

                if (UI.Contains("v1.0.2"))
                {
                    XtraMessageBox.Show("Current Version: " + UI + "\nNo Updates Available for now. Retry again later..", "No Updates", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    if (UPL.Contains("No"))
                    {
                        Application.DoEvents();
                        if (XtraMessageBox.Show("A new Update is Available!! \nVersion: PS4 Payloads Injector " + UI + "\n \n" + CH + "\n \n" + "Would you like to Download the Update now?", "New Update", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                        {
                            this.Hide();
                            UpdateForm UpdFrm = new UpdateForm();
                            UpdFrm.Show();
                        }
                        else
                        {
                            
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Uploading a new version right now. \nWait some seconds and retry. You will find a new Version! :)", "Uploading", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            }
            else
            {
                XtraMessageBox.Show("Your Internet connection is not working, or another error has occured.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void Main_Load(object sender, EventArgs e)
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == true)
            {
                WebClient wc = new WebClient();
                if (File.Exists("settings.ini"))
                {
                    string[] lines = File.ReadAllLines("settings.ini");
                    textEdit1.Text = lines[0];
                    if (lines[1] == "455")
                    {
                        Firm = 0;
                        Directory.CreateDirectory(Path.GetTempPath() + "ps4tool");
                        File.Create(Path.GetTempPath() + "ps4tool/455-payloads-list.txt");
                        File.Create(Path.GetTempPath() + "ps4tool/455-downloads-list.txt");
                    }
                    else if (lines[1] == "505")
                    {
                        Firm = 1;
                        Directory.CreateDirectory(Path.GetTempPath() + "ps4tool");
                        File.Create(Path.GetTempPath() + "ps4tool/505-payloads-list.txt");
                        File.Create(Path.GetTempPath() + "ps4tool/505-downloads-list.txt");
                    }
                    else if (lines[1] == "672")
                    {
                        Firm = 2;
                        Directory.CreateDirectory(Path.GetTempPath() + "ps4tool");
                        File.Create(Path.GetTempPath() + "ps4tool/672-payloads-list.txt");
                        File.Create(Path.GetTempPath() + "ps4tool/672-downloads-list.txt");
                    }
                }
                else
                {
                    Firm = 2;
                    textEdit1.Text = "192.168.1.01";
                    Directory.CreateDirectory(Path.GetTempPath() + "ps4tool");
                    File.Create(Path.GetTempPath() + "ps4tool/672-payloads-list.txt");
                    File.Create(Path.GetTempPath() + "ps4tool/672-downloads-list.txt");
                }
            }
            else
            {
                XtraMessageBox.Show("Internet connection needed.. \nEnable a connection then open the tool :)", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(10);
            }
        }

        private async void Main_Shown(object sender, EventArgs e)
        {
            WebClient wc = new WebClient();
            await DownloadPayload(pllink455, Path.GetTempPath() + "ps4tool/455-payloads-list.txt");
            await DownloadPayload(dllink455, Path.GetTempPath() + "ps4tool/455-downloads-list.txt");
            payloads455 = File.ReadAllLines(Path.GetTempPath() + "ps4tool/455-payloads-list.txt");
            downloads455 = File.ReadAllLines(Path.GetTempPath() + "ps4tool/455-downloads-list.txt");
            await DownloadPayload(pllink505, Path.GetTempPath() + "ps4tool/505-payloads-list.txt");
            await DownloadPayload(dllink505, Path.GetTempPath() + "ps4tool/505-downloads-list.txt");
            payloads505 = File.ReadAllLines(Path.GetTempPath() + "ps4tool/505-payloads-list.txt");
            downloads505 = File.ReadAllLines(Path.GetTempPath() + "ps4tool/505-downloads-list.txt");
            await DownloadPayload(pllink672, Path.GetTempPath() + "ps4tool/672-payloads-list.txt");
            await DownloadPayload(dllink672, Path.GetTempPath() + "ps4tool/672-downloads-list.txt");
            payloads672 = File.ReadAllLines(Path.GetTempPath() + "ps4tool/672-payloads-list.txt");
            downloads672 = File.ReadAllLines(Path.GetTempPath() + "ps4tool/672-downloads-list.txt");

            if (Firm == 0)
                checkEdit1.Checked = true;
            else if (Firm == 1)
                checkEdit2.Checked = true;
            else if (Firm == 2)
                checkEdit3.Checked = true;

            if (checkEdit1.Checked)
            {
                comboBoxEdit1.Properties.Items.Clear();
                for (int i = 0; i < payloads455.Length; i++)
                    comboBoxEdit1.Properties.Items.Add(payloads455[i]);

                comboBoxEdit1.SelectedIndex = 0;
            }
            else if (checkEdit2.Checked)
            {
                comboBoxEdit1.Properties.Items.Clear();
                for (int i = 0; i < payloads505.Length; i++)
                    comboBoxEdit1.Properties.Items.Add(payloads505[i]);

                comboBoxEdit1.SelectedIndex = 0;
            }
            else if (checkEdit3.Checked)
            {
                comboBoxEdit1.Properties.Items.Clear();
                for (int i = 0; i < payloads672.Length; i++)
                    comboBoxEdit1.Properties.Items.Add(payloads672[i]);

                comboBoxEdit1.SelectedIndex = 0;
            }
            TaskPic(true);
            CheckforUpdates();
        }

        private void Main_Closing(object sender, FormClosingEventArgs e)
        {
            TaskPic(false);

            if (Connected)
                Disconnect();

            if (File.Exists("settings.ini"))
            {
                File.Delete("settings.ini");
            }

            string[] lines = new string[2];
            lines[0] = textEdit1.Text;
            if (checkEdit1.Checked)
                lines[1] = "455";
            else if (checkEdit2.Checked)
                lines[1] = "505";
            else if (checkEdit3.Checked)
                lines[1] = "672";

            File.WriteAllLines("settings.ini", lines);

            foreach (string filen in Directory.GetFiles(Path.GetTempPath() + "ps4tool").Select(Path.GetFileName))
            {
                File.Delete(Path.GetTempPath() + "ps4tool/" + filen);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (checkEdit1.Checked || checkEdit2.Checked || checkEdit3.Checked)
            {
                if (Connect(textEdit1.Text))
                {
                    if (Language == 0)
                        barStaticItem1.Caption = "Connected to your PS4 !!";
                    else if (Language == 1)
                        barStaticItem1.Caption = "Connesso alla tua PS4 !!";

                    simpleButton2.Enabled = true;
                    simpleButton3.Enabled = true;
                }
                else
                {
                    if (Language == 0)
                        barStaticItem1.Caption = "Error with the Connection ..";
                    else if (Language == 1)
                        barStaticItem1.Caption = "Errore nella Connessione ..";

                    simpleButton2.Enabled = false;
                    simpleButton3.Enabled = false;
                }

                if (secondstart && !dotsrunning)
                    TaskPic(true);
            }
            else if (!checkEdit1.Checked && !checkEdit2.Checked && !checkEdit3.Checked)
            {
                XtraMessageBox.Show("Select your Firmware version first to make sure to inject a compatible payload.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit1.Checked)
            {
                checkEdit2.Checked = false;
                checkEdit3.Checked = false;
                Firm = 0;
            }
            
            comboBoxEdit1.Properties.Items.Clear();
            for (int i = 0; i < payloads455.Length; i++)
                comboBoxEdit1.Properties.Items.Add(payloads455[i]);

            comboBoxEdit1.SelectedIndex = 0;
        }

        private void checkEdit2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit2.Checked)
            {
                checkEdit1.Checked = false;
                checkEdit3.Checked = false;
                Firm = 1;
            }

            comboBoxEdit1.Properties.Items.Clear();
            for (int i = 0; i < payloads505.Length; i++)
                comboBoxEdit1.Properties.Items.Add(payloads505[i]);

            comboBoxEdit1.SelectedIndex = 0;
        }

        private void checkEdit3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit3.Checked)
            {
                checkEdit1.Checked = false;
                checkEdit2.Checked = false;
                Firm = 2;
            }

            comboBoxEdit1.Properties.Items.Clear();
            for (int i = 0; i < payloads672.Length; i++)
                comboBoxEdit1.Properties.Items.Add(payloads672[i]);

            comboBoxEdit1.SelectedIndex = 0;
        }

        private async void simpleButton2_Click(object sender, EventArgs e)
        {
            if (Connected)
            {
                TaskPic(false);
                if (checkEdit1.Checked)
                {
                    barStaticItem1.Caption = "Working ...";
                    await DownloadPayload(link455 + downloads455[comboBoxEdit1.SelectedIndex], Path.GetTempPath() + "ps4tool/" + downloads455[comboBoxEdit1.SelectedIndex]);
                    Thread.Sleep(500);
                    SendPayload(Path.GetTempPath() + "ps4tool/" + downloads455[comboBoxEdit1.SelectedIndex]);
                }
                else if (checkEdit2.Checked)
                {
                    barStaticItem1.Caption = "Working ...";
                    await DownloadPayload(link505 + downloads505[comboBoxEdit1.SelectedIndex], Path.GetTempPath() + "ps4tool/" + downloads505[comboBoxEdit1.SelectedIndex]);
                    Thread.Sleep(500);
                    SendPayload(Path.GetTempPath() + "ps4tool/" + downloads505[comboBoxEdit1.SelectedIndex]);
                }
                else if (checkEdit3.Checked)
                {
                    barStaticItem1.Caption = "Working ...";
                    await DownloadPayload(link672 + downloads672[comboBoxEdit1.SelectedIndex], Path.GetTempPath() + "ps4tool/" + downloads672[comboBoxEdit1.SelectedIndex]);
                    Thread.Sleep(500);
                    SendPayload(Path.GetTempPath() + "ps4tool/" + downloads672[comboBoxEdit1.SelectedIndex]);
                }
            }
            else
            {
                XtraMessageBox.Show("You have to connect first..", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (Connected)
            {
                TaskPic(false);
                OpenFileDialog dialogf = new OpenFileDialog
                {
                    Filter = "BIN File|*.bin",
                    Title = "PS4 Payload Injector",
                    InitialDirectory = Application.StartupPath
                };
                if (dialogf.ShowDialog() == DialogResult.OK)
                {
                    string filen = dialogf.FileName;
                    TaskPic(false);
                    barStaticItem1.Caption = "Working ...";
                    SendPayload(filen);
                }
            }
            else
            {
                XtraMessageBox.Show("You have to connect first..", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            Application.Restart();
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            Application.Exit();
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            Check4Updates();
        }
    }
}
