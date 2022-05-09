using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OracleDataClassGenerator
{
    public class CurrencyTextBox : TextBox
    {
        public String Format { get; set; }
        public CurrencyTextBox()
        {
            if (String.IsNullOrEmpty(Format))
                Format = "#,##0";

            this.TextAlign = HorizontalAlignment.Right;
            this.KeyUp += CurrencyTextBox_KeyUp;
            this.KeyPress += CurrencyTextBox_KeyPress;
        }

        private void CurrencyTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Cho phép nhập ký tự thập phân, nhập âm
            char decimalSymbol = ',';


            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != decimalSymbol && e.KeyChar != '-')
            {
                e.Handled = true;
            }

            //chỉ được nhập 1 ký tự thập phân
            else if (e.KeyChar == decimalSymbol && (sender as TextBox).Text.IndexOf(decimalSymbol) > -1)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void CurrencyTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V && e.Control)
            {
                string paste = Clipboard.GetText();

                this.Text = ToDouble(this.Text).ToString(this.Format);
            }
            else
                e.SuppressKeyPress = TextBox2Currency((TextBox)sender, e.KeyValue, this.Format);
        }
        private bool TextBox2Currency(TextBox sender, int keyval, string format)
        {
            // MaxLength = 21 (cho phep nhap 17 so 0, bao gom dau [.])
            // KeyValue từ 48 đến 57 tươn đương với các số từ 0 đến 9
            // KeyValue = 44 tương đương với dấu [,]
            // KeyValue = 45 tương đương với dấu [-]
            // KeyValue = 46 tương đương với dấu [.]
            // KeyValue = 8 tương đương với dấu [BackSpace]


            if ((keyval >= 48 && keyval < 58) || keyval == 46 || keyval == 44 || keyval == 45 || keyval == 8)
            {
                //khi gõ dấu thập phân thì xử lý ở đây
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                var decimalSymbol = cultureInfo.NumberFormat.NumberDecimalSeparator;
                if ((sender.Text.Contains(decimalSymbol)))
                {
                    int idx = sender.Text.IndexOf(decimalSymbol);
                    int lenDecimalGroup = sender.Text.Substring(idx + 1).Length;
                    if (lenDecimalGroup >= 1 && format == "#,##0.0")
                    {
                        ConvertCurrencyHasThousandSymbol(sender, format);
                    }
                    else if (lenDecimalGroup >= 2 && format == "#,##0.00")
                    {
                        ConvertCurrencyHasThousandSymbol(sender, format);
                    }
                    else if (lenDecimalGroup >= 3 && format == "#,##0.000")
                    {
                        ConvertCurrencyHasThousandSymbol(sender, format);
                    }
                    else if (lenDecimalGroup >= 4 && format == "#,##0.0000")
                    {
                        ConvertCurrencyHasThousandSymbol(sender, format);
                    }
                }
                else
                {
                    ConvertCurrencyHasThousandSymbol(sender, "#,##0");
                }
            }
            return true;
        }

        private static void ConvertCurrencyHasThousandSymbol(TextBox sender, string format)
        {
            int pos = sender.SelectionStart;
            int oriLen = sender.Text.Length;

            string currTx = "";
            if (sender.Text.Trim().Length > 0)
            {
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                var thousandSymbol = cultureInfo.NumberFormat.NumberGroupSeparator;

                int idxFirstThousandSymbol = sender.Text.IndexOf(thousandSymbol);
                if (idxFirstThousandSymbol == 0)//không có phân c
                    currTx = ToDouble(sender.Text.Substring(1)).ToString(format);
                else
                    currTx = ToDouble(sender.Text).ToString(format);
            }
            int modLen = currTx.Length;
            if (modLen != oriLen)
                pos += (modLen - oriLen);
            sender.Text = currTx;
            if (pos >= 0)
                sender.SelectionStart = pos;
        }

        private static Double ToDouble(object obj)
        {
            string sValue;
            if (obj == null || obj == DBNull.Value)
                sValue = "";
            else
                sValue = obj.ToString();
            
            try
            {
                var culture = new CultureInfo("vi-VN");
                culture.NumberFormat.NumberGroupSeparator = ".";
                culture.NumberFormat.NumberDecimalSeparator = ",";
                return Double.Parse(sValue, culture);
            }
            catch (Exception)
            {
            }


            try
            {
                var culture = new CultureInfo("vi-VN");
                culture.NumberFormat.NumberGroupSeparator = ",";
                culture.NumberFormat.NumberDecimalSeparator = ".";
                return Double.Parse(sValue, culture);

            }
            catch (Exception)
            {
            }

            return 0;
        }
    }
}
