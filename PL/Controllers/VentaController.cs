using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PL.Models;

namespace PL.Controllers
{
    public class VentaController : Controller
    {
        public IActionResult GetAll()
        {
            Venta ventaResult = new Venta();
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://192.168.0.136:3010/api/");
                var taskResponse = client.GetAsync("Venta");
                taskResponse.Wait();

                var resultService = taskResponse.Result;
                if (resultService.IsSuccessStatusCode)
                {
                    ventaResult.Ventas = new List<object>();
                    var readTask = resultService.Content.ReadAsStringAsync();
                    readTask.Wait();
                    dynamic jsonResult = JObject.Parse(readTask.Result.ToString());
                    foreach(var item in jsonResult.objects)
                    {
                        Venta venta = new Venta();
                        venta.Producto = new Producto();

                        venta.IdVenta = item.idVenta;
                        venta.Fecha = item.fecha;
                        venta.Producto.IdProducto = item.producto.idProducto;
                        venta.Producto.Nombre = item.producto.nombre;
                        venta.Producto.Detalles = item.producto.detalles;
                        venta.Producto.Precio = item.producto.precio;
                        venta.Producto.Cantidad = item.producto.cantidad;

                        ventaResult.Ventas.Add(venta);
                    }
                }
            }
            return View(ventaResult);
        }
        public IActionResult Tienda()
        {
            Producto producto = new Producto();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://192.168.0.136:3000/api/");
                var taskResponse = client.GetAsync("Producto");
                taskResponse.Wait();

                var resultService = taskResponse.Result;
                if (resultService.IsSuccessStatusCode)
                {
                    var readTask = resultService.Content.ReadAsStringAsync();
                    readTask.Wait();

                    dynamic jsonResult = JObject.Parse(readTask.Result.ToString());
                    producto.Productos = new List<object>();
                    foreach (var item in jsonResult.objects)
                    {
                        Producto prod = new Producto();
                        prod.IdProducto = item.idProducto;
                        prod.Nombre = item.nombre;
                        prod.Detalles = item.detalles;
                        prod.Precio = item.precio;
                        prod.Stock = item.stock;

                        producto.Productos.Add(prod);
                    }
                }
            }
            return View(producto);
        }
        public IActionResult Carrito()
        {
            Venta carrito = new Venta();
            carrito.Carrito = new List<object>();
            if(HttpContext.Session.GetString("carrito") == null)
            {
                return View(carrito);
            }
            else
            {
                GetCarrito(carrito);
                return View(carrito);
            }
        }
        public IActionResult AddCarrito(int idProducto)
        {
            bool existe = false;
            Venta venta = new Venta();
            venta.Carrito = new List<object>();
            Producto producto = GetByIdProducto(idProducto);
            if(HttpContext.Session.GetString("carrito") == null)
            {
                producto.Cantidad = 1;
                venta.Carrito.Add(producto);
                HttpContext.Session.SetString("carrito", Newtonsoft.Json.JsonConvert.SerializeObject(venta.Carrito));
            }
            else
            {
                GetCarrito(venta);
                foreach(Producto productoCarrito in venta.Carrito)
                {
                    if(producto.IdProducto == productoCarrito.IdProducto)
                    {
                        productoCarrito.Cantidad += 1;
                        existe = true;
                        break;
                    }
                    else
                    {
                        existe = false;
                    }
                }
                if (existe)
                {
                    HttpContext.Session.SetString("carrito", Newtonsoft.Json.JsonConvert.SerializeObject(venta.Carrito));
                }
                else
                {
                    producto.Cantidad += 1;
                    venta.Carrito.Add(producto);
                    HttpContext.Session.SetString("carrito", Newtonsoft.Json.JsonConvert.SerializeObject(venta.Carrito));
                }
            }
            return RedirectToAction("Tienda");
        }
        public IActionResult Comprar()
        {
            Venta venta = new Venta();
            venta.Carrito = new List<object>();
            GetCarrito(venta);
            foreach(Producto producto in venta.Carrito)
            {
                using(var client = new HttpClient())
                {
                    object nuevaVenta = new
                    {
                        IdProducto = producto.IdProducto,
                        Nombre = producto.Nombre,
                        Detalles = producto.Detalles,
                        Precio = producto.Precio,
                        Cantidad = producto.Cantidad
                    };
                    client.BaseAddress = new Uri("http://192.168.0.136:3010/api/");
                    var taskResponse = client.PostAsJsonAsync("Venta", nuevaVenta);
                    taskResponse.Wait();
                    if (taskResponse.Result.IsSuccessStatusCode)
                    {
                        ViewBag.Mensaje = "Producto(s) comprados correctamente.";
                    }
                    else
                    {
                        ViewBag.Mensaje = "Error al adquirir los productos.";
                    }
                }
            }
            HttpContext.Session.Clear();
            return PartialView("Modal");
        }
        public IActionResult ClearCarrito()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Carrito");
        }
        public Venta GetCarrito(Venta carrito)
        {
            var ventaSession = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(HttpContext.Session.GetString("carrito"));
            foreach(var obj in ventaSession)
            {
                Producto producto = Newtonsoft.Json.JsonConvert.DeserializeObject<Producto>(obj.ToString());
                carrito.Carrito.Add(producto);
            }
            return carrito;
        }
        public Producto GetByIdProducto(int idProducto)
        {
            Producto producto = new Producto();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://192.168.0.136:3000/api/");
                var taskResponse = client.GetAsync($"Producto/{idProducto}");
                taskResponse.Wait();

                var resultService = taskResponse.Result;
                if (resultService.IsSuccessStatusCode)
                {
                    var readTask = resultService.Content.ReadAsStringAsync();
                    readTask.Wait();

                    dynamic jsonResult = JObject.Parse(readTask.Result.ToString());

                    producto.IdProducto = jsonResult.objeto.idProducto;
                    producto.Nombre = jsonResult.objeto.nombre;
                    producto.Detalles = jsonResult.objeto.detalles;
                    producto.Precio = jsonResult.objeto.precio;
                    producto.Stock = jsonResult.objeto.stock;
                }
            }
            return producto;
        }
    }
}
