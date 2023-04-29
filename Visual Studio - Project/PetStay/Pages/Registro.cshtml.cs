using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace PetStay.Pages
{
    public class RegistroModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public RegistroModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            string nombre = Request.Form["nombre"];
            string apellidos = Request.Form["apellidos"];
            string correo = Request.Form["correo"];
            string contrase�a = Request.Form["contrase�a"];
            string confirmarContrase�a = Request.Form["contrase�aConfirmada"];

            if (!IsValidEmail(correo))
            {
                TempData["ErrorMessage"] = "El formato del correo es incorrecto";
                TempData["Correo"] = Request.Form["Correo"];
                return Page();
            }

            if (contrase�a != confirmarContrase�a)
            {
                TempData["ErrorMessage"] = "Las claves de acceso no coinciden";
                TempData["Contrase�a"] = Request.Form["Contrase�a"];
                return Page();
            }

            // Encriptar la contrase�a con MD5
            using (var md5 = MD5.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(contrase�a);
                byte[] hash = md5.ComputeHash(passwordBytes);

                // Convertir el hash a un string hexadecimal
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Verificar si el correo ya est� registrado
                    string query = "SELECT COUNT(*) FROM Usuario WHERE correo = @correo";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    command.Parameters.AddWithValue("@correo", correo);
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    if (count > 0)
                    {
                        TempData["ErrorMessage"] = "El correo proporcionado ya existe en la base de datos";
                        TempData["Correo"] = Request.Form["Correo"];
                        return Page();
                    }

                    // Insertar usuario en la base de datos
                    query = "INSERT INTO Usuario (nombre, apellido, correo, contrasenia, idTipoUsuario) VALUES (@nombre, @apellidos, @correo, @contrasena, @idTipoUsuario)";
                    command = new MySqlCommand(query, conn);
                    command.Parameters.AddWithValue("@nombre", nombre);
                    command.Parameters.AddWithValue("@apellidos", apellidos);
                    command.Parameters.AddWithValue("@correo", correo);
                    command.Parameters.AddWithValue("@contrasena", Encrypt(contrase�a));
                    command.Parameters.AddWithValue("@idTipoUsuario", 2);
                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Registro exitoso";
                return Redirect("~/Registro?exito=true");
            }
        }

        private string Encrypt(string input)
        {
            // Encriptar la contrase�a usando MD5
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        // Funci�n para validar si un correo electr�nico tiene un formato v�lido
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Utilizar la clase System.Net.Mail.MailAddress para validar el correo electr�nico
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
