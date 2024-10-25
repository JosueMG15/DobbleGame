﻿using DobbleGame.Utilidades;
using Dominio;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DobbleGame
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void IniciarSesion()
        {
            if (!HayCamposVacios())
            {
                Servidor.GestionJugadorClient proxy = new Servidor.GestionJugadorClient();
                var cuentaInicioSesion = proxy.IniciarSesionJugador(tbUsuario.Text, Utilidades.EncriptadorContraseña.GenerarHashSHA512(pbContraseña.Password));

                if (cuentaInicioSesion != null)
                {
                    CuentaUsuario.cuentaUsuarioActual = new CuentaUsuario
                    {
                        IdCuentaUsuario = cuentaInicioSesion.IdCuentaUsuario,
                        Usuario = cuentaInicioSesion.Usuario,
                        Correo = cuentaInicioSesion.Correo,
                        Contraseña = cuentaInicioSesion.Contraseña,
                        Foto = cuentaInicioSesion.Foto,
                        Puntaje = cuentaInicioSesion.Puntaje,
                        Estado = true,
                    };
                    VentanaMenu ventanaMenu = new VentanaMenu();
                    this.Close();
                    ventanaMenu.Show();
                }
                else
                {
                    MostrarMensaje("No se pudo inciar sesión");
                }
            }
            
        }

        private void BtnEntrarMenu_Click(object sender, RoutedEventArgs e)
        {
            IniciarSesion();
        }

        private bool HayCamposVacios()
        {
            bool hayCamposVacios =
                string.IsNullOrEmpty(tbUsuario.Text) ||
                string.IsNullOrEmpty(pbContraseña.Password);

            if (hayCamposVacios)
            {
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                return true;
            }

            return false;
        }

        private void ClicCrearCuentaTf(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VentanaRegistro ventanaRegistro = new VentanaRegistro();
            ventanaRegistro.Show();
            this.Close();
        }

        private void ClicRecuperarContraseñaTf(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VentanaRecuperarContraseña ventanaRecuperarContraseña = new VentanaRecuperarContraseña();
            ventanaRecuperarContraseña.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventanaRecuperarContraseña.ShowDialog();
        }

        private void BtnCambioIdioma_Click(object sender, RoutedEventArgs e)
        {
            string idiomaEspañol = "es-MX";
            string idiomaIngles = "en-US";

            if (Thread.CurrentThread.CurrentUICulture.Name == idiomaEspañol)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(idiomaIngles);
            } 
            else
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(idiomaEspañol);
            }
                
        }

        private void MostrarMensaje(string mensaje)
        {
            panelMensaje.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }

        private void PasswordBox_CambioDeContraseña(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }
    }
}
