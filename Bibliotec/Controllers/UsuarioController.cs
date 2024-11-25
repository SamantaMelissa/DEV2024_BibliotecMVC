using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bibliotec.Contexts;
using Bibliotec.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bibliotec.Controllers
{
    [Route("[controller]")]
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(ILogger<UsuarioController> logger)
        {
            _logger = logger;
        }

        Context c = new Context();
        public IActionResult Index()
        {
            ViewBag.Admin = HttpContext.Session.GetString("Admin");
            int id = int.Parse(HttpContext.Session.GetString("UserID")!);

            Usuario usuario = c.Usuario.FirstOrDefault(x => x.UsuarioID == id)!;

            if (usuario == null)
            {
                return NotFound();
            }
            Curso curso = c.Curso.FirstOrDefault(x => x.CursoID == usuario.CursoID)!;

            if (curso != null)
            {
                ViewBag.Curso = curso.Nome;

            }else{
                ViewBag.Curso = "Não possui curso";
            }
            
            ViewBag.Nome = usuario.Nome;
            ViewBag.Email = usuario.Email;
            ViewBag.Contato = usuario.Contato;
            ViewBag.DtNascimento = usuario.DtNascimento.ToString("dd/MM/yyyy");
            
            return View();
        }
    }
}