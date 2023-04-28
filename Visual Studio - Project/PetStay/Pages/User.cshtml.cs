using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;


namespace PetStay.Pages
{
    public class UserModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public UserModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string nombreUsuario { get; set; }
        public int idUsuario { get; set; }
        public List<Publicacion> Publicaciones { get; set; }

        public async Task OnGetAsync(string NombreUsuario, int IdUsuario)
        {
            nombreUsuario = NombreUsuario;
            idUsuario = IdUsuario;

            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var query = "SELECT titulo, imgAnuncio, descripcion, Usuario.nombre, medioContacto, Anuncio.idTipoAnuncio,  TipoAnuncio.nombre, Anuncio.idAnuncio " +
            "FROM Anuncio " +
            "INNER JOIN Usuario ON Usuario.idUsuario = Anuncio.idUsuario " +
            "INNER JOIN TipoAnuncio ON Anuncio.idTipoAnuncio = TipoAnuncio.idTipoAnuncio WHERE idEstado = 1";


            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            Publicaciones = new List<Publicacion>();

            while (await reader.ReadAsync())
            {
                var publicacion = new Publicacion
                {
                    TipoPublicacion = reader.GetString(6),
                    Imagen = (byte[])reader.GetValue(1),
                    Titulo = reader.GetString(0),
                    Autor = reader.GetString(3),
                    Contacto = reader.GetString(4),
                    IdPublicacion = reader.GetInt32(7)
                };

                Publicaciones.Add(publicacion);
            }
            connection.Close();

        }

        public class Publicacion
        {
            public string TipoPublicacion { get; set; }
            public byte[] Imagen { get; set; }
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public string Contacto { get; set; }
            public int IdPublicacion { get; set; }
        }
    }

}