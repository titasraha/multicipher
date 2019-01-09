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

namespace MultiCipher
{
    public partial class PasswordFrm : Form
    {
        private bool m_bIsNew;

        public CompositeKey Password { get; private set; }
        public string AlgoLabel
        {
            set
            {
                lblAlgorithm.Text = value;
            }
        }

        public PasswordFrm(bool bIsNew)
        {
            Password = null;
            m_bIsNew = bIsNew;

            InitializeComponent();

            txtPassword.EnableProtection(true);
            SecureTextBoxEx.InitEx(ref txtPassword);

            txtPassword2.EnableProtection(true);
            SecureTextBoxEx.InitEx(ref txtPassword2);

            lblPassword2.Visible = bIsNew;
            txtPassword2.Visible = bIsNew;


        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (m_bIsNew)
            {
                if (txtPassword.Text == "")
                {
                    MessageBox.Show(this, "Password can not be blank", "Blank Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                    txtPassword.Focus();
                    return;
                }


                if (!txtPassword.TextEx.Equals(txtPassword2.TextEx, false))
                {
                    MessageBox.Show(this, "Passwords do not match!", "Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                    txtPassword.Focus();
                    return;
                }
            }

            Password = new CompositeKey();

            byte[] pb = txtPassword.TextEx.ReadUtf8();
            Password.AddUserKey(new KcpPassword(pb));
            Array.Clear(pb, 0, pb.Length);
       
            DialogResult = DialogResult.OK;
        }


        private void chkShowPwd_CheckedChanged(object sender, EventArgs e)
        {
            bool bHide = !chkShowPwd.Checked;

            txtPassword.EnableProtection(bHide);
            if (m_bIsNew)
                txtPassword2.EnableProtection(bHide);           
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Password = null;
        }
    }
}
