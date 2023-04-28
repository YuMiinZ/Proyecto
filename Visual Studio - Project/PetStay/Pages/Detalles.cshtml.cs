using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace PetStay.Pages
{
    public class DetallesModel : PageModel
    {
        public string NombreUsuario { get; set; }
        public int IdUsuario { get; set; }
        public int IdPublicacion { get; set; }
        
        public PublicacionActual Publicacion { get; set; }
        public List<Comentario> Comentarios { get; set; }

        private readonly IConfiguration _configuration;

        public DetallesModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        [Required(ErrorMessage = "El campo Comentario es requerido.")]
        public string comentario { get; set; }

        public async Task OnGet(int idPublicacion, string nombreUsuario, int id)
        {
            IdPublicacion = idPublicacion;
            NombreUsuario = nombreUsuario;
            IdUsuario = id;

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

            query = "SELECT texto, Usuario.nombre from Comentario " +
                    "INNER JOIN Usuario ON Usuario.idUsuario = Comentario.idUsuario " +
                    "INNER JOIN Anuncio ON Anuncio.idAnuncio = Comentario.idAnuncio " +
                    $"WHERE Anuncio.idEstado = 1 and Anuncio.idAnuncio = '{IdPublicacion}'";


            using var command2 = new MySqlCommand(query, connection);
            using var reader2 = await command2.ExecuteReaderAsync();

            Comentarios = new List<Comentario>();
            while (await reader2.ReadAsync())
            {
                var comentario = new Comentario
                {
                    Autor = reader2.GetString(1),
                    DetalleComentario = reader2.GetString(0),
                };
                Comentarios.Add(comentario);
            }

            connection.Close();

        }


        public async Task<IActionResult> OnPostComentar()
        {
            var form = HttpContext.Request.Form;
            string nombreUsuario = HttpContext.Request.Form["NombreUsuario"];
            int idUsuario = int.Parse(HttpContext.Request.Form["IdUsuario"]);
            int idPublicacion = int.Parse(HttpContext.Request.Form["IdPublicacion"]);
            // Validar el comentario
            if (string.IsNullOrEmpty(comentario))
            {
                ModelState.AddModelError("comentario", "El comentario es obligatorio");
            }
            else
            {
                

                // Insertar el comentario en la base de datos
                using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var query = "INSERT INTO Comentario (texto, fecha, idUsuario, idAnuncio, idEstado) " +
                            "VALUES (@texto, @fecha, @idUsuario, @idAnuncio, @idEstado)";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@texto", comentario);
                command.Parameters.AddWithValue("@fecha", DateTime.Now);
                command.Parameters.AddWithValue("@idUsuario", idUsuario);
                command.Parameters.AddWithValue("@idAnuncio", idPublicacion);
                command.Parameters.AddWithValue("@idEstado", 1);

                await command.ExecuteNonQueryAsync();

                return RedirectToPage("/Detalles", new { idPublicacion, nombreUsuario, idUsuario });
            }

            return RedirectToPage("/Detalles", new { idPublicacion, nombreUsuario, idUsuario });

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
        public class Comentario
        {
            public string Autor { get; set; }
            public string DetalleComentario { get; set; }
        }
    }
}