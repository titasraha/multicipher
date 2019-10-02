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
using KeePass.UI;
using KeePassLib.Security;
using KeePassLib.Utility;
using System;
using System.Windows.Forms;

namespace MultiCipher.Yubikey
{
    public partial class RecoveryKeyFrm : Form
    {
        public ProtectedBinary Key { get; set; }
        public bool ViewMode { get; private set; }

        public RecoveryKeyFrm()
        {
            InitializeComponent();
            this.Key = null;
            this.ViewMode = false;
            this.txtKey.ReadOnly = false;
            this.lblTitle.Text = "Please enter the recovery key";
            this.cmdOk.Visible = true;
            this.cmdCancel.Text = "&Cancel";

            txtKey.EnableProtection(false);
            SecureTextBoxEx.InitEx(ref txtKey);
        }

        public RecoveryKeyFrm(ProtectedBinary key)
        {
            InitializeComponent();
            this.Key = key;
            this.ViewMode = true;

            txtKey.EnableProtection(false);
            SecureTextBoxEx.InitEx(ref txtKey);
        }

        private void RecoveryKeyFrm_Load(object sender, EventArgs e)
        {
            if (Key != null)
                txtKey.TextEx = Tools.BytesToHexString(Key);
        }



        private void cmdOk_Click(object sender, EventArgs e)
        {
            try
            {
                Key = Tools.HexStringToBytes(txtKey.TextEx, 20);
            }
            catch (Exception ex)
            {
                MessageService.ShowWarning(ex.Message);
                DialogResult = DialogResult.None;
            }
        }
    }
}
