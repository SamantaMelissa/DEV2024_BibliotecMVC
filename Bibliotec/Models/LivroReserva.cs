using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bibliotec.Models
{
    public class LivroReserva
    {
        [Key]
        public int LivroReservaID { get; set; }
        // cOLOCA O ? PARA QUE ACEITE DADOS NULOS -> NULL
        public DateOnly? DtReserva { get; set; }
        public DateOnly? DtDevolucao { get; set; }
        public int Status { get; set; }

        [ForeignKey("Usuario")]//DATA ANNOTATION
        public int UsuarioID { get; set; }
        public Usuario? Usuario { get; set; }

        [ForeignKey("Livro")]//DATA ANNOTATION
        public int LivroID { get; set; }
        public Livro? Livro { get; set; }
    }
}