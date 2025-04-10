using Dapper;
using Makai_APIProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;


namespace Makai_APIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly IConfiguration _configuration;

        public CartController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // consultar productos en el carrito
        [HttpGet]
        [Route("GetCartItems")]
        public async Task<IActionResult> GetCartItems(int userId)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = await context.QueryAsync<CartItemModel>("GetCartItems",
                    new { UserId = userId },
                    commandType: System.Data.CommandType.StoredProcedure);

                var respuesta = new RespuestaModel();

                if (result != null)
                {
                    respuesta.Indicador = true;
                    respuesta.Datos = result;
                }
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "No hay productos en el carrito";
                }

                return Ok(respuesta);
            }
        }

        //  añadir un producto al carrito
        [HttpPost]
        [Route("AddToCart")]
        public async Task<IActionResult> AddToCart(CartItemModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                // Crear un objeto DynamicParameters para manejar los parámetros de la consulta
                var parameters = new DynamicParameters();
                parameters.Add("user_id", model.user_id);
                parameters.Add("product_id", model.product_id);
                parameters.Add("quantity", model.quantity);

                // Definir el parámetro de salida para el mensaje
                parameters.Add("message", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);

                // Ejecutar el procedimiento almacenado
                await context.ExecuteAsync(
                    "AddToCart", // Nombre del procedimiento
                    parameters,  // Parámetros de entrada y salida
                    commandType: CommandType.StoredProcedure
                );

                // Crear la respuesta con el valor del mensaje
                var respuesta = new RespuestaModel
                {
                    Mensaje = parameters.Get<string>("message") // Obtener el mensaje de salida
                };

                // Verificar el mensaje para determinar si la operación fue exitosa
                if (!string.IsNullOrEmpty(respuesta.Mensaje))
                {
                    respuesta.Indicador = true;
                    respuesta.Datos = new
                    {
                        ProductId = model.product_id,
                        Quantity = model.quantity
                    };
                }
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Datos = null;
                }

                // Devolver la respuesta en formato JSON
                return Ok(respuesta);
            }
        }



        //  eliminar un producto del carrito
        [HttpDelete]
        [Route("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                // Definir los parámetros para el procedimiento almacenado
                var parameters = new DynamicParameters();
                parameters.Add("cart_id", cartId);

                try
                {
                    // Ejecutar el procedimiento almacenado
                    var result = await context.ExecuteAsync(
                        "RemoveFromCart", // Procedimiento almacenado
                        parameters,       // Parámetros de entrada
                        commandType: CommandType.StoredProcedure
                    );

                    // Crear el objeto de respuesta
                    var respuesta = new RespuestaModel
                    {
                        Indicador = result > 0, // Si hay filas afectadas, el indicador será true
                        Mensaje = result > 0 ? "Producto eliminado del carrito correctamente." : "No se encontró el producto para eliminar.",
                        Datos = new { CartId = cartId }
                    };

                    // Devolver la respuesta
                    return Ok(respuesta);
                }
                catch (Exception ex)
                {
                    // Manejar cualquier error que ocurra durante la ejecución
                    return StatusCode(500, new RespuestaModel
                    {
                        Indicador = false,
                        Mensaje = $"Error interno del servidor: {ex.Message}"
                    });
                }
            }
        }







        // limpiar carrito
        [HttpDelete]
        [Route("ClearCart")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                // Ejecutar el procedimiento ClearCart para vaciar el carrito de un usuario
                var result = await context.ExecuteAsync(
                    "ClearCart", // Procedimiento para vaciar el carrito
                    new { UserId = userId }, // Parámetro para vaciar el carrito de un usuario específico
                    commandType: CommandType.StoredProcedure
                );

                var respuesta = new RespuestaModel();

                if (result > 0)
                {
                    respuesta.Indicador = true;
                    respuesta.Mensaje = "Carrito vacío con éxito.";
                }
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "No se pudo vaciar el carrito.";
                }

                return Ok(respuesta);
            }
        }

        //  crear una nueva orden
        [HttpPost]
        [Route("CrearOrden")]
        public async Task<IActionResult> CrearOrden(OrdersModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                model.created_at = DateTime.Now;

                var result = await context.ExecuteAsync("CrearOrden",
                    new { model.user_id, model.total_price, model.status, model.created_at },
                    commandType: System.Data.CommandType.StoredProcedure);

                var respuesta = new RespuestaModel();

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "No se pudo crear la orden";
                }

                return Ok(respuesta);
            }
        }
    }
}
