using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinNorthWind_Project
{
    public enum validType { Common, Numeric, Required, RequiredNumeric };

    public partial class GudiTextBox : TextBox
    {
        private validType input;
        public validType InputType {
            get { return input; } 
            set 
            { 
                input = value;
                if (input == validType.Required || input == validType.RequiredNumeric)
                {
                    this.BackColor = Color.Aqua;
                }
            } 
        }

        public GudiTextBox()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (input == validType.Numeric || input == validType.RequiredNumeric)
            {
                if (! char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != '.')
                {
                    e.Handled = true;
                }
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            if (input == validType.Required || input == validType.RequiredNumeric)
            {
                if (string.IsNullOrWhiteSpace(this.Text.Trim()))
                {
                    MessageBox.Show($"{this.Tag} 필수입력항목입니다. ");
                    this.Focus();
                }
            }
        }
    }
}
