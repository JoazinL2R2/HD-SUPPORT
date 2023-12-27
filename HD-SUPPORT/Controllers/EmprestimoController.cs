
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
        public async Task<IActionResult> Index(string searchString)
        {
            var emprestimo = await _contexto.CadastroEmprestimos
                .Include(e => e.Equipamento)
                .Include(e => e.Funcionario)
                .ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                emprestimo = emprestimo
                    .Where(x => x.Funcionario.Nome.Contains(searchString)
                                || x.Funcionario.Email.Contains(searchString)
                                || x.Equipamento.IdPatrimonio.ToString().Contains(searchString))
                    .ToList();
            }

            return View(emprestimo);
        }
        [HttpGet]
        public IActionResult NovoEmprestimo()
        {
            return View();
        }

        [HttpPost]
        
        public async Task<IActionResult> NovoEmprestimo(EmprestimoViewModel cadastro)
        {
            var idPatrimonio = cadastro.Equipamento.IdPatrimonio;
            var email = cadastro.Funcionario.Email;

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
