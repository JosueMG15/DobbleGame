using DobbleGame.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DobbleGame
{
    public class SelectorPlantillaJugador : DataTemplateSelector
    {
        private static SelectorPlantillaJugador _instancia = new SelectorPlantillaJugador();
        public static SelectorPlantillaJugador Instancia => _instancia;

        public SelectorPlantillaJugador() { }

        public DataTemplate JugadorRojoPlantilla {  get; set; }
        public DataTemplate JugadorAzulPlantilla {  get; set; }
        public DataTemplate JugadorVerdePlantilla { get; set; }
        public DataTemplate JugadorAmarilloPlantilla { get; set; }
        public DataTemplate InvitarAzulPlantilla { get; set; }
        public DataTemplate InvitarVerdePlantilla { get; set; }
        public DataTemplate InvitarAmarilloPlantilla { get; set; }

        private int contadorPlantillas = 0;

        public override DataTemplate SelectTemplate(object item, DependencyObject contenedor)
        {
            var usuario = item as CuentaUsuario;

            if (usuario == null) return null;

            usuario.NumeroJugador = contadorPlantillas + 1;

            if (usuario.EsAnfitrion)
            {
                contadorPlantillas += 1;
                return JugadorRojoPlantilla;
            }

            switch (contadorPlantillas)
            {
                case 1:
                    contadorPlantillas += 1;
                    return JugadorAzulPlantilla;
                case 2:
                    contadorPlantillas += 1;
                    return JugadorVerdePlantilla;
                case 3:
                    contadorPlantillas += 1;
                    return JugadorAmarilloPlantilla;
                default:
                    return null;
            }
        }

        public void ReiniciarPlantillas()
        {
            contadorPlantillas = 0;
        }
    }
}
