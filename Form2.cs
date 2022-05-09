using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OracleDataClassGenerator
{
    public partial class Form2 : RibbonForm
    {
        public Form2()
        {
            InitializeComponent();
            Init_Menu();

        }
       
        void Init_Menu()
        {
            //ribbonPanel1.Items.Clear();

            //var btnThoat = new System.Windows.Forms.RibbonButton();
            //btnThoat.Image = global::OracleDataClassGenerator.Properties.Resources.Exit_32;
            //btnThoat.LargeImage = global::OracleDataClassGenerator.Properties.Resources.Exit_32;
            //btnThoat.MaxSizeMode = System.Windows.Forms.RibbonElementSizeMode.Large;
            //btnThoat.MinimumSize = new System.Drawing.Size(60, 0);
            //btnThoat.Name = "btnThoat";
            //btnThoat.Text = "Thoát";
            //btnThoat.TextAlignment = System.Windows.Forms.RibbonItem.RibbonItemTextAlignment.Center;

            

            //var btnLogout = new System.Windows.Forms.RibbonButton();
            //btnLogout.Image = global::OracleDataClassGenerator.Properties.Resources.Logout_32;
            //btnLogout.LargeImage = global::OracleDataClassGenerator.Properties.Resources.Logout_32;
            //btnLogout.MaxSizeMode = System.Windows.Forms.RibbonElementSizeMode.Large;
            //btnLogout.MinimumSize = new System.Drawing.Size(60, 0);
            //btnLogout.Name = "btnLogout";
            //btnLogout.Text = "Đăng xuất";
            //btnLogout.TextAlignment = System.Windows.Forms.RibbonItem.RibbonItemTextAlignment.Center;

       

            //var btnThamSoHT = new System.Windows.Forms.RibbonButton();
            //btnThamSoHT.Image = global::OracleDataClassGenerator.Properties.Resources.Settings_32;
            //btnThamSoHT.LargeImage = global::OracleDataClassGenerator.Properties.Resources.Settings_32;
            //btnThamSoHT.MinimumSize = new System.Drawing.Size(60, 0);
            //btnThamSoHT.Name = "btnThamSoHT";
            //btnThamSoHT.Text = "Tham số hệ thống";
            //btnThamSoHT.TextAlignment = System.Windows.Forms.RibbonItem.RibbonItemTextAlignment.Center;
            

            //var btnMoKy = new System.Windows.Forms.RibbonButton();
            //btnMoKy.Image = global::OracleDataClassGenerator.Properties.Resources.Secrecy_32;
            //btnMoKy.LargeImage = global::OracleDataClassGenerator.Properties.Resources.Secrecy_32;
            //btnMoKy.MinimumSize = new System.Drawing.Size(60, 0);
            //btnMoKy.Name = "btnMoKy";
            //btnMoKy.Text = "Mở số/ Khóa sổ";

            

            //var btnNhatKyHT = new System.Windows.Forms.RibbonButton();
            //btnNhatKyHT.Image = global::OracleDataClassGenerator.Properties.Resources.List_32;
            //btnNhatKyHT.LargeImage = global::OracleDataClassGenerator.Properties.Resources.List_32;
            //btnNhatKyHT.MinimumSize = new System.Drawing.Size(60, 0);
            //btnNhatKyHT.Name = "btnNhatKyHT";
            //btnNhatKyHT.Text = "Nhật ký hệ thống";

            //ribbonPanel1.Items.Add(btnNhatKyHT);
            //ribbonPanel1.Items.Add(btnMoKy);
            //ribbonPanel1.Items.Add(btnThamSoHT);
            //ribbonPanel1.Items.Add(btnThoat);
            //ribbonPanel1.Items.Add(btnLogout);
            
         
           
           
        }
        public bool Form_FindInTab(string formNameOrText)
        {
            bool val = false;
            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                if (this.MdiChildren[i].Name == formNameOrText || this.MdiChildren[i].Text == formNameOrText)
                {
                    val = true;
                    this.MdiChildren[i].BringToFront();
                    break;
                }
            }
            return val;
        }

        void OpenForm(string formName, string formText, bool ShowDialog=false)
        {

            if (!ShowDialog)
            {
                if (!Form_FindInTab(formText))
                {
                    var f = FormFactoy.CreateForm(formName, formText, ShowDialog);
                    f.Show(this.dockPanel1);
                }
            }
            else
            {
                var f = FormFactoy.CreateForm(formName, formText, ShowDialog);
                f.StartPosition = FormStartPosition.CenterParent;
                f.WindowState = FormWindowState.Normal;
                f.ControlBox = true;
                f.MinimizeBox = false;
                f.MaximizeBox = true;
                f.ShowDialog();
            }

        }
        private void ribbonButton7_Click(object sender, EventArgs e)
        {
            OpenForm("Form3","Quản lý người sử dụng");
        }

        private void ribbonButton25_Click(object sender, EventArgs e)
        {
            OpenForm("NhatKyHeThongSearch", " Nhật ký hệ thống");
           
        }
    }
}
