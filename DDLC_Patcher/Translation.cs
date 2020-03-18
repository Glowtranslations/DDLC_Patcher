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
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using DDLC_Patcher.FilesManipulation;

namespace DDLC_Patcher
{
    class Translation
    {
        private string System { get; }
        public string Temp { get; set; }
        public int Version { get; set; }
        public string GameDir { get; }
        public string[] VerFile { get; set; }


        public Translation(string system)
        {
            System = system;

            if(system.Equals("WIN") || system.Equals("LIN")) GameDir = "game/";
            else GameDir = "DDLC.app/Contents/Resources/autorun/game/";

            GenerateTempFolder();
        }



    /// <summary>
    /// Verifica la carpeta si es del juego o no.
    /// </summary>
        public bool CheckGameFolder()
        {
        return Directory.Exists("game") &&
               Directory.Exists("characters") &&
               Directory.Exists("lib") &&
               Directory.Exists("renpy") &&
               File.Exists("DDLC.py");
        }

    public bool CheckGameVersion(bool update)
    {
        var dir = (!update) ? GameDir + "scripts.rpa" : "Original/scripts.rpa";
        
        return VerFile[2] == Md5.CalculateMd5(dir);
    }

        /// <summary>
        /// Comprueba la versión en línea del parche
        /// </summary>
        public bool CheckVersion()
        {
            Internet.GetFile("https://raw.githubusercontent.com/Glowtranslations/DDLC_ESP/master/Version", "Version", Temp);
            VerFile = File.ReadAllLines(Temp + "/Version");
            return Convert.ToInt32(VerFile[0]) != Version;
        }

    /// <summary>
    /// Comprueba la versión instalada en el juego
    /// </summary>
        public string CheckTranslation()
        {
            if (!File.Exists("game/Version")) return "None";
            VerFile = File.ReadAllLines("game/Version");
            Version = Convert.ToInt32(VerFile[0]);
            return VerFile[1];


        }

    /// <summary>
    /// Se baja los archivos necesarios.
    /// </summary>
        public void DownloadRepo()
        {
            try
            {
                //Si estas usando un repositorio privado de git
                Internet.GetFile("https://github.com/Glowtranslations/DDLC_ESP/archive/master.zip", "TsFiles.zip", Temp);

                ZipFile.ExtractToDirectory(Temp + "/TsFiles.zip", Temp);
            }
            catch (Exception e)
            {
                MessageBox.Show("Se ha producido un error en la descarga de los repositorios (SO: " + System + ")\n" + e, "Error en la descarga de repositorios", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

    /// <summary>
    /// Descomprime el Update.zip que se encuentre en la misma carpeta que el parcheador
    /// </summary>
        public void ExtractUpdate()
        {
            try
            {
                ZipFile.ExtractToDirectory("Update.zip", Temp);
            }
            catch (Exception e)
            {
                MessageBox.Show("Se ha producido un error en la extracción del update (SO: " + System + ")\n" + e, "Error en la extracción del update", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

    /// <summary>
    /// Instala/Actualiza la traducción, es importante cambiarlo ya que es único por juego.
    /// </summary>
        public void InstallTranslation(bool updating)
        {
            //Se abre el changelog
            OpenUrl.Open(Temp + "/DDLC_ESP-master/Changelog.txt");

            try
            {
                //Aquí copias los archivos originales para las futuras actualizaciones
                if(!Directory.Exists("Original"))   
                {
                    Directory.CreateDirectory("Original");
                    File.Copy(GameDir + "scripts.rpa", "Original/scripts.rpa");
                }

                //Este método es cuando vas a actualizar la tradu, en el caso del doki no hace mucha falta pero lo dejo como ejemplo
                if(updating) File.Copy("../" + GameDir + "/Original/scripts.rpa", GameDir + "scripts.rpa");

                //Copy git files
                FixGame();

                File.Copy(Temp + "/DDLC_ESP-master/files/README.html", GameDir + "../README.html", true);
                File.Copy(Temp + "/DDLC_ESP-master/Version", GameDir + "Version", true);
                GameFiles.DirectoryCopy(Temp + "/DDLC_ESP-master/files/game", GameDir, true);

            }
            catch(Exception e)
            {
                MessageBox.Show("Se ha producido un error en la instalación de la traducción (SO: " + System + ")\n" + e, "Error en la instalación", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }


    /// <summary>
    /// Este método es único de DDLC, arregla un bug del motor con los rpa y los hash que impide visualizar la traducción correctamente.
    /// </summary>
        private void FixGame()
        {
            if (!File.Exists(GameDir + "scripts.rpa")) return;
            
            RpaManipulation.ExportRPA(GameDir + "scripts.rpa", GameDir);
            File.Delete(GameDir + "scripts.rpa");
        }

    /// <summary>
    /// Genera una carpeta temporal en %temp%
    /// </summary>
        private void GenerateTempFolder()
        {
            Temp = Path.GetTempPath() + "/" + Path.GetRandomFileName();
            Directory.CreateDirectory(Temp);
        }
    }
}
