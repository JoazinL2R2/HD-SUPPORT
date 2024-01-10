
using HD_SUPPORT.Migrations;
using HD_SUPPORT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace HD_SUPPORT.Controllers
{
    [Authorize(Roles = "HelpDesk")]
    public class EmprestimoController : Controller
    {
        private readonly BancoContexto _contexto;
        public EmprestimoController(BancoContexto contexto)
        {
            _contexto = contexto;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            if (HttpContext.Session.GetString("nome") == null)
            {
                return RedirectToAction("LogOut", "Home", new { area = "" });
            }

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
            return PartialView("_NovoEmprestimoPartialView");
        }

        [HttpPost]

        public async Task<IActionResult> NovoEmprestimo(EmprestimoViewModel cadastro)
        {
            var idPatrimonio = cadastro.Equipamento.IdPatrimonio;
            var email = cadastro.Funcionario.Email;

            var funcionario = await _contexto.CadastroUser.FirstOrDefaultAsync(u => u.Email == email);

            if (funcionario == null)
            {
                ModelState.AddModelError("Funcionario.Email", "Funcionário não encontrado.");
                return PartialView("_NovoEmprestimoPartialView", cadastro);
            }

            var func = await _contexto.CadastroEmprestimos.FirstOrDefaultAsync(u => u.Funcionario.Id == funcionario.Id);

            if (func != null)
            {
                ModelState.AddModelError("Funcionario.Email", "Funcionário Existente.");
                return PartialView("_NovoEmprestimoPartialView", cadastro);
            }

            var equipamentoDisponivel = await _contexto.CadastroEquipamentos
                .FirstOrDefaultAsync(x => x.IdPatrimonio == idPatrimonio && x.Disponivel);

            if (equipamentoDisponivel != null)
            {
                equipamentoDisponivel.Disponivel = false;

                var novoEmprestimo = new EmprestimoViewModel
                {
                    Funcionario = funcionario,
                    Equipamento = equipamentoDisponivel,
                    profissional_HD = cadastro.profissional_HD
                };

                await _contexto.CadastroEmprestimos.AddAsync(novoEmprestimo);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "Emprestimo", new { area = "" });
            }
            else
            {
                ModelState.AddModelError("Equipamento.IdPatrimonio", "Equipamento não está disponível ou não existe");
                return PartialView("_NovoEmprestimoPartialView", cadastro);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            EmprestimoViewModel cadastro = await _contexto.CadastroEmprestimos.FindAsync(id);
            cadastro.Equipamento = await _contexto.CadastroEquipamentos.FindAsync(cadastro.EquipamentoId);
            cadastro.Funcionario = await _contexto.CadastroUser.FindAsync(cadastro.FuncionarioId);
            return PartialView("_EditarEmprestimoPartialView", cadastro);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(EmprestimoViewModel cadastro)
        {
            var idPatrimonio = cadastro.Equipamento.IdPatrimonio;
            var email = cadastro.Funcionario.Email;

            var funcionario = await _contexto.CadastroUser.FirstOrDefaultAsync(u => u.Email == email);



            if (funcionario == null)
            {
                ModelState.AddModelError("Funcionario.Email", "Funcionário não encontrado.");
                return PartialView("_EditarEmprestimoPartialView", cadastro);
            }

            if (_contexto.CadastroEquipamentos.FirstOrDefault(x => x.IdPatrimonio == idPatrimonio) == null)
            {
                ModelState.AddModelError("Equipamento.IdPatrimonio", "Equipamento não encontrado.");
                return PartialView("_EditarEmprestimoPartialView", cadastro);
            }

            if (_contexto.CadastroEmprestimos.Any(x => x.Equipamento.Id == cadastro.Equipamento.Id && x.Id != cadastro.Equipamento.Id))
            {
                ModelState.AddModelError("Equipamento.IdPatrimonio", "Equipamento já emprestado");
                return PartialView("_EditarEmprestimoPartialView", cadastro);
            }
            if (_contexto.CadastroEmprestimos.Any(x => x.Funcionario.Email == email && x.Id != cadastro.Id))
            {
                ModelState.AddModelError("Funcionario.Email", "Funcionario já tem um empréstimo");
                return PartialView("_EditarEmprestimoPartialView", cadastro);
            }

            var equipamentoDisponivel = await _contexto.CadastroEquipamentos
                .FirstOrDefaultAsync(x => x.IdPatrimonio == idPatrimonio && x.Disponivel);

            if (equipamentoDisponivel != null || !_contexto.CadastroEmprestimos.Any(x => x.Equipamento.Id == cadastro.EquipamentoId && x.Id != cadastro.Id))
            {
                if (equipamentoDisponivel != null)
                {
                    var equipamento = _contexto.CadastroEquipamentos.FirstOrDefault(x => x.Id == cadastro.EquipamentoId);
                    equipamento.Disponivel = true;
                    _contexto.CadastroEquipamentos.Update(equipamento);

                    equipamentoDisponivel.Disponivel = false;
                }
                cadastro.Equipamento = equipamentoDisponivel;
                cadastro.Funcionario = funcionario;
                cadastro.profissional_HD = HttpContext.Session.GetString("profissional") + " - " + HttpContext.Session.GetString("nome");

                _contexto.CadastroEmprestimos.Update(cadastro);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "Emprestimo", new { area = "" });
            }
            else
            {
                ModelState.AddModelError("Equipamento.IdPatrimonio", "Equipamento não está disponível ou não existe");
                return PartialView("_EditarEmprestimoPartialView", cadastro);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(EmprestimoViewModel emprestimo)
        {
            var cadastro = await _contexto.CadastroEmprestimos.FindAsync(emprestimo.Id);
            if (cadastro != null)
            {
                var equipamento = await _contexto.CadastroEquipamentos.FindAsync(cadastro.EquipamentoId);
                equipamento.Disponivel = true;
                _contexto.CadastroEquipamentos.Update(equipamento);
                cadastro.profissional_HD = HttpContext.Session.GetString("profissional") + " - " + HttpContext.Session.GetString("nome");
                _contexto.CadastroEmprestimos.Update(cadastro);
                await _contexto.SaveChangesAsync();
                _contexto.CadastroEmprestimos.Remove(cadastro);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "Emprestimo", new { area = "" });
            }
            return RedirectToAction("Index", "CadastroFunc", new { area = "" });
        }

    }
}
