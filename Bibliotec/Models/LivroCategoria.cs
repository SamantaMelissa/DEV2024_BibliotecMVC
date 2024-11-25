using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bibliotec.Models
{
    public class LivroCategoria
    {
        [Key]
        public int LivroCategoriaID { get; set; }

        [ForeignKey("Livro")]//DATA ANNOTATION
        public int LivroID { get; set; }
        public Livro? Livro { get; set; }


        [ForeignKey("Categoria")]//DATA ANNOTATION
        public int CategoriaID { get; set; }
        public Categoria? Categoria { get; set; }
    }
}