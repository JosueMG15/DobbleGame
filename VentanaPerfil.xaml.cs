﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DobbleGame
{
    /// <summary>
    /// Lógica de interacción para VentanaPerfil.xaml
    /// </summary>
    public partial class VentanaPerfil : Window
    {
        public VentanaPerfil()
        {
            InitializeComponent();
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            VentanaMenu ventanaMenu = new VentanaMenu();
            ventanaMenu.Show();
            this.Close();
        }
    }
}
