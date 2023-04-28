using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System.IO;
using System.ComponentModel.DataAnnotations;


namespace PetStay.Pages
{
    public class CrearPublicacionModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public CrearPublicacionModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string NombreUsuario { get; set; }
        public int IdUsuario { get; set; }
        public void OnGet(string nombreUsuario, int id)
        {
            NombreUsuario = nombreUsuario;
            IdUsuario = id;
        }

        [BindProperty]
        public Anuncio Anuncio { get; set; }

        [BindProperty]
        public IFormFile Imagen { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {

            var form = HttpContext.Request.Form;
            string nombreUsuario = HttpContext.Request.Form["NombreUsuario"];
            int idUsuario = int.Parse(HttpContext.Request.Form["IdUsuario"]);



            if (!ModelState.IsValid)
            {
                return RedirectToPage("/CrearPublicacion", new { nombreUsuario, idUsuario });
            }

            if (form.Files.GetFile("imagen") != null && form.Files.GetFile("imagen").Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await form.Files.GetFile("imagen").CopyToAsync(ms);
                    Anuncio.Imagen = ms.ToArray();
                }
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string query = "INSERT INTO Anuncio (titulo, descripcion, medioContacto, imgAnuncio, idTipoAnuncio, idUsuario, idEstado) " +
                           "VALUES (@titulo, @descripcion, @contacto, @imagen, @tipoPublicacionId, @usuarioId, @estadoId)";

            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@titulo", form["Anuncio.Titulo"]);
            command.Parameters.AddWithValue("@descripcion", form["Anuncio.Descripcion"]);
            command.Parameters.AddWithValue("@contacto", form["Anuncio.Contacto"]);
            command.Parameters.AddWithValue("@imagen", Anuncio.Imagen);

            command.Parameters.AddWithValue("@usuarioId", idUsuario);
            command.Parameters.AddWithValue("@estadoId", 1);

            // Asignar el valor de tipoPublicacionId según la opción seleccionada
            if (Request.Form["info"] == "Informacion")
            {
                command.Parameters.AddWithValue("@tipoPublicacionId", 1);
            }
            else if (Request.Form["info"] == "servicio")
            {
                command.Parameters.AddWithValue("@tipoPublicacionId", 2);
            }

            command.ExecuteNonQuery();

            connection.Close();
            return RedirectToPage("/User", new { nombreUsuario, idUsuario });
        }
    }

    public class Anuncio
    {
        public int Id { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public string Contacto { get; set; }

        public byte[] Imagen { get; set; }

        public string TipoPublicacion { get; set; }
    }
}
