
namespace WinNorthWind_Project
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnFrmOrder = new System.Windows.Forms.Button();
            this.btnFrmEmployee = new System.Windows.Forms.Button();
            this.btnFrmBOM = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFrmOrder
            // 
            this.btnFrmOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFrmOrder.Location = new System.Drawing.Point(7, 3);
            this.btnFrmOrder.Name = "btnFrmOrder";
            this.btnFrmOrder.Size = new System.Drawing.Size(136, 32);
            this.btnFrmOrder.TabIndex = 0;
            this.btnFrmOrder.Text = "주문하기";
            this.btnFrmOrder.UseVisualStyleBackColor = true;
            this.btnFrmOrder.Click += new System.EventHandler(this.btnFrmOrder_Click);
            // 
            // btnFrmEmployee
            // 
            this.btnFrmEmployee.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFrmEmployee.Location = new System.Drawing.Point(154, 3);
            this.btnFrmEmployee.Name = "btnFrmEmployee";
            this.btnFrmEmployee.Size = new System.Drawing.Size(136, 32);
            this.btnFrmEmployee.TabIndex = 0;
            this.btnFrmEmployee.Text = "직원 등록/조회";
            this.btnFrmEmployee.UseVisualStyleBackColor = true;
            this.btnFrmEmployee.Click += new System.EventHandler(this.btnFrmEmployee_Click);
            // 
            // btnFrmBOM
            // 
            this.btnFrmBOM.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFrmBOM.Location = new System.Drawing.Point(301, 3);
            this.btnFrmBOM.Name = "btnFrmBOM";
            this.btnFrmBOM.Size = new System.Drawing.Size(136, 32);
            this.btnFrmBOM.TabIndex = 0;
            this.btnFrmBOM.Text = "BOM 정전개 조회";
            this.btnFrmBOM.UseVisualStyleBackColor = true;
            this.btnFrmBOM.Click += new System.EventHandler(this.btnFrmBOM_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnFrmOrder);
            this.panel1.Controls.Add(this.btnFrmBOM);
            this.panel1.Controls.Add(this.btnFrmEmployee);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1472, 39);
            this.panel1.TabIndex = 2;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1472, 798);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.IsMdiContainer = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmMain";
            this.Text = "frmMain";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnFrmOrder;
        private System.Windows.Forms.Button btnFrmEmployee;
        private System.Windows.Forms.Button btnFrmBOM;
        private System.Windows.Forms.Panel panel1;
    }
}