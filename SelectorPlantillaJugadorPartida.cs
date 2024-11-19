﻿using DobbleGame.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DobbleGame
{
    public class SelectorPlantillaJugadorPartida : DataTemplateSelector
    {
        public DataTemplate Plantilla1 { get; set; }
        public DataTemplate Plantilla2 { get; set; }
        public DataTemplate Plantilla3 { get; set; }
        public DataTemplate Plantilla4 { get; set; }

        private readonly Dictionary<string, bool> plantillasDisponibles = new Dictionary<string, bool>
        {
            {"Plantilla1", true},
            {"Plantilla2", true},
            {"Plantilla3", true},
            {"Plantilla4", true},
        };

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var jugador = item as Jugador;
            if (jugador == null)
                return base.SelectTemplate(item, container);

            if (jugador.Usuario == Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario
                && plantillasDisponibles["Plantilla1"])
            {
                plantillasDisponibles["Plantilla1"] = false;
                return Plantilla1;
            }
                
            if (plantillasDisponibles["Plantilla2"])
            {
                plantillasDisponibles["Plantilla2"] = false;
                return Plantilla2;
            }
            if (plantillasDisponibles["Plantilla3"])
            {
                plantillasDisponibles["Plantilla3"] = false;
                return Plantilla3;
            }
            if (plantillasDisponibles["Plantilla4"])
            {
                plantillasDisponibles["Plantilla4"] = false;
                return Plantilla4;
            }

            return base.SelectTemplate(item, container);
        }
    }
}