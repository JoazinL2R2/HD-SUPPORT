using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace HD_SUPPORT.Models
{
    public class CadastroUser
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Telegram { get; set; }
        public string? Telefone { get; set; }
        public string?   Status { get; set; }
        public string? Categoria { get; set; }
        public string? profissional_HD { get; set; }
    }
}
