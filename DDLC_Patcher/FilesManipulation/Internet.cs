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
using System.Net;

namespace DDLC_Patcher.FilesManipulation
{
    public class Internet
    {
        public static void GetFile(string url, string name, string Folder)
        {
            Random random = new Random();
            url += "?random=" + random.Next().ToString();
            using (WebClient client = new WebClient()) client.DownloadFile(url, @Folder + "/" + name);
        }
    }
}