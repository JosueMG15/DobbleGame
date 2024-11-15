using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobbleGame.LogicaDobble
{
    public class Icono
    {
        public int Id { get; set; }
        public string NombreIcono { get; set; }
        public string Ruta {  get; set; }

        public Icono(int id, string nombreIcono, string ruta)
        {
            Id = id;
            NombreIcono = nombreIcono;
            Ruta = ruta;
        }

        public static List<Icono> ListaIconos = new List<Icono>
        {
            new Icono(0, "Arbol", "pack://application:,,,/Iconos/Arbol.png"),
            new Icono(1, "Auto", "pack://application:,,,/Iconos/Auto.png"),
            new Icono(2, "Balon", "pack://application:,,,/Iconos/Balon.png"),
            new Icono(3, "Bebe", "pack://application:,,,/Iconos/Bebe.png"),
            new Icono(4, "Bicicleta", "pack://application:,,,/Iconos/Bicicleta.png"),
            new Icono(5, "Bola", "pack://application:,,,/Iconos/Bola.png"),
            new Icono(6, "Caballo", "pack://application:,,,/Iconos/Caballo.png"),
            new Icono(7, "Camara", "pack://application:,,,/Iconos/Camara.png"),
            new Icono(8, "Cangrejo", "pack://application:,,,/Iconos/Cangrejo.png"),
            new Icono(9, "Casa", "pack://application:,,,/Iconos/Casa.png"),
            new Icono(10, "Chanclas", "pack://application:,,,/Iconos/Chanclas.png"),
            new Icono(11, "Circo", "pack://application:,,,/Iconos/Circo.png"),
            new Icono(12, "Cohete", "pack://application:,,,/Iconos/Cohete.png"),
            new Icono(13, "Consola", "pack://application:,,,/Iconos/Consola.png"),
            new Icono(14, "Corazon", "pack://application:,,,/Iconos/Corazon.png"),
            new Icono(15, "Corona", "pack://application:,,,/Iconos/Corona.png"),
            new Icono(16, "Crayolas", "pack://application:,,,/Iconos/Crayolas.png"),
            new Icono(17,"Cubeta", "pack://application:,,,/Iconos/Cubeta.png"),
            new Icono(18, "Dragon", "pack://application:,,,/Iconos/Dragon.png"),
            new Icono(19, "Enojado", "pack://application:,,,/Iconos/Enojado.png"),
            new Icono(20, "Feliz", "pack://application:,,,/Iconos/Feliz.png"),
            new Icono(21, "Flor", "pack://application:,,,/Iconos/Flor.png"),
            new Icono(22, "Foco", "pack://application:,,,/Iconos/Foco.png"),
            new Icono(23, "Gato", "pack://application:,,,/Iconos/Gato.png"),
            new Icono(24, "Globos", "pack://application:,,,/Iconos/Globos.png"),
            new Icono(25, "Hamster", "pack://application:,,,/Iconos/Hamster.png"),
            new Icono(26,"Lapiz", "pack://application:,,,/Iconos/Lapiz.png"),
            new Icono(27, "Loro", "pack://application:,,,/Iconos/Loro.png"),
            new Icono(28, "Luna", "pack://application:,,,/Iconos/Luna.png"),
            new Icono(29, "Mano", "pack://application:,,,/Iconos/Mano.png"),
            new Icono(30, "Manzana", "pack://application:,,,/Iconos/Manzana.png"),
            new Icono(31, "Mochila", "pack://application:,,,/Iconos/Mochila.png"),
            new Icono(32, "Montaña", "pack://application:,,,/Iconos/Montaña.png"),
            new Icono(33, "Moto", "pack://application:,,,/Iconos/Moto.png"),
            new Icono(34, "Naranja", "pack://application:,,,/Iconos/Naranja.png"),
            new Icono(35, "Niña", "pack://application:,,,/Iconos/Niña.png"),
            new Icono(36, "Pastel", "pack://application:,,,/Iconos/Pastel.png"),
            new Icono(37, "Pato", "pack://application:,,,/Iconos/Pato.png"),
            new Icono(38, "Pera", "pack://application:,,,/Iconos/Pera.png"),
            new Icono(39, "Perro", "pack://application:,,,/Iconos/Perro.png"),
            new Icono(40, "Pie", "pack://application:,,,/Iconos/Pie.png"),
            new Icono(41, "Pila", "pack://application:,,,/Iconos/Pila.png"),
            new Icono(42, "Pino", "pack://application:,,,/Iconos/Pino.png"),
            new Icono(43, "Pintura", "pack://application:,,,/Iconos/Pintura.png"),
            new Icono(44, "Rana", "pack://application:,,,/Iconos/Rana.png"),
            new Icono(45, "Regalo", "pack://application:,,,/Iconos/Regalo.png"),
            new Icono(46, "Regla", "pack://application:,,,/Iconos/Regla.png"),
            new Icono(47, "Reloj", "pack://application:,,,/Iconos/Reloj.png"),
            new Icono(48, "Reno", "pack://application:,,,/Iconos/Reno.png"),
            new Icono(49, "Robot", "pack://application:,,,/Iconos/Robot.png"),
            new Icono(50, "Rompecabezas", "pack://application:,,,/Iconos/Rompecabezas.png"),
            new Icono(51, "Santa", "pack://application:,,,/Iconos/Santa.png"),
            new Icono(52, "Sofa", "pack://application:,,,/Iconos/Sofa.png"),
            new Icono(53, "Sol", "pack://application:,,,/Iconos/Sol.png"),
            new Icono(54, "Sombrilla", "pack://application:,,,/Iconos/Sombrilla.png"),
            new Icono(55, "Tren", "pack://application:,,,/Iconos/Tren.png"),
            new Icono(56, "Triste", "pack://application:,,,/Iconos/Triste.png"),
        };

        public static Icono ObtenerIcono(string nombreIcono)
        {
            return ListaIconos.FirstOrDefault(i => i.NombreIcono == nombreIcono);
        }

        public static Icono ObtenerIconoPorId(int id)
        {
            return ListaIconos.FirstOrDefault(i => i.Id == id);
        }
    }
}
