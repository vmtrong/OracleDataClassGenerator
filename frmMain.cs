using Oracle.ManagedDataAccess.Client;
using OracleDataClassGenerator.Engine;
using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace OracleDataClassGenerator
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            txtIP.Text = System.Configuration.ConfigurationManager.AppSettings["IP2"];
            txtSID.Text = System.Configuration.ConfigurationManager.AppSettings["SID2"];
            txtUserName.Text = System.Configuration.ConfigurationManager.AppSettings["UserName2"];
            txtPassword.Text = System.Configuration.ConfigurationManager.AppSettings["Password2"];

            statusBar1.Text = "";

            EnableButtonConnect(true);
            EnableButtonDisconnect(false);
        }
        void EnableButtonConnect(bool enable)
        {
            btnConnect.Enabled = enable;
            panelConn.Enabled = enable;
        }
        void EnableButtonDisconnect(bool enable)
        {
            panelGen.Enabled = enable;
            btnDisconnect.Enabled = enable;
            btnShowData.Enabled = enable;
            button2.Enabled = enable;
            button3.Enabled = enable;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            EnableButtonConnect(false);

            statusBar1.Text = "Connecting ...";

            Thread t = new Thread(new ThreadStart(ConnectDb));
            t.IsBackground = true;
            t.Start();

        }
        void ConnectDb()
        {
            OracleConnection conn = null;
            try
            {
                AppUtil.ConnectionString = string.Format("Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = "
                 + txtIP.Text + ")(PORT = " + "1521" + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = "
                 + txtSID.Text + ")));Password=" + txtPassword.Text + ";User ID=" + txtUserName.Text);
                conn = new OracleConnection(AppUtil.ConnectionString);
                conn.Open();

                //Get tables
                var sql = "SELECT table_name FROM all_tables WHERE owner = :owner ORDER BY owner, table_name";
                DataTable dtTables = new DataTable();
                var cmd = new OracleCommand(sql);
                cmd.Parameters.Add("owner", OracleDbType.Varchar2).Value = txtUserName.Text.ToUpper();
                cmd.Connection = conn;
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                da.Fill(dtTables);

                //Get packages
                sql = "SELECT object_name FROM USER_OBJECTS WHERE OBJECT_TYPE='PACKAGE'";
                DataTable dtPackages = new DataTable();
                cmd = new OracleCommand(sql);
                cmd.Connection = conn;
                da = new OracleDataAdapter(cmd);
                da.Fill(dtPackages);

                comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
                Invoke(new ThreadStart(delegate
                {
                    comboBox1.DisplayMember = "table_name";
                    comboBox1.ValueMember = "table_name";
                    comboBox1.DataSource = dtTables.DefaultView;
                    
                    TableChanged();
                    comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

                    cboPackageList.DisplayMember = "object_name";
                    cboPackageList.ValueMember = "object_name";
                    cboPackageList.DataSource = dtPackages.DefaultView;


                    EnableButtonConnect(false);
                    EnableButtonDisconnect(true);
                    button1.PerformClick();
                    
                }));
                
            }
            catch (Exception ex)
            {
                Invoke(new MethodInvoker(delegate
                {
                    statusBar1.Text = "Connect error.";
                    EnableButtonConnect(true);
                    EnableButtonDisconnect(false);
                }));
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TableChanged();
        }
        private void cboPackageList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void TableChanged()
        {
            AppUtil.TableName = comboBox1.SelectedValue.ToString();
            DataTable tb = Generator.GetTableSchema(AppUtil.TableName);
            txtClassInfo.Text = AppUtil.TableName;
            txtClassAccess.Text = AppUtil.TableName + "Dao";
            statusBar1.Text = "Table: " + AppUtil.TableName + "    " + "NumbersField: " + tb.Rows.Count;

            Definition.NameSpaceDefine = textBox1.Text;
            Definition.ClassInfoDefine = txtClassInfo.Text;
            Definition.ClassAccessDefine = txtClassAccess.Text;

            // DataTable tb = Generator.GetTableSchema(comboBox1.Text);
            txtDataClass.Text = Generator.ToContainerClass(tb);
            richTextBox2.Text = Generator.ToAccessClass(tb);

            DataTable _table = Generator.GetAll_Tab_Columns(AppUtil.TableName,txtUserName.Text);

            richTextBox3.Text = Generator.ToPackage(_table);
            //tabControl1.SelectedTab = tabPage3;
           
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            EnableButtonConnect(true);
            EnableButtonDisconnect(false);
            statusBar1.Text = "";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtIP.Text = System.Configuration.ConfigurationManager.AppSettings["IP1"];
                txtSID.Text = System.Configuration.ConfigurationManager.AppSettings["SID1"]; 
                txtUserName.Text = System.Configuration.ConfigurationManager.AppSettings["UserName1"];
                txtPassword.Text = System.Configuration.ConfigurationManager.AppSettings["Password1"];
            }
            else
            {
                txtIP.Text = System.Configuration.ConfigurationManager.AppSettings["IP2"];
                txtSID.Text = System.Configuration.ConfigurationManager.AppSettings["SID2"];
                txtUserName.Text = System.Configuration.ConfigurationManager.AppSettings["UserName2"];
                txtPassword.Text = System.Configuration.ConfigurationManager.AppSettings["Password2"];
            }
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
            txtPassword.Text = txtUserName.Text;
        }

  

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == ">")
            {
                panel2.Dock = DockStyle.Left;
                button1.Text = "<";
                groupBox1.Height = 180;
            }
            else
            {
                button1.Text = ">";
                panel2.Dock = DockStyle.None;
                groupBox1.Height = 140;
            }
                
        }

        private void btnShowData_Click(object sender, EventArgs e)
        {
            btnShowData.Enabled = false;
            try
            {
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = Generator.GetTableData();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                btnShowData.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            try
            {
                txtSchema.Text = Generator.ToSchama();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                button2.Enabled = true;
            }

          
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            try
            {
                txtPackageContent.Text = Generator.ToPackage(cboPackageList.SelectedValue.ToString());
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                button3.Enabled = true;
            }
            
        }
    }
}
