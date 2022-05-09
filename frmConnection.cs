
using Oracle.ManagedDataAccess.Client;
using OracleDataClassGenerator.Engine;
using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace OracleDataClassGenerator
{
    public partial class frmConnection : Form
    {
        //Delegate of type Action<string>
        private Action<string> statusDelegate;
        public Action<DataTable> TableDelegate { get; set; }
        public frmConnection(Action<string> theDelegate)
        {
            InitializeComponent();
            this.statusDelegate = theDelegate;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            this.statusDelegate("Connecting ...");

            Thread t = new Thread(new ThreadStart(ConnectDb));
            t.IsBackground = true;
            t.Start();

        }
        void ConnectDb()
        {
            OracleConnection conn = null;
            try
            {
                AppUtil.ConnectionString = string.Format("Data Source={0}/{1};User Id={2}; Password={3}", txtIP.Text, txtSID.Text, txtUserName.Text, txtPassword.Text);
                conn = new OracleConnection(AppUtil.ConnectionString);
                conn.Open();

                var sql = "SELECT table_name FROM all_tables WHERE owner = '" + txtUserName.Text.ToUpper() + "' ORDER BY owner, table_name";
                DataTable dt = new DataTable();
                OracleDataAdapter da = new OracleDataAdapter(sql, conn);
                da.Fill(dt);

                this.statusDelegate("Connected.");
                this.TableDelegate(dt);
                Invoke(new ThreadStart(delegate
                {
                    this.Close();

                }));

            }
            catch (Exception ex)
            {
                this.statusDelegate("Connect error.");
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
                
            }
        }
    }
}
