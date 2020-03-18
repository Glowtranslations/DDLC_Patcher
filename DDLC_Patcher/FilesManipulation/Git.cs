// Copyright (C) 2019 Pedro Garau Martínez
//
// This file is part of DDLC_Patcher.
//
// DDLC_Patcher is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// DDLC_Patcher is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with DDLC_Patcher. If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace DDLC_Patcher.FilesManipulation
{
    public class Git
    {
        public static void GetRepoZip(string token, string url, string name, string Folder)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var credentials = string.Format(CultureInfo.InvariantCulture, "{0}:", token);
                credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
                var contents = client.GetByteArrayAsync(url).Result;
                File.WriteAllBytes(@Folder + "/" + name, contents);
            }
        }
    }
}
