using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
//using IndexAttribute = System.ComponentModel.DataAnnotations.Schema.IndexAttribute;
namespace HD_SUPPORT.Models
{
    public class CadastroHelpDesk
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public byte[]? Foto { get; set; }
        public string CodigoVerificacao { get; internal set; }
    }
}