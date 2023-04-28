using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PetStay.Pages.Shared
{
    public class Datos
    {
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string TipoDeServicio { get; set; }
    }

    public class Registro_de_serviciosModel : PageModel
    {
        public List<Datos> DatosLista { get; set; }

        public void OnGet()
        {
            
        }
    }
}
