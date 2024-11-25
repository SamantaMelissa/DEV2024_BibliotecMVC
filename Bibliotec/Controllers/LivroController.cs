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
    public class LivroController : Controller
    {
        private readonly ILogger<LivroController> _logger;

        public LivroController(ILogger<LivroController> logger)
        {
            _logger = logger;
        }

        Context c = new Context();

        public IActionResult Index()
        {
            ViewBag.Admin = HttpContext.Session.GetString("Admin");

            // Busca todos os livros
            List<Livro> livros = c.Livro.ToList();

            // Cria um dicionário para armazenar as datas de reserva por livroId
            var livrosComReserva = c.LivroReserva.ToDictionary(lr => lr.LivroID, lr => lr.DtReserva);

            // Adiciona as informações à ViewBag
            ViewBag.Livros = livros;
            ViewBag.LivrosComReserva = livrosComReserva;

            return View();
        }

        //Criar método para mostrar os detalhes dos livros:
        [Route("Detalhes/{id}")]
        public IActionResult Detalhes(int id)
        {
            ViewBag.Admin = HttpContext.Session.GetString("Admin");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            Livro livroUnico = c.Livro.First(x => x.LivroID == id);

            //Procurar o id na CategoriaLivro:

            var categoriasDoLivro = c.LivroCategoria
                .Where(lc => lc.LivroID == id)
                .Select(lc => c.Categoria.First(c => c.CategoriaID == lc.CategoriaID))
                .ToList();

            ViewBag.Livro = livroUnico;
            ViewBag.Categoria = categoriasDoLivro;

            return View("Detalhes");
        }

        //Mostra a tela de editar
        [Route("Editar/{id}")]
        public IActionResult Editar(int id)
        {
            ViewBag.Admin = HttpContext.Session.GetString("Admin");
            // Buscar o livro específico pelo ID
            Livro livro = c.Livro.FirstOrDefault(j => j.LivroID == id)!;

            // Buscar as categorias relacionadas ao livro
            var categoriasDoLivro = c.LivroCategoria
              .Where(lc => lc.LivroID == id)
              .Select(lc => lc.Categoria)
              .ToList();

            // Passar as informações para a view
            ViewBag.Livro = livro;
            ViewBag.categoriasDoLivro = categoriasDoLivro;
            ViewBag.Categoria = c.Categoria.ToList();

            Console.WriteLine($"{livro.Imagem}");

            return View("Editar");
        }

        [Route("Atualizar/{id}")]
        public IActionResult Atualizar(int id, IFormCollection form, IFormFile? imagem)
        {
            // Buscar o livro específico pelo ID
            Livro livro = c.Livro.FirstOrDefault(j => j.LivroID == id);

            if (livro == null)
            {
                return NotFound(); // Retorna erro 404 se o livro não for encontrado
            }

            // Atualizar os dados do livro com as informações do formulário
            livro.Nome = form["Nome"];
            livro.Descricao = form["Descricao"];
            livro.Editora = form["Editora"];
            livro.Escritor = form["Escritor"];
            livro.Idioma = form["Idioma"];

            // Atualizar a imagem, se uma nova for fornecida
            if (imagem != null && imagem.Length > 0)
            {
                // Caminho onde a imagem será salva
                var caminhoImagem = Path.Combine("wwwroot/images/Livros", imagem.FileName);

                // Excluir a imagem antiga, se existir
                if (!string.IsNullOrEmpty(livro.Imagem))
                {
                    var caminhoAntigo = Path.Combine("wwwroot/images/Livros", livro.Imagem);
                    if (System.IO.File.Exists(caminhoAntigo))
                    {
                        System.IO.File.Delete(caminhoAntigo);
                    }
                }

                // Salvar a nova imagem no caminho especificado
                using (var stream = new FileStream(caminhoImagem, FileMode.Create))
                {
                    imagem.CopyTo(stream);
                }

                // Atualizar o caminho da imagem no banco de dados
                livro.Imagem = imagem.FileName;
            }

            // Atualizar categorias
            var categoriasSelecionadas = form["CategoriaID"].ToList();
            var categoriasDoLivro = c.LivroCategoria.Where(lc => lc.LivroID == id).ToList();

            // Remover categorias que não estão mais selecionadas
            foreach (var categoria in categoriasDoLivro)
            {
                if (!categoriasSelecionadas.Contains(categoria.CategoriaID.ToString()))
                {
                    c.LivroCategoria.Remove(categoria);
                }
            }

            // Adicionar novas categorias selecionadas
            foreach (var categoriaId in categoriasSelecionadas)
            {
                if (!categoriasDoLivro.Any(c => c.CategoriaID.ToString() == categoriaId))
                {
                    c.LivroCategoria.Add(new LivroCategoria
                    {
                        LivroID = id,
                        CategoriaID = int.Parse(categoriaId)
                    });
                }
            }

            // Salvar alterações no banco de dados
            c.SaveChanges();

            // Redirecionar para a página de detalhes ou outra página desejada
            return RedirectToAction("Detalhes", new { id });
        }

        [Route("Cadastro")]
        public IActionResult Cadastro()
        {
            ViewBag.Admin = HttpContext.Session.GetString("Admin");

            ViewBag.Categoria = c.Categoria.ToList();

            return View();
        }

        [Route("Cadastrar")]
        public IActionResult Cadastrar(IFormCollection form)
        {
            Livro novoLivro = new Livro();
            // List para armazenar múltiplas categorias
            List<LivroCategoria> livroCategorias = new List<LivroCategoria>();

            novoLivro.Nome = form["Nome"].ToString();
            novoLivro.Descricao = form["Descricao"].ToString();
            novoLivro.Editora = form["Editora"].ToString();
            novoLivro.Escritor = form["Escritor"].ToString();
            novoLivro.Idioma = form["Idioma"].ToString();
            // novoLivro.Imagem = form["Imagem"].ToString();

            // Console.WriteLine(form.Files.Count);


            // upload inicio
            // array = 0 sem arquivo array > 0 com arquivo
            if (form.Files.Count > 0)
            {
                // Armazena o array dentro da variavel:
                var file = form.Files[0];

                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Livros");

                // Verifica se a pasta existe, se n existe, ele cria:

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/", folder, file.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                novoLivro.Imagem = file.FileName;

            }
            else
            {
                novoLivro.Imagem = "padrao.png";
            }

            // upload final

            c.Livro.Add(novoLivro);

            c.SaveChanges();

            string[] categoriaIds = form["CategoriaID"].ToString().Split(',');

            foreach (string categoriaId in categoriaIds)
            {
                Console.WriteLine($"Banana");
                Console.WriteLine($"{categoriaId}");
                LivroCategoria livroCategoria = new LivroCategoria();
                livroCategoria.CategoriaID = int.Parse(categoriaId);
                livroCategoria.LivroID = novoLivro.LivroID;
                livroCategorias.Add(livroCategoria);
            }
            c.LivroCategoria.AddRange(livroCategorias);
            c.SaveChanges();

            return LocalRedirect("/Livro");
        }
    
        [Route("Excluir/{id}")]
        public IActionResult Excluir(int id)
        {
             Livro livro = c.Livro.First(j => j.LivroID == id);

            // Obter as entradas de LivroCategoria relacionadas ao livro
            var categoriasDoLivro = c.LivroCategoria.Where(lc => lc.LivroID == id).ToList();

            // Remover as entradas de LivroCategoria
            foreach (var livroCategoria in categoriasDoLivro)
            {
                c.LivroCategoria.Remove(livroCategoria);
            }

            // Remover o livro
            c.Livro.Remove(livro);

            // Salvar as alterações no banco de dados
            c.SaveChanges();
            
            return LocalRedirect("/Livro");
        }
    
    }
}