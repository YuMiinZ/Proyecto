using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;


namespace PetStay.Pages
{
    public class UserModel : PageModel
    {
        public string nombreUsuario { get; set; }
        public List<Publicacion> Publicaciones { get; set; }

        public async Task OnGetAsync(string NombreUsuario)
        {
            nombreUsuario = NombreUsuario;

            using var connection = new MySqlConnection("Server=localhost;Port=3306;Database=PetStay;Uid=root;Pwd=1234;");
            await connection.OpenAsync();

            var query = "SELECT titulo, imgAnuncio, descripcion, Usuario.nombre, medioContacto " +
            "FROM Anuncio " +
            "INNER JOIN Usuario ON Usuario.idUsuario = Anuncio.idUsuario " +
            "WHERE idEstado = 1";


            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            Publicaciones = new List<Publicacion>();

            while (await reader.ReadAsync())
            {
                var publicacion = new Publicacion
                {
                    Titulo = reader.GetString(0),
                    Imagen = (byte[])reader.GetValue(1),
                    Descripcion = reader.GetString(2),
                    Autor = reader.GetString(3),
                    Contacto = reader.GetString(4)
                };

                Publicaciones.Add(publicacion);
            }
        }

        public class Publicacion
        {
            public string Titulo { get; set; }
            public byte[] Imagen { get; set; }
            public string Descripcion { get; set; }
            public string Autor { get; set; }
            public string Contacto { get; set; }
        }
    }

}