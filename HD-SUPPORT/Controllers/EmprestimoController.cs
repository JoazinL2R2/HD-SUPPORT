
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
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult NovoEmprestimo()
        {
            return View();
        }

        [HttpPost]
        
        public async Task<IActionResult> NovoEmprestimo([Bind("email", "idPatrimonio")] string email, int idPatrimonio)
        {
            var funcionario = await _contexto.CadastroUser.FirstOrDefaultAsync(u => u.Email == email);

            if (funcionario == null)
            {
                ModelState.AddModelError(nameof(email), "Funcionário não encontrado.");
                return View();
            }

            var equipamentoDisponivel = await _contexto.CadastroEquipamentos
                .FirstOrDefaultAsync(x => x.IdPatrimonio == idPatrimonio && x.Disponivel);

            if (equipamentoDisponivel != null)
            {
                equipamentoDisponivel.Disponivel = false;


                var novoEmprestimo = new EmprestimoViewModel
                {
                    Funcionario = funcionario,
                    Equipamento = equipamentoDisponivel
                };

                await _contexto.CadastroEmprestimos.AddAsync(novoEmprestimo);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "Emprestimo", new { area = "" });
            }
            else
            {
                ModelState.AddModelError(nameof(idPatrimonio), "Equipamento não está disponível ou não existe");
                return View();
            }
        }



    }
}
