using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace PetStay.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Usuario Usuario { get; set; }
        public string nombreUsuario { get; set; }

        public void OnGet()
        {
            TempData["ErrorMessage"] = "";
        }

        public async Task<IActionResult> OnPostLoginAsync()
        {

            if (string.IsNullOrEmpty(Usuario.Correo) || string.IsNullOrEmpty(Usuario.Contraseña))
            {
                ModelState.AddModelError("", "Debe ingresar su correo electrónico y su contraseña");
                return Page();
            }
            // Verificar si el usuario y la contraseña existen en la base de datos
            // y si la contraseña ingresada coincide con la contraseña en la BD encriptada con MD5

            // Establecer una conexión con la base de datos
            using var connection = new MySqlConnection("Server=localhost;Port=3306;Database=PetStay;Uid=root;Pwd=1234;");

            // Encriptar la contraseña ingresada por el usuario con MD5

            var md5 = MD5.Create();
            var passwordBytes = Encoding.ASCII.GetBytes(Usuario.Contraseña);
            var hashedBytes = md5.ComputeHash(passwordBytes);
            var hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            // Construir una consulta SQL para verificar si el usuario y la contraseña existen en la base de datos
            var query = $"SELECT nombre, correo, contrasenia, idTipoUsuario FROM Usuario WHERE Usuario.correo = '{Usuario.Correo}' AND Usuario.contrasenia = '{hashedPassword}'";

            // Ejecutar la consulta
            await connection.OpenAsync();
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            // Si la consulta devuelve un resultado mayor que cero, el usuario y la contraseña existen en la base de datos
            if (await reader.ReadAsync())
            {
                nombreUsuario = reader["nombre"].ToString();
                int idTipoUsuario = (int)reader["idTipoUsuario"];

                if (idTipoUsuario == 2)
                {
                    // Redirigir a la página de usuario
                    return RedirectToPage("/User", new { nombreUsuario });
                }
                else
                {
                    // Redirigir a la página de administrador
                    return RedirectToPage("/Admin");
                }
            }

            // Si la consulta no devuelve ningún resultado, mostrar un mensaje de error
            var errorMessage = "Los datos ingresados son incorrectos";
            TempData["ErrorMessage"] = errorMessage;
            TempData["Correo"] = Usuario.Correo;
            TempData["Contraseña"] = Usuario.Contraseña;


            return Page();
        }
    }

    public class Usuario
    {
        [Required(ErrorMessage = "Debe ingresar su correo electrónico")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Debe ingresar su contraseña")]
        public string Contraseña { get; set; }
    }

}
