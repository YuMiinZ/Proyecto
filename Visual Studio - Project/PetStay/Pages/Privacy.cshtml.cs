using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;

namespace PetStay.Pages
{
    public class PrivacyModel : PageModel
    {
        public string TipoAnuncio { get; set; }
        public string UrlImagen { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string PublicadoPor { get; set; }
        public string Contacto { get; set; }
        public string UrlDetalles { get; set; }

        public byte[] Image { get; set; }

        // Agregamos la propiedad Model
        public PrivacyModel Model { get; set; }

        public void OnGet()
        {
            // Asignamos la instancia actual a la propiedad Model
            Model = this;
            using var connection = new MySqlConnection("Server=localhost;Port=3306;Database=PetStay;Uid=root;Pwd=1234;");
            connection.Open();

            var command = new MySqlCommand("SELECT imgDocIdentificacion FROM Solicitud WHERE idSolicitud = 1", connection);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                Image = (byte[])reader["imgDocIdentificacion"];
            }
        }
    }
}
