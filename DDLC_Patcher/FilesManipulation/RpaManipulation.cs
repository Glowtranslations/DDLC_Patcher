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

using RpaExtractor;
using System.IO;
using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace DDLC_Patcher.FilesManipulation
{
    class RpaManipulation
    {
        public static void ExportRPA(string rpa, string outfolder)
        {
            // 1
            Node nodo = NodeFactory.FromFile(rpa); // BinaryFormat

            // 2
            IConverter<BinaryFormat, Rpa> FadConverter = new BinaryFormat2Rpa { };
            Node nodoScript = nodo.Transform(FadConverter);

            // 3
            IConverter<Rpa, NodeContainerFormat> ContainerConverter = new Rpa2NodeContainer { };
            Node nodoContainer = nodoScript.Transform(ContainerConverter);

            foreach (var child in Navigator.IterateNodes(nodoContainer))
            {
                if (child.Stream == null)
                    continue;
                string output = Path.Combine(outfolder + "/" + child.Name);
                child.Stream.WriteTo(output);
            }
        }


    }
}
