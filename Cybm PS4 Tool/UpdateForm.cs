using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using DevExpress;
using DevExpress.XtraEditors;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTab;
using DevExpress.XtraBars;
using DevExpress.LookAndFeel;

namespace Cybm_PS4_Tool
{
    public partial class UpdateForm : DevExpress.XtraEditors.XtraForm
    {
        public UpdateForm()
        {
            InitializeComponent();
            DevExpress.Skins.SkinManager.EnableFormSkins();
        }

        private void UpdateForm_Load(object sender, EventArgs e)
        {
            
        }

        private void UpdateForm_Shown(object sender, EventArgs e)
        {
            string URL = "https://www.cybermodding.it/PS4tool/";
            string appName = "Payload_Injector_Update.rar";

            WebClient webc = new WebClient();
            webc.DownloadFileAsync(new Uri(URL + appName), appName);
            webc.DownloadFileCompleted += new AsyncCompletedEventHandler(CompletedDown);
            webc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressUpd);
        }

        private void DownloadProgressUpd(object sender, DownloadProgressChangedEventArgs e)
        {
            labelControl4.Text = e.TotalBytesToReceive.ToString();
            labelControl3.Text = e.BytesReceived.ToString();
            progressBarControl1.EditValue = e.ProgressPercentage;
        }

        private void CompletedDown(object sender, AsyncCompletedEventArgs e)
        {
            XtraMessageBox.Show("Download Completed !! \nYou can find the new Update in the Tool Folder.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            System.Threading.Thread.Sleep(1500);
            Application.Exit();
        }
    }
}