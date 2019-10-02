/*

    MultiCipher Plugin for Keepass Password Safe
    Copyright (C) 2019 Titas Raha <support@titasraha.com>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/
using System;
using System.Windows.Forms;
using KeePass.UI;
using KeePassLib.Keys;
using KeePassLib.Security;

namespace MultiCipher.Yubikey
{
    public partial class VerifyFrm : Form
    {
        public ProtectedBinary SecretKey { get; private set; }
       

        public VerifyFrm()
        {
            SecretKey = null;

            InitializeComponent();

            txtPassword.EnableProtection(false);
            SecureTextBoxEx.InitEx(ref txtPassword);

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text == "")
            {
                MessageBox.Show(this, "Secret Key cannot be blank", "Blank Key", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                txtPassword.Focus();
                return;
            }

            var secretBytesRaw = txtPassword.TextEx.ReadUtf8();
            byte[] secretBytes = new byte[20];

            try
            {
                SecretKey = Tools.HexStringToBytes(txtPassword.TextEx, 20);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                DialogResult = DialogResult.None;
                txtPassword.Focus();
            }

            
            
        }


       
        private void btnCancel_Click(object sender, EventArgs e)
        {
            SecretKey = null;
        }
    }
}
