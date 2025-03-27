using Dapper;
using Makai_APIProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
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
        [Route("ConsultarCart")]
        public async Task<IActionResult> ConsultarCart(int userId)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = await context.QueryAsync<CartItemModel>("ConsultarCart",
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

        //  agregar un producto al carrito
        [HttpPost]
        [Route("AgregarAlCarrito")]
        public async Task<IActionResult> AgregarAlCarrito(CartItemModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                model.added_at = DateTime.Now;

                var result = await context.ExecuteAsync("AgregarProductoCarrito",
                    new { model.user_id, model.product_id, model.quantity, model.added_at },
                    commandType: System.Data.CommandType.StoredProcedure);

                var respuesta = new RespuestaModel();

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "No se pudo agregar el producto al carrito";
                }

                return Ok(respuesta);
            }
        }

        //  eliminar un producto del carrito
        [HttpDelete]
        [Route("EliminarProductoCarrito")]
        public async Task<IActionResult> EliminarProductoCarrito(int cartId)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = await context.ExecuteAsync("EliminarProductoCarrito",
                    new { CartId = cartId },
                    commandType: System.Data.CommandType.StoredProcedure);

                var respuesta = new RespuestaModel();

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "No se pudo eliminar el producto del carrito";
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
