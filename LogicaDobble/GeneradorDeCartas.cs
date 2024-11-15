using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobbleGame.LogicaDobble
{
    public class GeneradorDeCartas
    {
        private static Random random = new Random();
        private static List<List<Icono>> GenerarCartas()
        {
            int n = 7;
            int numSimbolosPorCarta = n + 1;
            int totalCartas = n + n + n + 1;
            List<List<Icono>> cartas = new List<List<Icono>>();

            for (int i = 0; i < numSimbolosPorCarta; i++)
            {
                List<Icono> carta = new List<Icono>();
                carta.Add(Icono.ListaIconos[0]);
                for (int j = 0; j < n; j++)
                {
                    carta.Add(Icono.ListaIconos[j + 1 + i + n]);
                }
                cartas.Add(carta);
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    List<Icono> carta = new List<Icono>();
                    carta.Add(Icono.ListaIconos[i + 1]);
                    for (int k = 0; k < n; k++)
                    {
                        carta.Add(Icono.ListaIconos[(n + 1 + n * k + (i * k + j) % n)]);
                    }
                    cartas.Add(carta);
                }
            }

            return cartas;
        }

        public static void ImprimirCartas(List<List<Icono>> cartas)
        {
            int numCarta = 1;
            foreach (var carta in cartas)
            {
                Console.Write("Carta " + numCarta++ + ": ");
                Console.WriteLine(string.Join(", ", carta.Select(icono => icono.NombreIcono)));
            }
        }

        public static List<List<Icono>> ObtenerCartasRevueltas()
        {
            List<List<Icono>> cartasRevueltas = GenerarCartas();
            int numCartas = cartasRevueltas.Count;

            for (int i = numCartas - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                var temporal = cartasRevueltas[i];
                cartasRevueltas[i] = cartasRevueltas[j];
                cartasRevueltas[j] = temporal;
            }

            return cartasRevueltas;
        }
    }
}
