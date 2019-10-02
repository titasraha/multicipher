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
using KeePassLib.Utility;
using MultiCipher.KeeChallenge;
using KeePass.UI;
using MultiCipher.Yubikey;
using KeePassLib.Security;

namespace MultiCipher
{
    internal partial class Settings : Form
    {
        private Configuration m_Config;
        //private bool m_Loading;

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

            if (grp2ndKey.Enabled)
            {
                if (rdoDual.Checked)
                {
                    if (m_Config.GetNewDualPassword() == null)
                        return;
                    m_Config.KeyOption = KeyOption.DualPassword;
                }
                else if (rdoSingle.Checked)
                {
                    m_Config.DeriveDualKey();
                    m_Config.KeyOption = KeyOption.SinglePassword;
                }
                else if (rdoYubikeyHMACMode.Checked)
                {
                    if (!rdoSlot1.Checked && !rdoSlot2.Checked)
                    {
                        MessageService.ShowWarning("MultiCipher Plugin:", "Please Choose Yubikey Slot");
                        return;
                    }

                    if (!rdoVariable.Checked && !rdoFixed.Checked)
                    {
                        MessageService.ShowWarning("MultiCipher Plugin:", "Please Choose Variable or Fixed Challenge Input");
                        return;
                    }


                    byte YubikeySlot = 2;
                    byte ChallengeLength = ConfigYubikey.CHALLENGE_LEN_64;

                    if (rdoSlot1.Checked)
                        YubikeySlot = 1;

                    if (rdoVariable.Checked)
                        ChallengeLength = ConfigYubikey.CHALLENGE_LEN_VARIABLE;

                    bool ValidateKey = MessageService.AskYesNo("It is highly recommended that you validate the secret key to make sure that Challenge/Response is working correctly.\r\n\r\nDo you want to validate the secret key", "Please Confirm", true, MessageBoxIcon.Question);
                    ProtectedBinary Response;
                    if (ValidateKey)
                    {
                        var f = new VerifyFrm();
                        DialogResult dr = f.ShowDialog();
                        ProtectedBinary SecProtected = f.SecretKey;
                        UIUtil.DestroyForm(f);

                        if (dr != DialogResult.OK || SecProtected == null)
                            return;
                        Response = m_Config.Yubikey.GetYubikeyResponse(YubikeySlot, ChallengeLength, SecProtected, false);
                    }
                    else
                    {
                        Response = m_Config.Yubikey.GetYubikeyResponse(YubikeySlot, ChallengeLength, null, false);
                    }

                    if (Response == null)
                    {
                        MessageService.ShowWarning("Yubikey Challenge/Response Failed");
                        return;
                    }

                    if (ValidateKey)
                        MessageService.ShowInfo("Key Validation Successful!");

                    m_Config.GetFromRecovery(Response);  // Set the 2nd key
                    m_Config.YubikeySlot = YubikeySlot;
                    m_Config.YubikeyChallengeLength = ChallengeLength;                    
                    m_Config.KeyOption = KeyOption.Yubikey_HMAC_SHA1;

                    // Show the newly generated 2nd key to the user
                    var fRecovery = new RecoveryKeyFrm(Response);
                    fRecovery.ShowDialog();
                    UIUtil.DestroyForm(fRecovery);


                }
                else
                {
                    MessageService.ShowWarning("MultiCipher Plugin:", "Please Choose Password Mode");
                    return;
                }
            }

            m_Config.Host.Database.Modified = true;

            DialogResult = DialogResult.OK;
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
            //m_Loading = true;

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

            rdoSingle.Checked = m_Config.KeyOption == KeyOption.SinglePassword;
            rdoDual.Checked = m_Config.KeyOption == KeyOption.DualPassword;
            rdoYubikeyHMACMode.Checked = m_Config.KeyOption == KeyOption.Yubikey_HMAC_SHA1;

            rdoSlot1.Checked = m_Config.YubikeySlot == 1;
            rdoSlot2.Checked = m_Config.YubikeySlot == 2;

            rdoVariable.Checked = m_Config.YubikeyChallengeLength == ConfigYubikey.CHALLENGE_LEN_VARIABLE;
            rdoFixed.Checked = m_Config.YubikeyChallengeLength == ConfigYubikey.CHALLENGE_LEN_64;

            numTransformations.Value = m_Config.Key2Transformations;

            //m_Loading = false;
            
        }


        private void rdoDual_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoSingle_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoYubikeyHMACMode_CheckedChanged(object sender, EventArgs e)
        {
            grpYubikey.Enabled = rdoYubikeyHMACMode.Checked;
        }

        private void cmdReset_Click(object sender, EventArgs e)
        {
            rdoDual.Checked = false;
            rdoSingle.Checked = false;
            rdoYubikeyHMACMode.Checked = false;
            rdoSlot1.Checked = false;
            rdoSlot2.Checked = false;
            rdoVariable.Checked = false;
            rdoFixed.Checked = false;
            grp2ndKey.Enabled = true;
        }
    }
}
