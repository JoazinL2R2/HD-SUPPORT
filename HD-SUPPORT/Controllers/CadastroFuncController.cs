using HD_SUPPORT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
//using System.Data.Entity.Validation;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace HD_SUPPORT.Controllers
{
    [Authorize(Roles = "HelpDesk")]
    public class CadastroFuncController : Controller
    {
        private readonly BancoContexto _contexto;

        public CadastroFuncController(BancoContexto contexto)
        {
            _contexto = contexto;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            var Funcionarios = await _contexto.CadastroUser.ToListAsync();
            if (!string.IsNullOrEmpty(searchString))
            {
                 Funcionarios = _contexto.CadastroUser.Where(x => x.Nome.Contains(searchString)
                || x.Email.Contains(searchString)).ToList();
            }
            return View(Funcionarios);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult NovoCadastro()
        {
            return PartialView("_NovoCadastroFuncPartialView");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> NovoCadastro(CadastroUser cadastro)
        {
            if (_contexto.CadastroUser.Any(x => x.Email == cadastro.Email)) { 
                ModelState.AddModelError(nameof(cadastro.Email), "Email existente");
                return PartialView("_NovoCadastroFuncPartialView");
            }
            else
            {
                await _contexto.CadastroUser.AddAsync(cadastro);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "CadastroFunc", new { area = "" });
            }
        }
        public IActionResult Edit(int id)
        {
            CadastroUser cadastro = _contexto.CadastroUser.Where(x => x.Id == id).FirstOrDefault();
            return PartialView("_EditFuncPartialView", cadastro);
        }

        [HttpPost]
        public IActionResult Atualizar(CadastroUser cadastro)
        {
            try
            {
                if (cadastro == null)
                {
                    // Lógica para lidar com cadastro nulo, se necessário
                    return BadRequest("O objeto CadastroUser está nulo.");
                }

                if (_contexto.CadastroUser.Any(x => x.Email == cadastro.Email && x.Id != cadastro.Id))
                {
                    ModelState.AddModelError(nameof(cadastro.Email), "Email existente");
                    return View("Edit", cadastro);
                }
                else
                {
                    _contexto.CadastroUser.Update(cadastro);
                    _contexto.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                // Adicione logs detalhados ou mensagens de console para identificar a causa da exceção.
                Console.WriteLine($"Erro durante a atualização: {ex.Message}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditMaquina(int id)
        {
            CadastroUser cadastro = await _contexto.CadastroUser.FindAsync(id);
            return View(cadastro);
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarMaquina(CadastroUser cadastro)
        {
            _contexto.CadastroUser.Update(cadastro);
            await _contexto.SaveChangesAsync();
            return RedirectToAction("Index", "CadastroFunc", new { area = "" });
        }
        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            CadastroUser cadastro = await _contexto.CadastroUser.FindAsync(id);
            var emprestimo = _contexto.CadastroEmprestimos.FirstOrDefault(emp => emp.FuncionarioId == id);
            if (emprestimo != null)
            {
                var equipamento = await _contexto.CadastroEquipamentos.FindAsync(emprestimo.EquipamentoId);
                equipamento.Disponivel = true;
            }
            _contexto.CadastroUser.Remove(cadastro);
            await _contexto.SaveChangesAsync();
            return RedirectToAction("Index", "CadastroFunc", new { area = "" });
        }
    }

    
}