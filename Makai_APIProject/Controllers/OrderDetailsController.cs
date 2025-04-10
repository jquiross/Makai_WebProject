using Dapper;
using Makai_APIProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Makai_APIProject.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : Controller
    {
        private readonly IConfiguration _configuration;
        public OrderDetailsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("ConsultarOrderDetails")]
        public IActionResult ConsultarOrderDetails(int Id)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Query<OrderDetailsModel>("ConsultarOrderDetails",
                    new { Id });

                var respuesta = new RespuestaModel();

                if (result != null)
                {
                    respuesta.Indicador = true;
                    respuesta.Datos = result;
                }
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "No hay información registrada";
                }

                return Ok(respuesta);
            }
        }

        [HttpPost]
        [Route("RegistrarOrderDetail")]
        public IActionResult RegistrarOrderDetail(OrderDetailsModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Execute("RegistrarOrderDetail",
                    new { model.order_detail_id, model.order_id, model.product_id, model.quantity, model.price });

                var respuesta = new RespuestaModel();

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "El detalle de la orden no se ha registrado correctamente";
                }

                return Ok(respuesta);
            }
        }

        [HttpPut]
        [Route("ActualizarOrderDetail")]
        public IActionResult ActualizarOrderDetail(OrderDetailsModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Execute("ActualizarOrderDetail",
                    new { model.order_detail_id, model.order_id, model.product_id, model.quantity, model.price });

                var respuesta = new RespuestaModel();

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "El detalle de la orden no se ha actualizado correctamente";
                }

                return Ok(respuesta);
            }
        }
    }
}

