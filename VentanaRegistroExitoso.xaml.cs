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
    /// Lógica de interacción para VentanaRegistroExitoso.xaml
    /// </summary>
    public partial class VentanaRegistroExitoso : Window
    {
        public VentanaRegistroExitoso()
        {
            InitializeComponent();
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}