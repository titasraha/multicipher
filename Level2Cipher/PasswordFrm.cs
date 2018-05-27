/*

    MultiCipher Plugin for Keepass Password Safe
    Copyright (C) 2016 Titas Raha <support@titasraha.com>

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;



using KeePass.UI;
using KeePassLib.Cryptography;
using KeePassLib.Keys;
using KeePassLib.Security;


namespace MultiCipher
{
    public partial class PasswordFrm : Form
    {
        private SecureEdit m_secPassword = new SecureEdit();
        private SecureEdit m_secPassword2 = new SecureEdit();
        private CompositeKey m_pwd = null;
        private bool m_bIsNew;

        public CompositeKey Password
        {
            get { return m_pwd; }            
        }

        public PasswordFrm(bool bIsNew)
        {
            m_bIsNew = bIsNew;
            InitializeComponent();
            lblPassword2.Visible = bIsNew;
            txtPassword2.Visible = bIsNew;


        }

        private void btnOk_Click(object sender, EventArgs e)
        {

            if (m_bIsNew && !m_secPassword.ContentsEqualTo(m_secPassword2))
            {
                MessageBox.Show(this, "Passwords do not match!", "Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            m_pwd = new CompositeKey();

            byte[] pb = m_secPassword.ToUtf8();
            m_pwd.AddUserKey(new KcpPassword(pb));
            Array.Clear(pb, 0, pb.Length);
       
            DialogResult = DialogResult.OK;
        }

        private void PasswordFrmNew_Load(object sender, EventArgs e)
        {
            m_secPassword.SecureDesktopMode = false;
            m_secPassword.Attach(txtPassword, null, true);
            if (m_bIsNew)
                m_secPassword2.Attach(txtPassword2, null, true);
        }

        private void PasswordFrmNew_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_secPassword.Detach();
            if (m_bIsNew)
                m_secPassword2.Detach();
        }

        private void chkShowPwd_CheckedChanged(object sender, EventArgs e)
        {
            bool bHide = !chkShowPwd.Checked;

            m_secPassword.EnableProtection(bHide);
            if (m_bIsNew)
                m_secPassword2.EnableProtection(bHide);
            

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_pwd = null;
        }
    }
}
