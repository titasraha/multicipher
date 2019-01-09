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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using KeePass.App;

namespace MultiCipher
{
    internal partial class Settings : Form
    {
        private Configuration m_Config;
        private bool m_Loading;

        public Settings(Configuration Config)
        {
            Debug.Assert(Config != null);

            InitializeComponent();
            m_Config = Config;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

            m_Config.Key2Transformations = (ulong)numTransformations.Value;
            m_Config.Algorithm1 = ((KeyValuePair<SymAlgoCode, string>)cmbAlgo1.SelectedItem).Key;
            m_Config.Algorithm2 = ((KeyValuePair<SymAlgoCode, string>)cmbAlgo2.SelectedItem).Key;

            if (chkDualPwd.Checked)
                m_Config.KeyOption = KeyOption.DualPassword;
            else
                m_Config.KeyOption = KeyOption.SinglePassword;

            
        }

        private void SetCombo(ComboBox cmb, SymAlgoCode SymAlgo)
        {
            cmb.SelectedItem = null;
            foreach (KeyValuePair<SymAlgoCode, string> item in cmb.Items)
                if (item.Key == SymAlgo)
                {
                    cmb.SelectedItem = item;
                    break;
                }
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            m_Loading = true;

            Icon = AppIcons.Default;

            numTransformations.Minimum = ulong.MinValue;
            numTransformations.Maximum = ulong.MaxValue;

            cmbAlgo1.Items.Clear();
            cmbAlgo2.Items.Clear();
            foreach (SymAlgoInfo Sym in CipherInfo.List)
            {
                var item = new KeyValuePair<SymAlgoCode, string>(Sym.SymAlgoCode, Sym.Description);
                cmbAlgo1.Items.Add(item);
                cmbAlgo2.Items.Add(item);
            }                      

            SetCombo(cmbAlgo1, m_Config.Algorithm1);
            SetCombo(cmbAlgo2, m_Config.Algorithm2);

            chkDualPwd.Checked = m_Config.KeyOption == KeyOption.DualPassword;
            cmdSetPassword.Enabled = chkDualPwd.Checked;
            numTransformations.Value = m_Config.Key2Transformations;

            m_Loading = false;
            
        }

        private void chkDualPwd_CheckedChanged(object sender, EventArgs e)
        {
            cmdSetPassword.Enabled = chkDualPwd.Checked;
            if (cmdSetPassword.Enabled && !m_Loading)
                m_Config.ResetPassword();
        }

        private void cmdSetPassword_Click(object sender, EventArgs e)
        {
            m_Config.GetNewDualPassword(true);
        }


    }
}
