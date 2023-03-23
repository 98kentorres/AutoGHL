using System;
using System.Collections.Generic;
using System.Text;
using EASendMail;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.VisualStyles;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using static AutoGHL.Login;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Cryptography.Xml;
using System.Drawing.Text;
using System.Data;
using System.Net.Http.Headers;

namespace AutoGHL
{
    public partial class Form1 : Form
    {
        public string info = "";

        string _UserName = LoginInfo.Username;
        string _StoreCode = LoginInfo.StoreCode;
        string _Signature = LoginInfo.Signature;
        string _StoreName = LoginInfo.StoreName;

        string dateNow = DateTime.Now.ToString("MM-d-yyyy");
        string currentYear = DateTime.Now.ToString("yyyy");
        string currentMonth = DateTime.Now.ToString("MMMM");

        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width,
                                   Screen.PrimaryScreen.WorkingArea.Height - this.Height);
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void checkGhlDirectory()
        {
            string dir = @"C:\\AutoGHL\\ghl-pending";
            // If directory does not exist, create it
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public void signatureStatus()
        {
            if (!Directory.Exists(@"\\199.84.20.14\c\\AutoGHL\\signature"))
            {
                lblUserSignature.Text = "Error";
            }
            else
            {
                lblUserSignature.Text = "Done";
            }
        }
        public async Task GenerateReport()
        {
            //create txt log
            string fileName = @"C:\\AutoGHL\\Logs\\" + currentYear + "\\" + currentMonth + "\\" + dateNow + ".txt";
            FileInfo fi = new FileInfo(fileName);

            if (Directory.Exists(@"C:\AutoGHL\ghl-pending"))
            {
                DirectoryInfo dirGHL = new DirectoryInfo(@"C:\AutoGHL\ghl-pending");
                FileInfo[] FilesGHL = dirGHL.GetFiles("*.xlsx");
                var dateNow = DateTime.Now.ToString("MM/d/yyyy");
            Retry:
                try
                {
                    lblGhlReport.Text = "Running";
                    Task.Delay(1500).Wait();
                    lblUserSignature.Text = "Running";
                    //Logs
                    using (StreamWriter sw = fi.AppendText())
                    {
                    sw.WriteLine(DateTime.Now.ToString() + " -> Preparing GHL Report.");
                    sw.WriteLine(DateTime.Now.ToString() + " -> Verifying user signature.");
                    Task.Delay(2500).Wait();
                    SmtpMail oMail = new SmtpMail("TryIt");

                    // Set sender email address, please change it to yours
                    oMail.From = "autoghl.snrshopping@gmail.com";

                    // Set recipient email address, please change it to yours
                    oMail.To = ("Galy Escalante<gescalante@snrshopping.com>");
                    oMail.Cc = ("<snritoperations@googlegroups.com>, Renee Ruth Valentine<rvalentin@snrshopping.com>, Breinan Foronda<bforonda@snrshopping.com>");

                    // Set email subject
                    oMail.Subject = "Automated GHL Pending for " + _StoreName + ". " + dateNow;

                    // Add file attachment
                    foreach (FileInfo fileGHL in FilesGHL)
                    {
                        Attachment ghlFile = oMail.AddAttachment(@"C:\\AutoGHL\\ghl-pending\\" + fileGHL.Name);
                    }

                    // Add image attachment from local disk
                    Attachment oAttachment = oMail.AddAttachment(@"\\199.84.20.14\c\AutoGHL\signature\" + _Signature + ".png");

                    string contentID = "test001@host";
                    oAttachment.ContentID = contentID;

                    // Your SMTP server address
                    SmtpServer oServer = new SmtpServer("smtp.gmail.com");

                    // User and password for ESMTP authentication, if your server doesn't require
                    // User authentication, please remove the following codes.
                    oServer.User = "autoghl.snrshopping@gmail.com";
                    oServer.Password = "sryepnyrtmwnlfas";

                    // Set 25 or 587 port.
                    oServer.Port = 587;

                    // detect TLS connection automatically
                    oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                    sw.WriteLine(DateTime.Now.ToString() + " -> Connecting to smtp server.");
                    DirectoryInfo dir = new DirectoryInfo(@"C:\AutoGHL\ghl-pending");
                    FileInfo[] Files = dir.GetFiles("*.xlsx");
                    if (Files.Length > 0)
                    {
                        Task.Delay(2500).Wait();
                        signatureStatus();
                        // content of email
                        oMail.HtmlBody = "<b>Good Evening</b>, <br><br>" +
                        "Please see attached file, GHL Pending for " + _StoreName + ". <br><br>" +
                        "Thank you, <br><br>" +
                        @"<img src='cid:" + contentID + @"'/>";

                        lblGhlReport.Text = "Done";
                        lblSendGHL.Text = "Running";
                        //Logs
                        sw.WriteLine(DateTime.Now.ToString() + " -> " + _UserName + " signature's verified.");
                        sw.WriteLine(DateTime.Now.ToString() + " -> Done preparing GHL report.");
                        sw.WriteLine(DateTime.Now.ToString() + " -> Sending email to " + oMail.To);
                        sw.WriteLine(DateTime.Now.ToString() + " -> Sending email carbon copy to " + oMail.Cc);
                        Task.Delay(2500).Wait();
                        SmtpClient oSmtp = new SmtpClient();
                        oSmtp.SendMail(oServer, oMail);

                        lblSendGHL.Text = "Done";
                        sw.WriteLine(DateTime.Now.ToString() + " -> Email sent successfully!");
                        MessageBox.Show("email was sent successfully!");
                        this.Close();
                    }
                    else
                    {
                        Task.Delay(2500).Wait();
                        signatureStatus();
                        // content of email
                        oMail.HtmlBody = "<b>Good Evening</b>, <br><br>" +
                        "No GHL pending here at " + _StoreName + ". <br><br>" +
                        "Thank you, <br><br>" +
                        @"<img src='cid:" + contentID + @"'/>";

                        lblGhlReport.Text = "Done";
                        lblSendGHL.Text = "Running";
                        sw.WriteLine(DateTime.Now.ToString() + " -> " + _UserName + " signature's verified.");
                        sw.WriteLine(DateTime.Now.ToString() + " -> Done preparing GHL report.");
                        sw.WriteLine(DateTime.Now.ToString() + " -> Sending email to " + oMail.To);
                        sw.WriteLine(DateTime.Now.ToString() + " -> Sending email carbon copy to " + oMail.Cc);
                        Task.Delay(1500).Wait();
                        Task.Delay(2500).Wait();
                        SmtpClient oSmtp = new SmtpClient();
                        oSmtp.SendMail(oServer, oMail);

                        lblSendGHL.Text = "Done";
                        MessageBox.Show("email was sent successfully!");
                        sw.WriteLine(DateTime.Now.ToString() + " -> Email sent successfully!");
                        this.Close();
                        }
                    }
                }
                catch (Exception ep)
                {
                    MessageBox.Show("failed to send email with the following error: " + ep.Message);
                    //Logs
                    using (StreamWriter sw = fi.AppendText())
                    {
                        sw.WriteLine(DateTime.Now.ToString() + " -> " + "failed to send email with the following error: " + ep.Message);
                        sw.WriteLine(DateTime.Now.ToString() + " -> " + "Rerunning autoghl app");
                    }
                    goto Retry;
                }
            }
            else
            {
                Task.Delay(1500).Wait();
                lblGhlReport.Text = "Running";
                Task.Delay(1500).Wait();
                lblGhlReport.Text = "Error";
                Task.Delay(1500).Wait();
                lblUserSignature.Text = "Error";
                Task.Delay(1500).Wait();
                lblSendGHL.Text = "Error";
                MessageBox.Show("Make sure you have this in your local disk 'C:\\AutoGHL\\ghl-pending'");
                //Logs
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(DateTime.Now.ToString() + " -> " + "failed to proceed: 'c:\\AutoGHL\\ghl-pending' not found!");
                }
                this.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Visible = true;
            //checkGhlDirectory();
            //GenerateReport();
        }

        private void lblUserSignature_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {

        }
    }
}