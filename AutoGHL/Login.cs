using System;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Diagnostics;
using static System.Net.WebRequestMethods;
using Microsoft.VisualBasic;

namespace AutoGHL
{
    public partial class Login : Form
    {
        SqlConnection con = new SqlConnection("Data Source=199.84.1.201;Initial Catalog=testDb;Persist Security Info=True;User ID=sa;Password=@dm1n@8800;MultipleActiveResultSets=true");
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;

        string dateNow = DateTime.Now.ToString("MM-d-yyyy");
        string currentYear = DateTime.Now.ToString("yyyy");
        string currentMonth = DateTime.Now.ToString("MMMM");

        public Login()
        {
            InitializeComponent();
        }

        public static class LoginInfo
        {
            public static string Username;
            public static string StoreCode;
            public static string Signature;
            public static string StoreName;
        }
         
        public async Task AutoLogin()
        {
            if (System.IO.File.Exists(@"C:\\AutoGHL\\config.x.txt"))
            {
                var data = System.IO.File.ReadLines(@"C:\\AutoGHL\\config.x.txt");

                txtUsername.Text = data.ToArray()[0];
                txtPassword.Text = data.ToArray()[1];
                await Task.Delay(1000);
                SendKeys.SendWait("{TAB}");
                await Task.Delay(1000);
                SendKeys.SendWait("{ENTER}");
            }
            else
            {
                MessageBox.Show("config.x not found in 'C:\\AutoGHL'");
            }
        }

