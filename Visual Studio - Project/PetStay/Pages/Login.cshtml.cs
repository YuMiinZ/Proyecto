using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;


namespace PetStay.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public Usuario Usuario { get; set; }
        public string nombreUsuario { get; set; }
        public int idUsuario { get; set; }

        public void OnGet()
        {
            TempData["ErrorMessage"] = "";
            HttpContext.Session.Remove("nombreUsuario");
            HttpContext.Session.Remove("idUsuario");
        }

        public async Task<IActionResult> OnPostLoginAsync()
        {

            // Verificar si el usuario y la contrase�a existen en la base de datos
            // y si la contrase�a ingresada coincide con la contrase�a en la BD encriptada con MD5

            // Establecer una conexi�n con la base de datos
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new MySqlConnection(connectionString);


            // Encriptar la contrase�a ingresada por el usuario con MD5

            var md5 = MD5.Create();
            var passwordBytes = Encoding.ASCII.GetBytes(Usuario.Contrase�a);
            var hashedBytes = md5.ComputeHash(passwordBytes);
            var hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            // Construir una consulta SQL para verificar si el usuario y la contrase�a existen en la base de datos
            var query = $"SELECT idUsuario, nombre, correo, contrasenia, idTipoUsuario FROM Usuario WHERE Usuario.correo = '{Usuario.Correo}' " +
                $"AND Usuario.contrasenia = '{hashedPassword}'";

            // Ejecutar la consulta
            await connection.OpenAsync();
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            // Si la consulta devuelve un resultado mayor que cero, el usuario y la contrase�a existen en la base de datos
            if (await reader.ReadAsync())
            {
                HttpContext.Session.SetString("nombreUsuario", reader["nombre"].ToString());
                HttpContext.Session.SetString("idUsuario",reader["idUsuario"].ToString());


                
                int idTipoUsuario = (int)reader["idTipoUsuario"];

                if (idTipoUsuario != 1)
                {
                    // Redirigir a la p�gina de usuario
                    return RedirectToPage("/User");
                }
                else
                {
                    // Redirigir a la p�gina de administrador
                    return RedirectToPage("/Admin");
                }
            }

            // Si la consulta no devuelve ning�n resultado, mostrar un mensaje de error
            var errorMessage = "Los datos ingresados son incorrectos";
            TempData["ErrorMessage"] = errorMessage;
            TempData["Correo"] = Usuario.Correo;
            TempData["Contrase�a"] = Usuario.Contrase�a;


            return Page();
        }
    }

    public class Usuario
    {
        public string Correo { get; set; }
        public string Contrase�a { get; set; }
    }

}
