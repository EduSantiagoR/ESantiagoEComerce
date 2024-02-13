namespace PL.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Detalles { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int Cantidad { get; set; }
        public List<object> Productos { get; set; }
    }
}
