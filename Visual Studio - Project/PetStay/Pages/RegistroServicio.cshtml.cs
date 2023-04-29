using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PetStay.Pages
{
    public class RegistroServicioModel : PageModel
    {
        public void OnGet()
        {
        }
    }

    public class Servicio
    {
        public string Nombre { get; set; }
        public int Cedula { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public int TipoServicio { get; set; }
        public string TelefonoPrincipal { get; set; }
        public string TelefonoSecundario { get; set; }
        public byte[] Imagen { get; set; }

    }
}