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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using DDLC_Patcher.FilesManipulation;
using DDLC_Patcher.Properties;

namespace DDLC_Patcher
{
    public partial class MainWindow : Form
    {
        public static bool Internet { get; set; }
        private Translation Ts { get; }
        private string OperatingSystem { get; }

        public MainWindow()
        {
            InitializeComponent();
            Internet = CheckInternet();

            //Get the current SO
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    OperatingSystem = "LIN";
                    break;
                case PlatformID.MacOSX:
                    OperatingSystem = "MAC";
                    break;
                default:
                    OperatingSystem = "WIN";
                    break;
            }

            Ts = new Translation(OperatingSystem);

            //Check if the game exist
            if(!Ts.CheckGameFolder())
            {
                MessageBox.Show("Esta carpeta no corresponde al videojuego de Doki Doki Literature Club. ¿Has puesto el programa en la carpeta correspondiente? (SO: " + OperatingSystem + ")", "No se ha reconocido el juego.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(3);
            }

            //Check the translation
            TranslationVersion.Text = Ts.CheckTranslation();
            if (Ts.Version == 0)
            {
                Update.BackgroundImage = Resources.bt_instalar;
                Play.Enabled = false;
                Play.BackgroundImage = Resources.bt_jugar_dis;
            }
            InitCustomLabelFont();
        }

        private void InitCustomLabelFont()
        {
            Play.TabStop = false;
            Play.FlatStyle = FlatStyle.Flat;
            Play.FlatAppearance.BorderSize = 0;
            Play.FlatAppearance.BorderColor = Color.FromArgb(255,255,255); //transparent

            Update.TabStop = false;
            Update.FlatStyle = FlatStyle.Flat;
            Update.FlatAppearance.BorderSize = 0;
            Update.FlatAppearance.BorderColor = Color.FromArgb(255, 255, 255); //transparent

            Credits.TabStop = false;
            Credits.FlatStyle = FlatStyle.Flat;
            Credits.FlatAppearance.BorderSize = 0;
            Credits.FlatAppearance.BorderColor = Color.FromArgb(255, 255, 255); //transparent

            Exit.TabStop = false;
            Exit.FlatStyle = FlatStyle.Flat;
            Exit.FlatAppearance.BorderSize = 0;
            Exit.FlatAppearance.BorderColor = Color.FromArgb(255, 255, 255); //transparent
        }

