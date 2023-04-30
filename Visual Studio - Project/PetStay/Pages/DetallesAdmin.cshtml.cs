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
    public class DetallesAdminModel : PageModel
    {
        public int IdPublicacion { get; set; }

        public PublicacionActual Publicacion { get; set; }
        public List<ComentarioAdmin> ComentariosAdmin { get; set; }

        private readonly IConfiguration _configuration;

        public DetallesAdminModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task OnGet(int idPublicacion)
        {
            IdPublicacion = idPublicacion;

            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var query = "SELECT titulo, imgAnuncio, descripcion, Usuario.nombre, medioContacto, Anuncio.idTipoAnuncio,  TipoAnuncio.nombre, Anuncio.idAnuncio " +
                "FROM Anuncio " +
                "INNER JOIN Usuario ON Usuario.idUsuario = Anuncio.idUsuario " +
                "INNER JOIN TipoAnuncio ON Anuncio.idTipoAnuncio = TipoAnuncio.idTipoAnuncio " +
                $"WHERE idEstado = 1 and Anuncio.idAnuncio = '{IdPublicacion}'";


            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            if (reader.Read())
            {
                Publicacion = new PublicacionActual
                {
                    Titulo = reader.GetString(0),
                    Imagen = (byte[])reader["imgAnuncio"],
                    Descripcion = reader.GetString(2),
                    Autor = reader.GetString(3),
                    Contacto = reader.GetString(4),
                    IdPublicacion = reader.GetInt32(7)
                };
            }
            reader.Close();

            query = "SELECT texto, Usuario.nombre, idComentario from Comentario " +
                    "INNER JOIN Usuario ON Usuario.idUsuario = Comentario.idUsuario " +
                    "INNER JOIN Anuncio ON Anuncio.idAnuncio = Comentario.idAnuncio " +
                    $"WHERE Comentario.idEstado = 1 and Anuncio.idAnuncio = '{IdPublicacion}'";


            using var command2 = new MySqlCommand(query, connection);
            using var reader2 = await command2.ExecuteReaderAsync();

            ComentariosAdmin = new List<ComentarioAdmin>();
            while (await reader2.ReadAsync())
            {
                var comentario = new ComentarioAdmin
                {
                    Autor = reader2.GetString(1),
                    DetalleComentario = reader2.GetString(0),
                    IdComentario = reader2.GetInt32(2)
                };
                ComentariosAdmin.Add(comentario);
            }

            connection.Close();

        }

        public async Task<IActionResult> OnPostEliminarComentario()
        {
            var form = HttpContext.Request.Form;

            int idPublicacion = int.Parse(HttpContext.Request.Form["IdPublicacion"]);
            int idComentario = int.Parse(HttpContext.Request.Form["IdComentario"]);

            //Modifica el estado de la publicación "Eliminar"
            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var query = "UPDATE Comentario SET idEstado = 2 WHERE idComentario = @idComentario";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@idComentario", idComentario);
            await command.ExecuteNonQueryAsync();

            return RedirectToPage("/DetallesAdmin", new { idPublicacion });

        }


        public async Task<IActionResult> OnPostEliminarPublicacion()
        {
            var form = HttpContext.Request.Form;

            int idPublicacion = int.Parse(HttpContext.Request.Form["IdPublicacion"]);

            //Modifica el estado de la publicación "Eliminar"
            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var query = "UPDATE Anuncio SET idEstado = 2 WHERE idAnuncio = @idPublicacion";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@idPublicacion", idPublicacion);
            await command.ExecuteNonQueryAsync();

            return RedirectToPage("/Admin", new { idPublicacion });

        }


        public class PublicacionActual
        {
            public string TipoPublicacion { get; set; }
            public byte[] Imagen { get; set; }
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public string Autor { get; set; }
            public string Contacto { get; set; }
            public int IdPublicacion { get; set; }
        }
        public class ComentarioAdmin
        {
            public string Autor { get; set; }
            public string DetalleComentario { get; set; }
            public int IdComentario { get; set; }
        }
    }
}