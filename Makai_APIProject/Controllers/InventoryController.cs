using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Makai_APIProject.Models;
using System;
using Microsoft.Data.SqlClient;


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

        // GET
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> Get()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = "SELECT inventory_id, product_id, stock, last_updated FROM Inventory";
            var result = await connection.QueryAsync<Inventory>(query);

            return Ok(result);
        }

        // GET
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> Get(int id)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = "SELECT inventory_id, product_id, stock, last_updated FROM Inventory WHERE inventory_id = @Id";
            var result = await connection.QuerySingleOrDefaultAsync<Inventory>(query, new { Id = id });

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Inventory inventory)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            
            var query = @"
                INSERT INTO Inventory (product_id, stock)
                VALUES (@product_id, @stock);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            
            var newId = await connection.ExecuteScalarAsync<int>(query, inventory);
            inventory.inventory_id = newId;

            return CreatedAtAction(nameof(Get), new { id = newId }, inventory);
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Inventory inventory)
        {
            if (id != inventory.inventory_id)
                return BadRequest("El ID de la ruta no coincide con el ID del objeto.");

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

           
            var query = @"
                UPDATE Inventory
                SET product_id = @product_id,
                    stock = @stock,
                    last_updated = GETDATE()
                WHERE inventory_id = @inventory_id
            ";

            var rowsAffected = await connection.ExecuteAsync(query, inventory);
            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }

        // DELETE =borrar 
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var query = "DELETE FROM Inventory WHERE inventory_id = @Id";
            var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });

            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }
    }
}
