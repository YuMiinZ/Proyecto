using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PetStay.Pages
{
    public class RegistroModel : PageModel
    {
        public void OnGet()
        {
        }
    }

    public class RegistrarUsuario
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
    }
}