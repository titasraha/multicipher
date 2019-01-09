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
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib.Keys;
using System.Windows.Forms;
using System.Diagnostics;


namespace MultiCipher
{
    /// <summary>
    /// In memory configuration structure
    /// </summary>
    public class Configuration
    {
        private CompositeKey m_DualPassword;
        public IPluginHost Host { get; private set; }

        internal SymAlgoCode Algorithm1 { get; set; }
        internal SymAlgoCode Algorithm2 { get; set; }
        internal KeyOption KeyOption { get; set; }
        internal ulong Key2Transformations { get; set; }
        

        public Configuration(IPluginHost Host)
        {
            Debug.Assert(Host != null);
            this.Host = Host;

            // Defaults
            Algorithm1 = SymAlgoCode.AES256;
            Algorithm2 = SymAlgoCode.ThreeDES;
            KeyOption = KeyOption.SinglePassword;
            Key2Transformations = 10000;
            m_DualPassword = null;
        }

        private string GetAlgoLabel()
        {
            return Algorithm1.GetDescription() + " + " + Algorithm2.GetDescription();
        }

        public void ResetPassword()
        {
            m_DualPassword = null;
        }

        /// <summary>
        /// Opens a create new password dialog
        /// </summary>
        /// <param name="ForceDialog">True - to force it to ask for new password even if the password is set </param>
        /// <returns></returns>
        public CompositeKey GetNewDualPassword(bool ForceDialog)
        {
            if (m_DualPassword == null || ForceDialog)
            {
                PasswordFrm f = new PasswordFrm(true);
                f.AlgoLabel = GetAlgoLabel();
                DialogResult dr = f.ShowDialog();

                if (dr == DialogResult.OK)
                    m_DualPassword = f.Password;
                
                UIUtil.DestroyForm(f);
            }
            return m_DualPassword;
        }      

        public CompositeKey GetDualPassword()
        {
            PasswordFrm f = new PasswordFrm(false);
            f.AlgoLabel = GetAlgoLabel();
            DialogResult dr = f.ShowDialog();

            if (dr == DialogResult.OK && f.Password != null)
                m_DualPassword = f.Password;
            else
                m_DualPassword = null;

            UIUtil.DestroyForm(f);

            return m_DualPassword;
        }
    }



    public enum KeyOption
    {
        DualPassword = 0, SinglePassword = 1
    }
}
