using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;


namespace PetStay.Pages
{
    public class RecuperarContrasenaModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public RecuperarContrasenaModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [EmailAddress]
        public string Correo { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Establecer una conexión con la base de datos
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using var connection = new MySqlConnection(connectionString);

                // Construir una consulta SQL para verificar si el usuario existe en la base de datos
                var query = $"SELECT COUNT(*) FROM Usuario WHERE Usuario.correo = '{Request.Form["Correo"]}'";

                // Ejecutar la consulta para verificar si el usuario existe
                await connection.OpenAsync();
                using var command = new MySqlCommand(query, connection);
                var count = Convert.ToInt32(await command.ExecuteScalarAsync());

                if (count > 0)
                {
                    // Encriptar la nueva contraseña ingresada por el usuario con MD5
                    var md5 = MD5.Create();
                    var passwordBytes = Encoding.ASCII.GetBytes(Request.Form["Contraseña"]);
                    var hashedBytes = md5.ComputeHash(passwordBytes);
                    var hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                    // Construir una consulta SQL para actualizar la contraseña del usuario
                    var updateQuery = $"UPDATE Usuario SET Usuario.contrasenia = '{hashedPassword}' WHERE Usuario.correo = '{Request.Form["Correo"]}'";

                    // Ejecutar la consulta para actualizar la contraseña del usuario
                    using var updateCommand = new MySqlCommand(updateQuery, connection);
                    await updateCommand.ExecuteNonQueryAsync();

                    // Mostrar mensaje de éxito
                    TempData["SuccessMessage"] = "Contraseña modificada exitosamente";
                    return Redirect("~/RecuperarContrasena?exito=true");
                }
                else
                {
                    // Mostrar mensaje de error si el usuario no existe
                    TempData["ErrorMessage"] = "El correo no se encuentra registrado en la base de datos";
                    TempData["Correo"] = Request.Form["Correo"];
                    return Page();
                }
            }

            // Si el modelo no es válido, se mostrarán los mensajes de validación en la página
            return Page();
        }

    }
}
