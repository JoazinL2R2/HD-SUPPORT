using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HD_SUPPORT.Models
{
    public class BancoContexto : DbContext
    {
        public DbSet<CadastroHelpDesk> CadastroHD { get; set; }
        public DbSet<CadastroUser> CadastroUser { get; set; }
        public BancoContexto(DbContextOptions<BancoContexto> opcoes) : base(opcoes)
        {

        }
    }
}