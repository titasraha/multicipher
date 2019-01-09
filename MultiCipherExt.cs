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
using System.Diagnostics;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib;
using KeePassLib.Utility;

namespace MultiCipher
{
    public sealed class MultiCipherExt:Plugin
    {
        private Configuration m_Config;
        private ToolStripMenuItem m_MultiCipherMenuItem;

        private static MultiCipherEngine m_Level2CipherEngine = new MultiCipherEngine();       

        public override bool Initialize(IPluginHost host)
        {
            if (host == null) return false;

           
            Debug.Assert(m_Level2CipherEngine != null);
            if (m_Level2CipherEngine == null) return false;
            
            m_Config = new Configuration(host);

            m_Level2CipherEngine.SetConfig(m_Config);
            host.CipherPool.AddCipher(m_Level2CipherEngine);

            host.MainWindow.ToolsMenu.DropDownOpening += OnToolsMenu;
            

            return true;
        }

        public override void Terminate()
        {
            Debug.Assert(m_Config != null);

            m_Config.Host.MainWindow.ToolsMenu.DropDownOpening -= OnToolsMenu;
        }

        private bool IsValidMultiCipherDatabase()
        {
            Debug.Assert(m_Level2CipherEngine != null && m_Config != null && m_Config.Host != null);

            PwDatabase pd = m_Config.Host.Database;
            return ((pd != null) && pd.IsOpen && m_Level2CipherEngine.CipherUuid.Equals(pd.DataCipherUuid));
        }

        private void OnToolsMenu(object sender, EventArgs e)
        {

            bool bOpen = IsValidMultiCipherDatabase();
                                
            if (m_MultiCipherMenuItem != null)
                m_MultiCipherMenuItem.Enabled = bOpen;
        }

        public override ToolStripMenuItem GetMenuItem(PluginMenuType t)
        {
            if (t != PluginMenuType.Main) return null;

            ToolStripMenuItem tsmi = new ToolStripMenuItem("MultiCipher Encryption Settings...");
            tsmi.Click += OnShowSetting;

            m_MultiCipherMenuItem = tsmi;            

            return tsmi;

        }

        private void OnShowSetting(object sender, EventArgs e)
        {
            bool IsValid = IsValidMultiCipherDatabase();

            Debug.Assert(IsValid);

            if (IsValid)
            {
                var form = new Settings(m_Config);

                UIUtil.ShowDialogAndDestroy(form);
            }
            else
                MessageService.ShowWarning("MultiCipher Plugin:", "Not a valid MultiCipher Database");


        }
    }
}
