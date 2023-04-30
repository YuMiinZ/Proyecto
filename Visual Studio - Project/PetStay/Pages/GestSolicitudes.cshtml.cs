using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace PetStay.Pages
{
    public class GestSolicitudesModel : PageModel
    {
        public Solicitud solicitud { get; set; }
        public List<Solicitud> Solicitudes { get; set; }

        private readonly IConfiguration _configuration;

        public GestSolicitudesModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGet()
        {
            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var query = "SELECT Solicitud.nombre, Solicitud.cedula, TipoServicio.nombre, Solicitud.idSolicitud, Solicitud.idTipoServicio, Solicitud.idUsuario from Solicitud "+
                "INNER JOIN TipoServicio ON TipoServicio.idTipoServicio = Solicitud.idTipoServicio " +
                $"WHERE Solicitud.idEstado = 1";


            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            Solicitudes = new List<Solicitud>();

            while (await reader.ReadAsync())
            {
                solicitud = new Solicitud
                {
                    Nombre = reader.GetString(0),
                    Cedula = reader.GetInt32(1),
                    TipoServicio = reader.GetString(2),
                    IdSolicitud = reader.GetInt32(3),
                    IdTipoServicio = reader.GetInt32(4),
                    IdUsuario = reader.GetInt32(5)
                };
                Solicitudes.Add(solicitud);
            }
            reader.Close();
        }
    }

    public class Solicitud
    {
        public string Nombre { get; set; }
        public int Cedula { get; set; }
        public string TipoServicio { get; set; }
        public int IdSolicitud { get; set; }
        public int IdTipoServicio { get; set; }
        public int IdUsuario { get; set; }
    }
}