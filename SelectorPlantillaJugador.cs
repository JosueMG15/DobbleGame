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
        public DataTemplate JugadorRojoPlantilla { get; set; }
        public DataTemplate JugadorAzulPlantilla { get; set; }
        public DataTemplate JugadorVerdePlantilla { get; set; }
        public DataTemplate JugadorAmarilloPlantilla { get; set; }

        private int contadorPlantillas = 0;
        private Dictionary<DataTemplate, bool> plantillasDisponibles;

        public SelectorPlantillaJugador() { }

        public void IniciarlizarPlantillas()
        {
            plantillasDisponibles = new Dictionary<DataTemplate, bool>();

            if (JugadorAzulPlantilla != null)
                plantillasDisponibles.Add(JugadorAzulPlantilla, true);
            if (JugadorVerdePlantilla != null)
                plantillasDisponibles.Add(JugadorVerdePlantilla, true);
            if (JugadorAmarilloPlantilla != null)
                plantillasDisponibles.Add(JugadorAmarilloPlantilla, true);
        }

        public override DataTemplate SelectTemplate(object objeto, DependencyObject contenedor)
        {
            var usuario = objeto as Jugador;

            if (usuario == null) return null;

            usuario.NumeroJugador = contadorPlantillas + 1;

            if (usuario.EsAnfitrion)
            {
                contadorPlantillas += 1;
                
                if (JugadorRojoPlantilla != null)
                {
                    return JugadorRojoPlantilla;
                }
            }

            var plantillaDisponible = plantillasDisponibles.FirstOrDefault(p => p.Value).Key;
            
            if (plantillaDisponible != null)
            {
                contadorPlantillas += 1;
                plantillasDisponibles[plantillaDisponible] = false;
                return plantillaDisponible;
            }

            return JugadorAmarilloPlantilla;
        }

        public void ReiniciarPlantillas()
        {
            contadorPlantillas = 0;

            foreach (var llave in plantillasDisponibles.Keys.ToList())
            {
                plantillasDisponibles[llave] = true;
            }
        }
    }
}
