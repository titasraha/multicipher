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
using System.Text;
using System.Diagnostics;

using KeePass.Plugins;

namespace MultiCipher
{
    public sealed class MultiCipherExt:Plugin
    {
        private IPluginHost m_host = null;
        private static MultiCipherEngine m_Level2CipherEngine = new MultiCipherEngine();

        public override bool Initialize(IPluginHost host)
        {
            if (host == null) return false;
            m_host = host;

            Debug.Assert(m_Level2CipherEngine != null);
            if (m_Level2CipherEngine == null) return false;

            m_host.CipherPool.AddCipher(m_Level2CipherEngine);

            return true;
        }
    }
}
