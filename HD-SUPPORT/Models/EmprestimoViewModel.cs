namespace HD_SUPPORT.Models
{
    public class EmprestimoViewModel
    {


        public int Id { get; set; }
        public CadastroUser Funcionario { get; set; }
        public int FuncionarioId { get; set; }
        public CadastroEquip Equipamento { get; set; }
        public int EquipamentoId { get; set; }
    }
}
