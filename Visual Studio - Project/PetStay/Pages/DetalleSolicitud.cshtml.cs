using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace PetStay.Pages
{
    public class DetalleSolicitudModel : PageModel
    {
        public int IdSolicitud { get; set; }
        public int IdTipoServicio { get; set; }
        public int IdUsuario { get; set; }

        public SolicitudActual solicitud { get; set; }

        private readonly IConfiguration _configuration;

        public DetalleSolicitudModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task OnGet()
        {
            IdSolicitud = int.Parse(Request.Query["idSolicitud"]);
            IdTipoServicio = int.Parse(Request.Query["idTipoServicio"]);
            IdUsuario = int.Parse(Request.Query["idUsuario"]);

            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var query = "SELECT Solicitud.nombre, Solicitud.cedula, Solicitud.provincia, Solicitud.canton, Solicitud.distrito, " +
                        "Solicitud.telefonoPrincipal, Solicitud.telefonoSecundario, Solicitud.imgDocIdentificacion, TipoServicio.nombre " +
                        "FROM Solicitud INNER JOIN TipoServicio ON TipoServicio.idTipoServicio = Solicitud.idTipoServicio " +
                        $"WHERE Solicitud.idEstado = 1 and Solicitud.idSolicitud = '{IdSolicitud}'";




            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            if (reader.Read())
            {
                solicitud = new SolicitudActual
                {
                    Nombre = reader.GetString(0),
                    ImgDocIdentificacion = (byte[])reader["imgDocIdentificacion"],
                    Cedula = reader.GetInt32(1),
                    Provincia = reader.GetString(2),
                    Canton = reader.GetString(3),
                    Distrito = reader.GetString(4),
                    TelefonoPrincipal = reader.GetString(5),
                    TelefonoSecundario = reader.GetString(6),
                    TipoServicio = reader.GetString(8)
                };
            }
            reader.Close();
        }

        public async Task<IActionResult> OnPostRechazarAsync()
        {
            var form = HttpContext.Request.Form;
            int idSolicitud = int.Parse(HttpContext.Request.Form["IdSolicitud"]);
            //Modifica el estado de la publicación "Eliminar"
            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var query = "UPDATE Solicitud SET idEstado = 2 WHERE Solicitud.idSolicitud = @idSolicitud";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@idSolicitud", idSolicitud);
            await command.ExecuteNonQueryAsync();


            return RedirectToPage("/GestSolicitudes");

        }

        public async Task<IActionResult> OnPostAceptarAsync()
        {
            var form = HttpContext.Request.Form;

            int idTipoServicio = int.Parse(HttpContext.Request.Form["IdTipoServicio"]);
            int idUsuario = int.Parse(HttpContext.Request.Form["IdUsuario"]);
            int idTipoUsuario;

            if (idTipoServicio == 1)
            {
                idTipoUsuario = 3;
            }
            else
            {
                idTipoUsuario = 4;
            }

            Console.WriteLine("Tipo Usuario "+idTipoUsuario+" ID USUARIO: "+idUsuario);
            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var updateUsuarioQuery = $"UPDATE Usuario SET idTipoUsuario = {idTipoUsuario} WHERE idUsuario = {idUsuario}";
            using var command = new MySqlCommand(updateUsuarioQuery, connection);
            await command.ExecuteNonQueryAsync();

            var updateSolicitudQuery = $"UPDATE Solicitud SET idEstado = 2 WHERE idSolicitud = {int.Parse(HttpContext.Request.Form["IdSolicitud"])}";
            using var command2 = new MySqlCommand(updateSolicitudQuery, connection);
            await command2.ExecuteNonQueryAsync();




            return RedirectToPage("/GestSolicitudes");

        }

        public class SolicitudActual
        {
            public string Nombre { get; set; }
            public int Cedula { get; set; }
            public string Provincia { get; set; }
            public string Canton { get; set; }
            public string Distrito { get; set; }
            public string TelefonoPrincipal { get; set; }
            public string TelefonoSecundario { get; set; }
            public byte[] ImgDocIdentificacion { get; set; }
            public string TipoServicio { get; set; }
        }
    }
}