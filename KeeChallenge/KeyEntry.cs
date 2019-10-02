﻿/* KeeChallenge--Provides Yubikey challenge-response capability to Keepass
*  Copyright (C) 2014  Ben Rush
*  
*  Modified by Titas Raha <support@titasraha.com> on Sep 16, 2019
*    Changed namespace, removed KeeChallengeProv dependency
*  
*  This program is free software; you can redistribute it and/or
*  modify it under the terms of the GNU General Public License
*  as published by the Free Software Foundation; either version 2
*  of the License, or (at your option) any later version.
*  
*  This program is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU General Public License for more details.
*  
*  You should have received a copy of the GNU General Public License
*  along with this program; if not, write to the Free Software
*  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.UI;

namespace MultiCipher.KeeChallenge
{
    public partial class KeyEntry : Form
    {
        private Timer countdown;
        private byte[] m_challenge;
        private byte[] m_response;
        private YubiWrapper yubi;
        private YubiSlot yubiSlot;
        private bool RecoveryButton;
        //private KeeChallengeProv m_parent;
        
        private bool success;

        private BackgroundWorker keyWorker;

        public byte[] Response
        {
            get { return m_response; }
            private set { m_response = value; }
        }

        public byte[] Challenge
        {
            get { return m_challenge; }
            set { m_challenge = value; }
        }

        public bool RecoveryMode
        {
            get;
            private set;
        }

        //public KeyEntry(YubiSlot slot)
        //{
        //    InitializeComponent();
        //   // m_parent = parent;
        //    success = false;
        //    Response = new byte[YubiWrapper.yubiRespLen];
        //    Challenge = null;
        //    yubiSlot = slot;
        //    RecoveryMode = false;
        //    Icon = Icon.FromHandle(Properties.Resources.yubikey.GetHicon());
        //}

        public KeyEntry(YubiSlot slot, byte[] challenge, bool AllowRecovery)
        {
            InitializeComponent();
            //m_parent = parent;
            success = false;
            Response = new byte[YubiWrapper.yubiRespLen];
            Challenge = challenge;
            yubiSlot = slot;
            RecoveryMode = false;
            RecoveryButton = AllowRecovery;
            Icon = Icon.FromHandle(Properties.Resources.yubikey.GetHicon());
        }
               
        private void YubiChallengeResponse(object sender, DoWorkEventArgs e) //Should terminate in 15seconds worst case
        {
            //Send the challenge to yubikey and get response
            if (Challenge == null) return;
            success = yubi.ChallengeResponse(yubiSlot, Challenge, out m_response);
            if (!success)
                MessageBox.Show("Error getting response from Yubikey", "Error");           

            return;
        }

        private void KeyWorkerDone(object sender, EventArgs e) //guaranteed to run after YubiChallengeResponse
        {
            if (success)
                DialogResult = DialogResult.OK;  //setting this calls Close() IF the form is shown using ShowDialog()
            else DialogResult = DialogResult.No; 
        }

        private void Countdown(object sender, EventArgs eventArgs)
        {
            if (countdown == null) return;
            if (progressBar.Value > 0)
                progressBar.Value--;
            else
            {
                CountdownCompleted();
            }
        }

        private void CountdownCompleted()
        {
            countdown.Stop();
            Close();
        }
        
        private void OnFormLoad(object sender, EventArgs e)
        {
            ControlBox = false;

            progressBar.Maximum = 15;
            progressBar.Minimum = 0;
            progressBar.Value = 15;

            yubi = new YubiWrapper();
            try
            {
                while (!yubi.Init())
                {
                    YubiPrompt prompt = new YubiPrompt(RecoveryButton);
                    DialogResult res =  prompt.ShowDialog();
                    if (res != DialogResult.Retry)
                    {
                        RecoveryMode = prompt.RecoveryMode;
                        DialogResult = DialogResult.Abort;
                        return;
                    }
                }
            }
            catch (PlatformNotSupportedException err)
            {
                Debug.Assert(false);
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK);
                return;
            }
            //spawn background countdown timer
            countdown = new Timer();
            countdown.Tick += Countdown;
            countdown.Interval = 1000;
            countdown.Enabled = true;

            keyWorker = new BackgroundWorker();            
            keyWorker.DoWork += YubiChallengeResponse;
            keyWorker.RunWorkerCompleted += KeyWorkerDone;
            keyWorker.RunWorkerAsync();     
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            if (countdown != null)
            {
                countdown.Enabled = false;
                countdown.Dispose();
            }
            if (yubi != null)
            {
                yubi.Close();
            }
            //GlobalWindowManager.RemoveWindow(this);
        }

        private void AbortButton_Click(object sender, EventArgs e)
        {
            CountdownCompleted();
        }
    }
}
