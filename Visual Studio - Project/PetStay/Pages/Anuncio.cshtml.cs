using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PetStay.Pages
{
    public class AnuncioModel : PageModel
    {
        public List<Anuncio> Anuncios { get; set; }

        public void OnGet()
        {
            // cargar los datos de los anuncios desde alguna fuente de datos
            // y luego asignarlos a la propiedad Anuncios
        }
    }

    public class Anuncio
    {
        public string TipoAnuncio { get; set; }
        public string UrlImagen { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string PublicadoPor { get; set; }
        public string Contacto { get; set; }
        public string UrlDetalles { get; set; }
    }
}