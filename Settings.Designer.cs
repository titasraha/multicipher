namespace MultiCipher
{
    partial class Settings
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
            this.chkDualPwd = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbAlgo1 = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbAlgo2 = new System.Windows.Forms.ComboBox();
            this.m_lblKdfIt = new System.Windows.Forms.Label();
            this.numTransformations = new System.Windows.Forms.NumericUpDown();
            this.cmdSetPassword = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTransformations)).BeginInit();
            this.SuspendLayout();
            // 
            // chkDualPwd
            // 
            this.chkDualPwd.AutoSize = true;
            this.chkDualPwd.Location = new System.Drawing.Point(12, 165);
            this.chkDualPwd.Name = "chkDualPwd";
            this.chkDualPwd.Size = new System.Drawing.Size(127, 17);
            this.chkDualPwd.TabIndex = 4;
            this.chkDualPwd.Text = "Dual Password Mode";
            this.chkDualPwd.UseVisualStyleBackColor = true;
            this.chkDualPwd.CheckedChanged += new System.EventHandler(this.chkDualPwd_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(284, 196);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(203, 196);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbAlgo1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(347, 52);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cipher 1 (Key provided by KeePass)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Algorithm:";
            // 
            // cmbAlgo1
            // 
            this.cmbAlgo1.DisplayMember = "Value";
            this.cmbAlgo1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlgo1.FormattingEnabled = true;
            this.cmbAlgo1.Location = new System.Drawing.Point(65, 19);
            this.cmbAlgo1.Name = "cmbAlgo1";
            this.cmbAlgo1.Size = new System.Drawing.Size(276, 21);
            this.cmbAlgo1.TabIndex = 1;
            this.cmbAlgo1.ValueMember = "Key";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmbAlgo2);
            this.groupBox2.Controls.Add(this.m_lblKdfIt);
            this.groupBox2.Controls.Add(this.numTransformations);
            this.groupBox2.Location = new System.Drawing.Point(12, 70);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(347, 83);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Cipher 2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Algorithm:";
            // 
            // cmbAlgo2
            // 
            this.cmbAlgo2.DisplayMember = "Value";
            this.cmbAlgo2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlgo2.FormattingEnabled = true;
            this.cmbAlgo2.Location = new System.Drawing.Point(65, 19);
            this.cmbAlgo2.Name = "cmbAlgo2";
            this.cmbAlgo2.Size = new System.Drawing.Size(276, 21);
            this.cmbAlgo2.TabIndex = 1;
            this.cmbAlgo2.ValueMember = "Key";
            // 
            // m_lblKdfIt
            // 
            this.m_lblKdfIt.AutoSize = true;
            this.m_lblKdfIt.Location = new System.Drawing.Point(6, 56);
            this.m_lblKdfIt.Name = "m_lblKdfIt";
            this.m_lblKdfIt.Size = new System.Drawing.Size(147, 13);
            this.m_lblKdfIt.TabIndex = 2;
            this.m_lblKdfIt.Text = "Key Transformation Iterations:";
            // 
            // numTransformations
            // 
            this.numTransformations.Location = new System.Drawing.Point(159, 54);
            this.numTransformations.Maximum = new decimal(new int[] {
            3650,
            0,
            0,
            0});
            this.numTransformations.Name = "numTransformations";
            this.numTransformations.Size = new System.Drawing.Size(105, 20);
            this.numTransformations.TabIndex = 3;
            this.numTransformations.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cmdSetPassword
            // 
            this.cmdSetPassword.Location = new System.Drawing.Point(144, 161);
            this.cmdSetPassword.Name = "cmdSetPassword";
            this.cmdSetPassword.Size = new System.Drawing.Size(132, 23);
            this.cmdSetPassword.TabIndex = 5;
            this.cmdSetPassword.Text = "Set 2nd &Password";
            this.cmdSetPassword.UseVisualStyleBackColor = true;
            this.cmdSetPassword.Click += new System.EventHandler(this.cmdSetPassword_Click);
            // 
            // Settings
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(371, 231);
            this.Controls.Add(this.cmdSetPassword);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.chkDualPwd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MultiCipher Options";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTransformations)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkDualPwd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numTransformations;
        private System.Windows.Forms.Label m_lblKdfIt;
        private System.Windows.Forms.Button cmdSetPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbAlgo1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbAlgo2;
    }
}