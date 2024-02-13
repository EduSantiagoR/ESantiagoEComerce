using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PL.Models;

namespace PL.Controllers
{
    public class ProductoController : Controller
    {
        public IActionResult GetAll()
        {
            Producto producto = new Producto();
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://192.168.0.136:3000/api/");
                var taskResponse = client.GetAsync("Producto");
                taskResponse.Wait();

                var resultService = taskResponse.Result;
                if(resultService.IsSuccessStatusCode)
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
        public IActionResult Form(int? idProducto)
        {
            Producto producto = new Producto();
            if (idProducto != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://192.168.0.136:3000/api/");
                    var taskResponse = client.GetAsync($"Producto/{idProducto.Value}");
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
            }
            return View(producto);
        }
        [HttpPost]
        public IActionResult Form(Producto producto)
        {
            if(producto.IdProducto == 0)
            {
                ViewBag.Mensaje = Add(producto) ? "Producto agregado correctamente." : "Error al registrar el producto.";
            }
            else
            {
                ViewBag.Mensaje = Update(producto) ? "Producto actualizado correctamente." : "Error al actualizar el producto.";
            }
            return PartialView("Modal");
        }
        [HttpGet]
        public IActionResult Delete(int idProducto)
        {
            ViewBag.Mensaje = DeleteProducto(idProducto) ? "Producto eliminado correctamente." : "Error al eliminar el producto.";
            return PartialView("Modal");
        }

        bool Add(Producto producto)
        {
            bool correct = false;
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://192.168.0.136:3000/api/");
                var taskResponse = client.PostAsJsonAsync("Producto", producto);
                taskResponse.Wait();

                var resultService = taskResponse.Result;
                if (resultService.IsSuccessStatusCode)
                {
                    correct = true;
                }
            }
            return correct;
        }
        bool Update(Producto producto)
        {
            bool correct = false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://192.168.0.136:3000/api/");
                var taskResponse = client.PutAsJsonAsync($"Producto/{producto.IdProducto}", producto);
                taskResponse.Wait();

                var resultService = taskResponse.Result;
                if (resultService.IsSuccessStatusCode)
                {
                    correct = true;
                }
            }
            return correct;
        }
        bool DeleteProducto(int idProducto)
        {
            bool correct = false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://192.168.0.136:3000/api/");
                var taskResponse = client.DeleteAsync($"Producto/{idProducto}");
                taskResponse.Wait();

                var resultService = taskResponse.Result;
                if (resultService.IsSuccessStatusCode)
                {
                    correct = true;
                }
            }
            return correct;
        }
    }
}
