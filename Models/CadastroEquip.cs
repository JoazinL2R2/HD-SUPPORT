using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HD_SUPPORT.Models
{
    public class CadastroEquip
    {
        public int IdPatrimonio { get; set; }
        public string Modelo { get; set; }
        public string Processador { get; set; }
        public string SistemaOperacionar { get; set; }
        public string HeadSet { get; set; }
        public int DtEmeprestimoInicio { get; set; }
        public int DtEmeprestimoFinal { get; set; }
    }
}
