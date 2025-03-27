using Dapper;
using Makai_APIProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Makai_APIProject.Controllers
{
   

        [AllowAnonymous]
        [Route("api/[controller]")]
        [ApiController]
        public class LoginController : ControllerBase
        {
            private readonly IConfiguration _configuration;
            public LoginController(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            [HttpPost]
            [Route("RegistrarCuenta")]
            public IActionResult RegistrarCuenta(UsersModel model)
            {
                using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
                {
                    var result = context.Execute("RegistrarCuenta",
                        new { model.email, model.name, model.password, model.address ,model.phone,model.role_id});

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

            [HttpPost]
            [Route("IniciarSesion")]
            public IActionResult IniciarSesion(UsersModel model)
            {
                using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
                {
                    var result = context.QueryFirstOrDefault<UsersModel>("IniciarSesion",
                        new { model.email, model.password });

                    var respuesta = new RespuestaModel();

                    if (result != null)
                    {
                        result.Token = GenerarToken(result.user_id, result.role_id);

                        respuesta.Indicador = true;
                        respuesta.Datos = result;
                    }
                    else
                    {
                        respuesta.Indicador = false;
                        respuesta.Mensaje = "Su información no se ha validado correctamente";
                    }

                    return Ok(respuesta);
                }
            }

            //Recuperar la contraseña

            private string GenerarToken(long user_id,long role_id)
            {
                string SecretKey = _configuration.GetSection("Variables:llaveToken").Value!;

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("Id", user_id.ToString()));
                claims.Add(new Claim("Role", role_id.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(20),
                    signingCredentials: cred);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

        }

    }
        