        private void checkLogDirectory()
        {
            string dir = @"C:\\AutoGHL\\Logs\\" + currentYear + "\\" + currentMonth;
            // If directory does not exist, create it
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        private void Login_Load(object sender, EventArgs e)
        {
            checkLogDirectory();
            timer1.Start();
            //create txt log
            string fileName = @"C:\\AutoGHL\\Logs\\" + currentYear + "\\" + currentMonth + "\\" + dateNow + ".txt";
            FileInfo fi = new FileInfo(fileName);
            try
            {
                var date = DateTime.Now;
                if (date.Hour >= 20)
                { 
                    // Check if file already exists. If yes, append text.     
                    if (fi.Exists)
                    {
                        // Appending the given texts
                        using (StreamWriter sw = fi.AppendText())
                        {
                            sw.WriteLine(DateTime.Now.ToString() + " -> Open AutoGHL App.");
                            timer1.Start();
                        }
                    }
                    else
                    {
                        // Create a new file     
                        using (StreamWriter sw = fi.CreateText())
                        {
                            sw.WriteLine(DateTime.Now.ToString() + " -> Open AutoGHL App.");
                            timer1.Start();
                        }
                    }
                }
                else
                {
                    // Check if file already exists. If yes, append text.     
                    if (fi.Exists)
                    {
                        // Appending the given texts
                        using (StreamWriter sw = fi.AppendText())
                        {
                            sw.WriteLine(DateTime.Now.ToString() + " -> Unable to run AutoGHL, Please re-run app around 8PM!");
                            MessageBox.Show("Unable to run AutoGHL, Please re-run app around 8PM!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.Close();
                        }
                    }
                    else
                    {
                        // Create a new file     
                        using (StreamWriter sw = fi.CreateText())
                        {
                            sw.WriteLine(DateTime.Now.ToString() + " -> Unable to run AutoGHL, Please re-run app around 8PM!.");
                        }
                        MessageBox.Show("Unable to run AutoGHL, Please re-run app around 8PM!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Close();
                    } 
                } 
            }
            catch (Exception Ex)
            {
                MessageBox.Show("" + Ex);
            } 
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            //create txt log
            string fileName = @"C:\\AutoGHL\\Logs\\" + currentYear + "\\" + currentMonth + "\\"+dateNow+".txt";
            FileInfo fi = new FileInfo(fileName);
            try
            {
                // Appending the given texts
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(DateTime.Now.ToString() + " -> Attempting to login as " + txtUsername.Text);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("" + Ex);
            }

            int count = 0;
            if (txtUsername.Text != "")
            {
                if (txtPassword.Text != "")
                {
                    con.Open();
                    cmd = new SqlCommand("SELECT * from tblUserAccess where username = @username and password = @password", con);
                    cmd.Parameters.AddWithValue("username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("password", txtPassword.Text);
                    cmd.Parameters.AddWithValue("id", count);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        IPHostEntry host;
                        string localIP = "?";
                        host = Dns.GetHostEntry(Dns.GetHostName());

                        foreach (IPAddress ip in host.AddressList)
                        {
                            if (ip.AddressFamily.ToString() == "InterNetwork")
                            {
                                localIP = ip.ToString();
                                SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM tblUserAccess WHERE username='" + txtUsername.Text + "'", con);
                                DataTable dt = new DataTable(); //this is creating a virtual table
                                sda.Fill(dt);
                                string username = dt.Rows[count][1].ToString();
                                string StoreCode = dt.Rows[count][3].ToString();
                                string StoreName = dt.Rows[count][4].ToString();
                                string signature = dt.Rows[count][5].ToString();

                                if (localIP == "199.84.2.14" || localIP == "199.84.2.13" || localIP == "199.84.2.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = "8801";
                                    LoginInfo.StoreName = "BGC";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if (localIP == "199.84.3.14" || localIP == "199.84.3.13" || localIP == "199.84.3.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = "8803";
                                    LoginInfo.StoreName = "Congressional";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if (localIP == "199.84.4.14" || localIP == "199.84.4.13" || localIP == "199.84.4.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = "8804";
                                    LoginInfo.StoreName = "Alabang";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if (localIP == "199.84.5.14" || localIP == "199.84.5.13" || localIP == "199.84.5.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = "8805";
                                    LoginInfo.StoreName = "Aseana";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if (localIP == "199.84.6.14" || localIP == "199.84.6.13" || localIP == "199.84.6.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = "8806";
                                    LoginInfo.StoreName = "Cebu";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if(localIP == "199.84.7.14" || localIP == "199.84.7.13" || localIP == "199.84.7.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = "8807";
                                    LoginInfo.StoreName = "Shaw";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if(localIP == "199.84.8.14" || localIP == "199.84.8.13" || localIP == "199.84.8.12"){
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = "8808";
                                    LoginInfo.StoreName = "San Fernando, Pampanga";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if (localIP == "199.84.9.14" || localIP == "199.84.9.13" || localIP == "199.84.9.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = "8809";
                                    LoginInfo.StoreName = "Davao";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if(localIP == "199.84.10.14" || localIP == "199.84.10.13" || localIP == "199.84.10.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = "8810";
                                    LoginInfo.StoreName = "Imus, Cavite";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show(); 
                                }
                                else if (localIP == "199.84.11.14" || localIP == "199.84.11.13" || localIP == "199.84.11.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = StoreCode;
                                    LoginInfo.StoreName = "Nuvali";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if (localIP == "199.84.12.14" || localIP == "199.84.12.13" || localIP == "199.84.12.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = StoreCode;
                                    LoginInfo.StoreName = "CDO";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if (localIP == "199.84.13.14" || localIP == "199.84.13.13" || localIP == "199.84.13.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = StoreCode;
                                    LoginInfo.StoreName = "ILOILO";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if (localIP == "199.84.14.14" || localIP == "199.84.14.13" || localIP == "199.84.14.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = StoreCode;
                                    LoginInfo.StoreName = "Cabanatuan";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if(localIP == "199.84.15.14" || localIP == "199.84.15.13" || localIP == "199.84.15.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = StoreCode;
                                    LoginInfo.StoreName = "COMMONWEALTH";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if (localIP == "199.84.16.14" || localIP == "199.84.16.13" || localIP == "199.84.16.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = StoreCode;
                                    LoginInfo.StoreName = "Dau";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if (localIP == "199.84.17.14" || localIP == "199.84.17.13" || localIP == "199.84.17.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = StoreCode;
                                    LoginInfo.StoreName = "Paranaque";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if (localIP == "199.84.18.14" || localIP == "199.84.18.13" || localIP == "199.84.18.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = StoreCode;
                                    LoginInfo.StoreName = "LIPA";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if(localIP == "199.84.19.14" || localIP == "199.84.19.13" || localIP == "199.84.19.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = StoreCode;
                                    LoginInfo.StoreName = "Libis";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if(localIP == "199.84.20.14" || localIP == "199.84.20.13" || localIP == "199.84.20.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = StoreCode;
                                    LoginInfo.StoreName = "Circuit, Makati";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if(localIP == "199.84.21.14" || localIP == "199.84.21.12" || localIP == "199.84.21.13")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = StoreCode;
                                    LoginInfo.StoreName = "New Manila";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if (localIP == "199.84.22.14" || localIP == "199.84.22.13" || localIP == "199.84.22.12")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = StoreCode;
                                    LoginInfo.StoreName = "Sucat";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                else if (localIP == "199.84.23.14" || localIP == "199.84.23.12" || localIP == "199.84.23.13")
                                {
                                    LoginInfo.Username = username;
                                    LoginInfo.Signature = signature;
                                    LoginInfo.StoreCode = StoreCode;
                                    LoginInfo.StoreName = "Marikina";
                                    Form1 frm = new Form1();
                                    this.Hide();
                                    frm.Show();
                                }
                                //create txt log
                                try
                                {
                                    // Appending the given texts
                                    using (StreamWriter sw = fi.AppendText())
                                    {
                                        sw.WriteLine(DateTime.Now.ToString() + " -> Successfully logged in as " + txtUsername.Text);
                                    }
                                }
                                catch (Exception Ex)
                                {
                                    MessageBox.Show("" + Ex);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Incorrect username or password!");
                        //create txt log
                        try
                        {
                            // Appending the given texts
                            using (StreamWriter sw = fi.AppendText())
                            {
                                sw.WriteLine(DateTime.Now.ToString() + " -> Failed to login using " + txtUsername.Text);
                            }
                        }
                        catch (Exception Ex)
                        {
                            MessageBox.Show("" + Ex);
                        }
                        txtUsername.Text = "";
                        txtPassword.Text = "";
                    }
                    con.Close();
                }
                else
                {
                    MessageBox.Show("Please insert your password!");
                }
            }
            else
            {
                MessageBox.Show("Please insert your username!");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int loading = 0;
            if (progressBar1.Value < 100)
            {
                progressBar1.Value += 1;
            }
            else
            {
                timer1.Stop();
                AutoLogin(); 

                //create txt log
                string fileName = @"C:\\AutoGHL\\Logs\\" + currentYear + "\\" + currentMonth + "\\" + dateNow + ".txt";
                FileInfo fi = new FileInfo(fileName);
                try
                {
                    // Check if file already exists. If yes, append text.     
                    if (fi.Exists)
                    {
                        // Appending the given texts
                        using (StreamWriter sw = fi.AppendText())
                        {
                            sw.WriteLine(DateTime.Now.ToString() + " -> Open AutoGHL App.");
                        }
                    }
                    else
                    {
                        // Create a new file     
                        using (StreamWriter sw = fi.CreateText())
                        {
                            sw.WriteLine(DateTime.Now.ToString() + " -> Open AutoGHL App.");
                        }
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("" + Ex);
                    // Check if file already exists. If yes, append text.     
                    if (fi.Exists)
                    {
                        // Appending the given texts
                        using (StreamWriter sw = fi.AppendText())
                        {
                            sw.WriteLine(DateTime.Now.ToString() + " -> App has closed.");
                        }
                    }
                    else
                    {
                        // Create a new file     
                        using (StreamWriter sw = fi.CreateText())
                        {
                            sw.WriteLine(DateTime.Now.ToString() + " -> App has closed");
                        }
                    }
                    this.Close();
                }
            }
        }
    }
}
