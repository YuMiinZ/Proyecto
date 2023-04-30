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

namespace PetStay.Pages
{
    public class RegistroServicioModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        [BindProperty]
        public IFormFile Imagen { get; set; }
        [BindProperty]
        public string Nombre { get; set; }
        [BindProperty]
        public int Cedula { get; set; }
        [BindProperty]
        public string Provincia { get; set; }
        [BindProperty]
        public string Canton { get; set; }
        [BindProperty]
        public string Distrito { get; set; }
        [BindProperty]
        public string TelefonoPrincipal { get; set; }
        [BindProperty]
        public string TelefonoSecundario { get; set; }
        [BindProperty]
        public int IdTipoServicio { get; set; }
        [BindProperty]
        public int IdEstado { get; set; }

        public RegistroServicioModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "El documento de identidad debe de contener solo números";
                return Page();
            }

            // Extraer idUsuario de la sesión actual
            int idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));


            // Convertir imagen a un arreglo de bytes
            byte[] imagenBytes = null;
            using (var memoryStream = new MemoryStream())
            {
                await Imagen.CopyToAsync(memoryStream);
                imagenBytes = memoryStream.ToArray();
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Insertar la solicitud en la tabla Solicitud
                using (var command = new MySqlCommand(
                    "INSERT INTO Solicitud (nombre, cedula, provincia, canton, distrito, " +
                    "telefonoPrincipal, telefonoSecundario, imgDocIdentificacion, idTipoServicio, idEstado, idUsuario) " +
                    "VALUES (@nombre, @cedula, @provincia, @canton, @distrito, " +
                    "@telefonoPrincipal, @telefonoSecundario, @imgDocIdentificacion, @idTipoServicio, @idEstado, @idUsuario);",
                    connection))
                {
                    command.Parameters.AddWithValue("@nombre", Request.Form["nombre"]);
                    command.Parameters.AddWithValue("@cedula", int.Parse(Request.Form["cedula"]));
                    command.Parameters.AddWithValue("@provincia", Request.Form["provincia"]);
                    command.Parameters.AddWithValue("@canton", Request.Form["canton"]);
                    command.Parameters.AddWithValue("@distrito", Request.Form["distrito"]);
                    command.Parameters.AddWithValue("@telefonoPrincipal", Request.Form["telefonoPrincipal"]);
                    command.Parameters.AddWithValue("@telefonoSecundario", Request.Form["telefonoSecundario"]);
                    command.Parameters.AddWithValue("@imgDocIdentificacion", imagenBytes);
                    command.Parameters.AddWithValue("@idEstado", 1);
                    command.Parameters.AddWithValue("@idUsuario", idUsuario);

                    // Asignar el valor de tipoPublicacionId según la opción seleccionada
                    if (Request.Form["info"] == "Cuido")
                    {
                        command.Parameters.AddWithValue("@idTipoServicio", 1);
                    }
                    else if (Request.Form["info"] == "Hospedaje")
                    {
                        command.Parameters.AddWithValue("@idTipoServicio", 2);
                    }


                    await command.ExecuteNonQueryAsync();
                    connection.Close();

                    TempData["SuccessMessage"] = "La solicitud se ha registrado exitosamente";

                    return Redirect("~/RegistroServicio?exito=true");
                }
            }
        }
    }

}