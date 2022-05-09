using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OracleDataClassGenerator
{
    public class FormFactoy
    {
        public static frmBase CreateForm(string formName, string formText, bool ShowDialog)
        {
            var f = new frmBase();
            if (formName == "Form3")
            {
               // f = new Form3 { Text = formText, TabText = formText };
            }
            if (formName == "NhatKyHeThongSearch")
            {
               // f = new Form3 { Text = formText, TabText = formText };
            }

            return f;
        }
    }
}
