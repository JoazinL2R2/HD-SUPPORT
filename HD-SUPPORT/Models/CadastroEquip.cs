namespace HD_SUPPORT.Models
{
    public class CadastroEquip
    {
        public int Id { get; set; }
        public int IdPatrimonio { get; set; }
        public string Modelo { get; set; }
        public string Processador { get; set; }
        public string SistemaOperacionar { get; set; }
        public string HeadSet { get; set; }
        public DateTime DtEmeprestimoInicio { get; set; }
        public DateTime DtEmeprestimoFinal { get; set; }
        public bool Disponivel { get; set; } = true;
    }
}
