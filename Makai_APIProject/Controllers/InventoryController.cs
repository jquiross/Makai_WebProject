using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Makai_APIProject.Models;
using System;
using Microsoft.Data.SqlClient;
using System.Data;


namespace Makai_APIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public InventoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("IngresarInventario")]
        public IActionResult IngresarInventario(Inventory model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Execute("IngresarInventario",
                    new { model.Category_id, model.Name, model.Description, model.Price, model.Stock_quantity, model.Created_at, model.Image_url });

                var respuesta = new RespuestaModel();

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "Su información no se ha registrado correctamente";
                }

                return Ok(respuesta);
            }
        }


        [HttpGet]
        [Route("ObtenerInventario")]
        public IActionResult ObtenerInventario()
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Query<Inventory>(
                    "ObtenerInventario",
                    commandType: CommandType.StoredProcedure
                ).ToList();


              

                return Ok(result);
            }
        }

        [HttpDelete]
        [Route("EliminarProducto")]
        public IActionResult EliminarProducto(Inventory model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Execute(
                    "EliminarProducto", new { model.Product_id });


                var respuesta = new RespuestaModel();

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "Exito";
                }

                return Ok(respuesta);
            }
        }

        [HttpPut]
        [Route("EditarInventario")]
        public IActionResult EditarInventario(Inventory model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Execute(
                    "EditarInventario", new { model.Product_id,model.Category_id, model.Name, model.Description, model.Price, model.Stock_quantity, model.Created_at, model.Image_url });


                var respuesta = new RespuestaModel();

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "Exito";
                }

                return Ok(respuesta);
            }
        }


    }
}
