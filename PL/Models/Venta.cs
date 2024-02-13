namespace PL.Models
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public Producto Producto { get; set; }
        public DateTime Fecha { get; set; }
        public List<object>? Ventas { get; set; }
        public List<object>? Carrito { get; set; }
    }
}
