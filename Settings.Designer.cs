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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbAlgo1 = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.grp2ndKey = new System.Windows.Forms.GroupBox();
            this.grpYubikey = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rdoVariable = new System.Windows.Forms.RadioButton();
            this.rdoFixed = new System.Windows.Forms.RadioButton();
            this.rdoSlot1 = new System.Windows.Forms.RadioButton();
            this.rdoSlot2 = new System.Windows.Forms.RadioButton();
            this.rdoSingle = new System.Windows.Forms.RadioButton();
            this.rdoYubikeyHMACMode = new System.Windows.Forms.RadioButton();
            this.rdoDual = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbAlgo2 = new System.Windows.Forms.ComboBox();
            this.m_lblKdfIt = new System.Windows.Forms.Label();
            this.numTransformations = new System.Windows.Forms.NumericUpDown();
            this.cmdReset = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.grp2ndKey.SuspendLayout();
            this.grpYubikey.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTransformations)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(284, 355);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(203, 355);
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
            this.groupBox1.Text = "Cipher 1 (Key/IV provided by KeePass)";
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
            this.groupBox2.Controls.Add(this.grp2ndKey);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmbAlgo2);
            this.groupBox2.Controls.Add(this.m_lblKdfIt);
            this.groupBox2.Controls.Add(this.numTransformations);
            this.groupBox2.Location = new System.Drawing.Point(12, 70);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(347, 279);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Cipher 2";
            // 
            // grp2ndKey
            // 
            this.grp2ndKey.Controls.Add(this.grpYubikey);
            this.grp2ndKey.Controls.Add(this.rdoSingle);
            this.grp2ndKey.Controls.Add(this.rdoYubikeyHMACMode);
            this.grp2ndKey.Controls.Add(this.rdoDual);
            this.grp2ndKey.Enabled = false;
            this.grp2ndKey.Location = new System.Drawing.Point(56, 80);
            this.grp2ndKey.Name = "grp2ndKey";
            this.grp2ndKey.Size = new System.Drawing.Size(228, 185);
            this.grp2ndKey.TabIndex = 4;
            this.grp2ndKey.TabStop = false;
            this.grp2ndKey.Text = "2nd Encryption Key";
            // 
            // grpYubikey
            // 
            this.grpYubikey.Controls.Add(this.panel1);
            this.grpYubikey.Controls.Add(this.rdoSlot1);
            this.grpYubikey.Controls.Add(this.rdoSlot2);
            this.grpYubikey.Location = new System.Drawing.Point(33, 92);
            this.grpYubikey.Name = "grpYubikey";
            this.grpYubikey.Size = new System.Drawing.Size(171, 89);
            this.grpYubikey.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rdoVariable);
            this.panel1.Controls.Add(this.rdoFixed);
            this.panel1.Location = new System.Drawing.Point(14, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(139, 52);
            this.panel1.TabIndex = 11;
            // 
            // rdoVariable
            // 
            this.rdoVariable.AutoSize = true;
            this.rdoVariable.Location = new System.Drawing.Point(3, 3);
            this.rdoVariable.Name = "rdoVariable";
            this.rdoVariable.Size = new System.Drawing.Size(90, 17);
            this.rdoVariable.TabIndex = 0;
            this.rdoVariable.TabStop = true;
            this.rdoVariable.Text = "Variable Input";
            this.rdoVariable.UseVisualStyleBackColor = true;
            // 
            // rdoFixed
            // 
            this.rdoFixed.AutoSize = true;
            this.rdoFixed.Location = new System.Drawing.Point(3, 26);
            this.rdoFixed.Name = "rdoFixed";
            this.rdoFixed.Size = new System.Drawing.Size(114, 17);
            this.rdoFixed.TabIndex = 1;
            this.rdoFixed.TabStop = true;
            this.rdoFixed.Text = "Fixed 64 byte input";
            this.rdoFixed.UseVisualStyleBackColor = true;
            // 
            // rdoSlot1
            // 
            this.rdoSlot1.AutoSize = true;
            this.rdoSlot1.Location = new System.Drawing.Point(17, 5);
            this.rdoSlot1.Name = "rdoSlot1";
            this.rdoSlot1.Size = new System.Drawing.Size(52, 17);
            this.rdoSlot1.TabIndex = 0;
            this.rdoSlot1.TabStop = true;
            this.rdoSlot1.Text = "Slot 1";
            this.rdoSlot1.UseVisualStyleBackColor = true;
            // 
            // rdoSlot2
            // 
            this.rdoSlot2.AutoSize = true;
            this.rdoSlot2.Location = new System.Drawing.Point(101, 5);
            this.rdoSlot2.Name = "rdoSlot2";
            this.rdoSlot2.Size = new System.Drawing.Size(52, 17);
            this.rdoSlot2.TabIndex = 1;
            this.rdoSlot2.TabStop = true;
            this.rdoSlot2.Text = "Slot 2";
            this.rdoSlot2.UseVisualStyleBackColor = true;
            // 
            // rdoSingle
            // 
            this.rdoSingle.AutoSize = true;
            this.rdoSingle.Location = new System.Drawing.Point(17, 24);
            this.rdoSingle.Name = "rdoSingle";
            this.rdoSingle.Size = new System.Drawing.Size(187, 17);
            this.rdoSingle.TabIndex = 0;
            this.rdoSingle.TabStop = true;
            this.rdoSingle.Text = "Single Password (Use Master Key)";
            this.rdoSingle.UseVisualStyleBackColor = true;
            this.rdoSingle.CheckedChanged += new System.EventHandler(this.rdoSingle_CheckedChanged);
            // 
            // rdoYubikeyHMACMode
            // 
            this.rdoYubikeyHMACMode.AutoSize = true;
            this.rdoYubikeyHMACMode.Location = new System.Drawing.Point(17, 69);
            this.rdoYubikeyHMACMode.Name = "rdoYubikeyHMACMode";
            this.rdoYubikeyHMACMode.Size = new System.Drawing.Size(128, 17);
            this.rdoYubikeyHMACMode.TabIndex = 2;
            this.rdoYubikeyHMACMode.TabStop = true;
            this.rdoYubikeyHMACMode.Text = "Yubikey HMAC-SHA1";
            this.rdoYubikeyHMACMode.UseVisualStyleBackColor = true;
            this.rdoYubikeyHMACMode.CheckedChanged += new System.EventHandler(this.rdoYubikeyHMACMode_CheckedChanged);
            // 
            // rdoDual
            // 
            this.rdoDual.AutoSize = true;
            this.rdoDual.Location = new System.Drawing.Point(17, 46);
            this.rdoDual.Name = "rdoDual";
            this.rdoDual.Size = new System.Drawing.Size(126, 17);
            this.rdoDual.TabIndex = 1;
            this.rdoDual.TabStop = true;
            this.rdoDual.Text = "Dual Password Mode";
            this.rdoDual.UseVisualStyleBackColor = true;
            this.rdoDual.CheckedChanged += new System.EventHandler(this.rdoDual_CheckedChanged);
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
            this.m_lblKdfIt.Size = new System.Drawing.Size(201, 13);
            this.m_lblKdfIt.TabIndex = 2;
            this.m_lblKdfIt.Text = "Key Transformation Iterations (AES-KDF):";
            // 
            // numTransformations
            // 
            this.numTransformations.Location = new System.Drawing.Point(236, 54);
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
            // cmdReset
            // 
            this.cmdReset.Location = new System.Drawing.Point(12, 355);
            this.cmdReset.Name = "cmdReset";
            this.cmdReset.Size = new System.Drawing.Size(123, 23);
            this.cmdReset.TabIndex = 4;
            this.cmdReset.Text = "&Reset 2nd Key";
            this.cmdReset.UseVisualStyleBackColor = true;
            this.cmdReset.Click += new System.EventHandler(this.cmdReset_Click);
            // 
            // Settings
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(368, 391);
            this.Controls.Add(this.cmdReset);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
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
            this.grp2ndKey.ResumeLayout(false);
            this.grp2ndKey.PerformLayout();
            this.grpYubikey.ResumeLayout(false);
            this.grpYubikey.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTransformations)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numTransformations;
        private System.Windows.Forms.Label m_lblKdfIt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbAlgo1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbAlgo2;
        private System.Windows.Forms.RadioButton rdoSlot2;
        private System.Windows.Forms.RadioButton rdoSlot1;
        private System.Windows.Forms.RadioButton rdoYubikeyHMACMode;
        private System.Windows.Forms.RadioButton rdoDual;
        private System.Windows.Forms.RadioButton rdoSingle;
        private System.Windows.Forms.Button cmdReset;
        private System.Windows.Forms.GroupBox grp2ndKey;
        private System.Windows.Forms.Panel grpYubikey;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rdoVariable;
        private System.Windows.Forms.RadioButton rdoFixed;
    }
}