        private void MainWindow_Shown(Object sender, EventArgs e)
        {
            //Check the translation
            if (!Internet || Ts.Version == 0 || !Ts.CheckVersion()) return;
            MessageBox.Show("Hay una nueva actualización disponible, se procederá a instalarse.", "Nueva actualización disponible", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _ = ApplyTranslation(false, true);
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }
    /// <summary>
    /// Ejecuta el juego
    /// </summary>
        private void Play_Click(object sender, EventArgs e)
        {

            if (OperatingSystem.Equals("WIN")) System.Diagnostics.Process.Start("DDLC.exe");

            else if (OperatingSystem.Equals("MAC")) System.Diagnostics.Process.Start("DDLC.app\\Contents\\MacOS\\DDLC");

            else if (OperatingSystem.Equals("LIN")) System.Diagnostics.Process.Start("DDLC.sh");
        }

    /// <summary>
    /// Créditos
    /// </summary>
        private void Credits_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Esta traducción ha sido realizada por GlowTranslations.\n\n" +
                            "Créditos:\n\n" +
                            "-Darkmet98: Líder, programación del juego y de herramientas, traducción, corrección, tester y edición gráfica.\n\n" +
                            "-Roli300: Traducción y corrección.\n\n" +
                            "-Erena: Traducción y testeo.\n\n" +
                            "-Airu: Traducción.\n\n" +
                            "-Sany: Corrección.\n\n" +
                            "-Jesa: Corrección.\n\n" +
                            "-Oscar73: Corrección.\n\n" +
                            "-Yuny: Corrección.\n\n" +
                            "-Fox: Edición gráfica.\n\n" +
                            "-Roxa Amakura: Doblaje de Monika.\n\n" +
                            "-Gross: Testeo.\n\n" +
                            "-Retroductor: Testeo.\n\n" +
                            "-DRUB RoXaS: Testeo.\n\n" +
                            "-Laura Sullivan: Testeo.\n\n" +
                            "-Scythe: Maquetación de la canción.\n\n" +
                            "-Ilducci: Maquetación del doblaje.\n\n" +
                            "-All-Ice Team: Consejos y ayudas sobre la traducción.\n\n" +
                            "GlowTranslatios está totalmente en contra de todo tipo de pirateria. Ésta traducción ha sido realizada por fans y para fans, sin ningún ánimo de lucro. La distribución del parche es gratuita. Cualquier modo de cobro por éste trabajo está prohibido y será considerada una estafa.");
        }
    /// <summary>
    /// Comprueba si hay actualizaciones.
    /// </summary>
        private void Update_Click(object sender, EventArgs e)
        {
            if(Internet && !File.Exists("Update.zip"))
            {
                if (Ts.CheckVersion())
                    _ = ApplyTranslation(false, (Ts.Version != 0));
                else
                    MessageBox.Show("No hay nuevas actualizaciones.", "No hay nuevas actualizaciones",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (File.Exists("Update.zip"))
            {
                _ = ApplyTranslation(true, false);
            }
            else
            {
                MessageBox.Show("No tienes una conexión a internet y no se encuentra el archivo Update.zip, no se puede comprobar las nuevas actualizaciones.", "No tienes internet", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }
    /// <summary>
    /// Instala/Actualiza la traducción.
    /// </summary>
        private async Task ApplyTranslation(bool offline, bool update)
        {
            if (!offline)
            {
                if (!Ts.CheckGameVersion(update))
                {
                    MessageBox.Show("Se ha detectado una versión incompatible del juego. ¿Estás usando la última versión de la web de ddlc.moe o de steam? (SO: " + OperatingSystem + ")",
                        "Versión incompatible detectada.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
            }


            //Block Controls
            Play.Enabled = false;
            Play.BackgroundImage = Resources.bt_jugar_dis;
            Update.Enabled = false;
            Update.BackgroundImage = update ? Resources.bt_actualizar_dis : Resources.bt_instalar_dis;
            Credits.Enabled = false;
            Credits.BackgroundImage = Resources.bt_info_dis;
            Exit.Enabled = false;
            Exit.BackgroundImage = Resources.bt_salir_dis;
            TextLabel.Visible = true;

            Task task1;
            if (!offline)
            {
                TextLabel.Text = "Descargando el parche...";
                task1 = Task.Run(() => Ts.DownloadRepo());
                await task1;
            }
            else
            {
                TextLabel.Text = "Extrayendo el parche...";
                task1 = Task.Run(() => Ts.ExtractUpdate());
                await task1;
            }


            TextLabel.Text = "Aplicando traducción...";

            Task task2 = Task.Run(() => Ts.InstallTranslation(false));
            await task2;
            MessageBox.Show("Se ha instalado correctamente la traducción.", "Se ha instalado la traducción", MessageBoxButtons.OK, MessageBoxIcon.Information);

            TranslationVersion.Text = Ts.CheckTranslation();

            //Change window
            TextLabel.Text = "Proceso finalizado";


            //Enable Controls
            Play.Enabled = true;
            Play.BackgroundImage = Resources.bt_jugar;
            Update.Enabled = true;
            Update.BackgroundImage = Resources.bt_actualizar;
            Credits.Enabled = true;
            Credits.BackgroundImage = Resources.bt_info;
            Exit.Enabled = true;
            Exit.BackgroundImage = Resources.bt_salir;
        }



        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Directory.Delete(Ts.Temp, true);
        }

    /// <summary>
    /// Comprueba si hay internet
    /// </summary>
        private bool CheckInternet()
        {
            try
            {
                using (var client = new WebClient())
                    using (var stream = client.OpenRead("https://www.glowtranslations.es")) return true;
            }
            catch
            {
                return false;
            }
        }

        private void PictureBox3_Click(object sender, EventArgs e)
        {
            OpenUrl.Open("https://www.glowtranslations.es/");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(Directory.Exists(Ts.Temp)) Directory.Delete(Ts.Temp, true);
            Environment.Exit(0);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenUrl.Open("https://ddlc.moe/");
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            OpenUrl.Open("https://tradusquare.es/");
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            OpenUrl.Open("https://github.com/Glowtranslations/DDLC_ESP/blob/master/Bug.md");
        }
    }
}
