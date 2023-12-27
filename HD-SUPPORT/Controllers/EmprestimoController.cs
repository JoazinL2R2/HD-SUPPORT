
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
                ModelState.AddModelError("Funcionario.Email", "Funcionário não encontrado.");
                return View();
            }

            var func = await _contexto.CadastroEmprestimos.FirstOrDefaultAsync(u => u.Funcionario.Id == funcionario.Id);

            if (func != null)
            {
                ModelState.AddModelError("Funcionario.Email", "Funcionário Existente.");
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
                ModelState.AddModelError("Equipamento.IdPatrimonio", "Equipamento não está disponível ou não existe");
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            EmprestimoViewModel cadastro = await _contexto.CadastroEmprestimos.FindAsync(id);
            cadastro.Equipamento = await _contexto.CadastroEquipamentos.FindAsync(cadastro.EquipamentoId);
            cadastro.Funcionario = await _contexto.CadastroUser.FindAsync(cadastro.FuncionarioId);
            return View(cadastro);
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
                return View("Edit", cadastro);
            }

            if (_contexto.CadastroEmprestimos.Any(x => x.Equipamento.Id == cadastro.Equipamento.Id && x.Id != cadastro.Equipamento.Id))
            {
                ModelState.AddModelError("Equipamento.IdPatrimonio", "Equipamento já emprestado");
                return View("Edit", cadastro);
            }
            if (_contexto.CadastroEmprestimos.Any(x => x.Funcionario.Email == email && x.Id != cadastro.Id))
            {
                ModelState.AddModelError("Funcionario.Email", "Funcionario já cadastrado");
                return View("Edit", cadastro);
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

                _contexto.CadastroEmprestimos.Update(cadastro);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "Emprestimo", new { area = "" });
            }
            else
            {
                ModelState.AddModelError("Equipamento.IdPatrimonio", "Equipamento não está disponível ou não existe");
                return View("Edit", cadastro);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            var cadastro = await _contexto.CadastroEmprestimos.FindAsync(id);
            if (cadastro != null) { 
                var equipamento = await _contexto.CadastroEquipamentos.FindAsync(cadastro.Equipamento.Id);
                equipamento.Disponivel = true;
                _contexto.CadastroEquipamentos.Update(equipamento);
                _contexto.CadastroEmprestimos.Remove(cadastro);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "Emprestimo", new { area = "" });
            }
            return RedirectToAction("Index", "CadastroFunc", new { area = "" });
        }

    }
}
