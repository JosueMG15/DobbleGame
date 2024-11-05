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
        public DataTemplate JugadorRojoPlantilla {  get; set; }
        public DataTemplate JugadorAzulPlantilla {  get; set; }
        public DataTemplate JugadorVerdePlantilla { get; set; }
        public DataTemplate JugadorAmarilloPlantilla { get; set; }
        public DataTemplate InvitarAzulPlantilla { get; set; }
        public DataTemplate InvitarVerdePlantilla { get; set; }
        public DataTemplate InvitarAmarilloPlantilla { get; set; }

        private readonly Dictionary<string, bool> plantillasDisponibles = new Dictionary<string, bool>
        {
            {"JugadorRojoPlantilla", true},
            {"JugadorAzulPlantilla", true },
            {"JugadorVerdePlantilla", true },
            {"JugadorAmarilloPlantilla", true }
        };

        public override DataTemplate SelectTemplate(object item, DependencyObject contenedor)
        {
            var usuario = item as CuentaUsuario;

            if (usuario == null) return null;

            if (usuario.EsAnfitrion)
            {
                OcuparPlantilla("JugadorRojoPlantilla");
                return JugadorRojoPlantilla;
            }

            string nombrePlantillaDisponible = ObtenerPlantillaDisponible();
            if (nombrePlantillaDisponible == null)
                return null;

            OcuparPlantilla(nombrePlantillaDisponible);

            switch (nombrePlantillaDisponible)
            {
                case "JugadorAzulPlantilla":
                    return JugadorAzulPlantilla;
                case "JugadorVerdePlantilla":
                    return JugadorVerdePlantilla;
                case "JugadorAmarilloPlantilla":
                    return JugadorAmarilloPlantilla;
                default:
                    return null;
            }
        }

        private string ObtenerPlantillaDisponible()
        {
            return plantillasDisponibles.FirstOrDefault(p => p.Value).Key;
        }

        private void OcuparPlantilla(string nombrePlantilla)
        {
            if (plantillasDisponibles.ContainsKey(nombrePlantilla))
            {
                plantillasDisponibles[nombrePlantilla] = false;
            }
        }

        public void ReiniciarPlantillas()
        {
            foreach (var plantilla in plantillasDisponibles.Keys.ToList())
            {
                plantillasDisponibles[plantilla] = true;
            }
        }
    }
}
