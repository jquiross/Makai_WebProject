using Makai_WebProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Makai_WebProject.Controllers
{
    public class ProductsController : Controller
    {
        private readonly string _apiBaseUrl;

        public ProductsController(IConfiguration configuration)
        {
          
            _apiBaseUrl = configuration["ApiSettings:InventoryBaseUrl"];
         
        }

       
        //Listado de Productos y Stock
        
        public async Task<IActionResult> Index(string search)
        {
            List<ProductInventoryViewModel> products = new List<ProductInventoryViewModel>();

            using (HttpClient client = new HttpClient())
            {
                // Se asigna la URL base del API (debe venir desde la configuración)
                client.BaseAddress = new Uri(_apiBaseUrl);
                try
                {
                    HttpResponseMessage response = await client.GetAsync("");
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        products = JsonConvert.DeserializeObject<List<ProductInventoryViewModel>>(jsonData);

                        // Filtro opcional por product_id (convertido a string)
                        if (!string.IsNullOrWhiteSpace(search))
                        {
                            products = products.FindAll(p => p.product_id.ToString().Contains(search));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo obtener la información de productos desde el API. Código: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al llamar al API: " + ex.Message);
                }
            }
            
            ViewBag.ApiBaseUrl = _apiBaseUrl;
            return View(products);
        }

       
        //Detalle del Productoo
       
        public async Task<IActionResult> Details(int id)
        {
            ProductInventoryViewModel product = null;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseUrl);
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        product = JsonConvert.DeserializeObject<ProductInventoryViewModel>(jsonData);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al llamar al API: " + ex.Message);
                    return NotFound();
                }
            }
            return View(product);
        }

        
        //Crear Nuevo Producto
       
        public IActionResult Create()
        {
            return View();
        }

       
        //Crear Nuevo Producto
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductInventoryViewModel product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseUrl);
                try
                {
                    
                    string jsonData = JsonConvert.SerializeObject(product);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("", content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo crear el producto. Código: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al llamar al API: " + ex.Message);
                }
            }
            return View(product);
        }

      
        //Editar/Actualizar Producto
       
        public async Task<IActionResult> Edit(int id)
        {
            ProductInventoryViewModel product = null;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseUrl);
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        product = JsonConvert.DeserializeObject<ProductInventoryViewModel>(jsonData);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al llamar al API: " + ex.Message);
                    return NotFound();
                }
            }
            return View(product);
        }

      
        //Editar/Actualizar Producto
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductInventoryViewModel product)
        {
            if (id != product.inventory_id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseUrl);
                try
                {
                    string jsonData = JsonConvert.SerializeObject(product);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync($"{id}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo actualizar el producto. Código: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al llamar al API: " + ex.Message);
                }
            }
            return View(product);
        }

    
        //Eliminar Producto
      
        public async Task<IActionResult> Delete(int id)
        {
            ProductInventoryViewModel product = null;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseUrl);
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        product = JsonConvert.DeserializeObject<ProductInventoryViewModel>(jsonData);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al llamar al API: " + ex.Message);
                    return NotFound();
                }
            }
            return View(product);
        }

       
        //Eliminar Producto
     
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseUrl);
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync($"{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo eliminar el producto. Código: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al llamar al API: " + ex.Message);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
