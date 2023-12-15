
using HD_SUPPORT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HD_SUPPORT.Controllers
{
    public class EmprestimoController : Controller
    {
        private readonly BancoContexto _contexto;
        public EmprestimoController(BancoContexto contexto)
        {
            _contexto = contexto;
        }

        public bool RealizarEmprestimo(int equipamentoId, int usuarioId)
        {
            // Verifica se o equipamento está disponível
            CadastroEquip equipamento = ObterEquipamentoPorId(equipamentoId);
            CadastroUser user = ObterUserPorId(usuarioId);

            if (equipamento != null && equipamento.Disponivel)
            {
                // Marca o equipamento como indisponível
                equipamento.Disponivel = false;

                // Cria um novo registro de empréstimo
                CadastroEquip emprestimo = new CadastroEquip
                {
                    IdPatrimonio = equipamentoId,
                    DtEmeprestimoInicio = DateTime.Now,
                    DtEmeprestimoFinal = equipamento.DtEmeprestimoFinal
                };

                // Lógica para salvar o empréstimo no banco de dados
                 _contexto.CadastroEmprestimos.Add(emprestimo);
                 _contexto.SaveChanges();
                // ...

                return true; // Empréstimo realizado com sucesso
            }

            return false; // Equipamento indisponível
        }

        public bool DevolverEquipamento(int emprestimoId)
        {
            // Lógica para verificar se o empréstimo é válido
            CadastroEquip emprestimo = ObterEmprestimoPorId(emprestimoId);

            if (emprestimo != null)
            {
                // Marca o equipamento como disponível
                CadastroEquip equipamento = ObterEquipamentoPorId(emprestimo.IdPatrimonio);
                if (equipamento != null)
                {
                    equipamento.Disponivel = true;

                    // Lógica para atualizar o equipamento no banco de dados
                    _contexto.CadastroEmprestimos.Update(equipamento);
                    
                    // ...
                }

                // Lógica para marcar o empréstimo como devolvido no banco de dados
                equipamento.DtEmeprestimoFinal = DateTime.Now;
                // ...
                _contexto.CadastroEmprestimos.Update(emprestimo);

                return true; // Devolução realizada com sucesso
            }

            return false; // Empréstimo não encontrado
        }

        // Métodos fictícios para obter equipamento e empréstimo por ID
        private CadastroEquip ObterEquipamentoPorId(int equipamentoId)
        {
            // Lógica para obter equipamento por ID do banco de dados
            CadastroEquip equipamento = _contexto.CadastroEmprestimos.Find(equipamentoId);
            // ...

            return null; // Retornar o equipamento encontrado ou null se não existir
        }
        private CadastroUser ObterUserPorId(int userId)
        {
            // Lógica para obter user por ID do banco de dados
            CadastroUser user = _contexto.CadastroUser.Find(userId);
            // ...

            return null; // Retornar o usuario encontrado ou null se não existir
        }

        private CadastroEquip ObterEmprestimoPorId(int emprestimoId)
        {
            // Lógica para obter empréstimo por ID do banco de dados
            CadastroEquip equip = _contexto.CadastroEmprestimos.Find(emprestimoId);
            // ...

            return null; // Retornar o empréstimo encontrado ou null se não existir
        }

    }
}
