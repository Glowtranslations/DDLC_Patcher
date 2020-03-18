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

using Monika.Rpy;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.Media.Text;

namespace DDLC_Patcher
{
    class RpyManipulation
    {
        public static void ExportRPY(string po, string rpy, string outfile)
        {
            // 1
            Node nodoPo = NodeFactory.FromFile(po); // Po
            nodoPo.Transform<Po2Binary, BinaryFormat, Po>();

            Node nodoOr = NodeFactory.FromFile(rpy); // BinaryFormat


            // 2
            IConverter<BinaryFormat, Rpy> TextConverter = new BinaryFormat2Rpy
            {

                PoFix = nodoPo.GetFormatAs<Po>()

            };
            Node nodoRpy = nodoOr.Transform(TextConverter);

            // 3
            IConverter<Rpy, BinaryFormat> RpyConverter = new Rpy2BinaryFormat { };
            Node nodoFile = nodoRpy.Transform(RpyConverter);
            //3
            nodoFile.Stream.WriteTo(outfile);
        }
    }
}