using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System.IO;

namespace PetStay.Pages
{
    public class RegistroServicioModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        [BindProperty]
        public IFormFile Imagen { get; set; }
        [BindProperty]
        public string Nombre { get; set; }
        [BindProperty]
        public string Cedula { get; set; }
        [BindProperty]
        public string Provincia { get; set; }
        [BindProperty]
        public string Canton { get; set; }
        [BindProperty]
        public string Distrito { get; set; }
        [BindProperty]
        public string TelefonoPrincipal { get; set; }
        [BindProperty]
        public string TelefonoSecundario { get; set; }
        [BindProperty]
        public int IdTipoServicio { get; set; }
        [BindProperty]
        public int IdEstado { get; set; }

        public RegistroServicioModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

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