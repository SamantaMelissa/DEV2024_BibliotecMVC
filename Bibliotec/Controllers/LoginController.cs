using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bibliotec.Contexts;
using Bibliotec.Models;

namespace Bibliotec.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        Context c = new Context();

        public IActionResult Index()
        {
            return View();
        }

        [Route("Logar")]
        public IActionResult Logar(IFormCollection form)
        {

            string emailInformado = form["Email"].ToString();
            string senhaInformado = form["Senha"].ToString();

            Usuario usuarioBuscado = c.Usuario.FirstOrDefault(usuario => usuario.Email == emailInformado && usuario.Senha == senhaInformado)!;
            // Usuario usuarioBuscado = c.Usuario.First(usuario => usuario.Email == emailInformado && usuario.Senha == senhaInformado)!;

            //    List<Usuario> listaUsuario = c.Usuario.ToList();

            //    foreach (Usuario user in listaUsuario)
            //    {
            //         if(user.Email == emailInformado && user.Senha == senhaInformado){
            //             Console.WriteLine($"acheiiii");
            //             break;
            //         }
            //    }

            if (usuarioBuscado == null)
            {
                Console.WriteLine($"Dados inválidos!");
                return LocalRedirect("~/Login");
            }
            else
            {
                HttpContext.Session.SetString("UserName", usuarioBuscado.Nome!);
                HttpContext.Session.SetString("Admin", usuarioBuscado.Admin.ToString());
                HttpContext.Session.SetString("UserID", usuarioBuscado.UsuarioID.ToString());

                return LocalRedirect("~/Livro");
            }
        }

        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("UserID");
            HttpContext.Session.Remove("Admin");

            return LocalRedirect("~/");
        }



    }
}