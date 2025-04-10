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

        #region añadir un producto al carrito

        [HttpPost]
        [Route("AddToCart")]
        public async Task<IActionResult> AddToCart(CartItemModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {

                var parameters = new DynamicParameters();
                parameters.Add("user_id", model.user_id);
                parameters.Add("product_id", model.product_id);
                parameters.Add("quantity", model.quantity);


                parameters.Add("message", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);


                await context.ExecuteAsync(
                    "AddToCart", // Nombre del procedimiento
                    parameters,  // Parámetros de entrada y salida
                    commandType: CommandType.StoredProcedure
                );


                var respuesta = new RespuestaModel
                {
                    Mensaje = parameters.Get<string>("message")
                };


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

                // json
                return Ok(respuesta);
            }
        }
        #endregion

        #region actualizar cantidad de producto

        [HttpPut]
        [Route("UpdateCartQuantity")]
        public async Task<IActionResult> UpdateCartQuantity(int cart_id, int quantity)
        {
            var respuesta = new RespuestaModel();

            try
            {
                using (var connection = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
                {
                    var result = await connection.ExecuteAsync(
                        "UpdateCartQuantity",
                        new { cart_id, quantity },
                        commandType: CommandType.StoredProcedure
                    );

                    respuesta.Indicador = true;
                    respuesta.Mensaje = quantity <= 0
                        ? "Producto eliminado del carrito."
                        : "Cantidad actualizada en el carrito.";
                    respuesta.Datos = new { cart_id, quantity };

                    return Ok(respuesta);
                }
            }
            catch (Exception ex)
            {
                respuesta.Indicador = false;
                respuesta.Mensaje = $"Error interno del servidor: {ex.Message}";
                respuesta.Datos = null;
                return StatusCode(500, respuesta);
            }
        }
        #endregion

        #region eliminar un producto del carrito

        [HttpDelete]
        [Route("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart(int cart_id)
        {
            var respuesta = new RespuestaModel();

            try
            {
                using (var connection = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            await connection.ExecuteAsync(
                                "RemoveFromCart",
                                new { cart_id },
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction
                            );

                            transaction.Commit();

                            respuesta.Indicador = true;
                            respuesta.Mensaje = "Producto eliminado del carrito correctamente.";
                            respuesta.Datos = new { cart_id };
                        }
                        catch (SqlException ex) when (ex.Number == 50000)
                        {
                            transaction.Rollback();

                            respuesta.Indicador = false;
                            respuesta.Mensaje = ex.Message;
                            respuesta.Datos = new { cart_id };

                            return NotFound(respuesta);
                        }
                    }
                }

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Indicador = false;
                respuesta.Mensaje = $"Error interno del servidor: {ex.Message}";
                respuesta.Datos = null;
                return StatusCode(500, respuesta);
            }
        }
        #endregion

        #region limpiar carrito

        [HttpDelete("ClearCart")]
        public async Task<ActionResult<RespuestaModel>> ClearCart(int userId)
        {
            var respuesta = new RespuestaModel();
            try
            {
                // Obtener la cadena de conexión desde appsettings.json
                var connectionString = _configuration.GetSection("ConnectionStrings:BDConnection").Value;

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Definir los parámetros para el procedimiento almacenado
                    var parameters = new { user_id = userId };

                    // Ejecutar el procedimiento almacenado
                    var result = await connection.ExecuteAsync("ClearCart", parameters, commandType: CommandType.StoredProcedure);

                    if (result > 0)
                    {
                        respuesta.Indicador = true;
                        respuesta.Mensaje = "Carrito limpiado con éxito.";
                    }
                    else
                    {
                        respuesta.Indicador = false;
                        respuesta.Mensaje = "No se encontraron elementos en el carrito para el usuario.";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Indicador = false;
                respuesta.Mensaje = $"Error al procesar la solicitud: {ex.Message}";
            }

            return Ok(respuesta);
        }
        #endregion

        #region detalle de productos en el carrito

        [HttpGet]
        [Route("GetCartItems")]
        public async Task<IActionResult> GetCartItems(int user_id)
        {
            var respuesta = new RespuestaModel();

            try
            {
                using (var connection = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
                {
                    var cartItems = await connection.QueryAsync(
                        "GetCartItems",
                        new { user_id },
                        commandType: CommandType.StoredProcedure
                    );

                    if (cartItems.Any())
                    {
                        respuesta.Indicador = true;
                        respuesta.Mensaje = "Productos encontrados en el carrito.";
                        respuesta.Datos = cartItems;
                    }
                    else
                    {
                        respuesta.Indicador = false;
                        respuesta.Mensaje = "El carrito está vacío.";
                        respuesta.Datos = null;
                    }

                    return Ok(respuesta);
                }
            }
            catch (Exception ex)
            {
                respuesta.Indicador = false;
                respuesta.Mensaje = $"Error interno del servidor: {ex.Message}";
                respuesta.Datos = null;
                return StatusCode(500, respuesta);
            }
        }
        #endregion

        #region crear una nueva orden

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
        #endregion
    }
}
