using HD_SUPPORT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
//using System.Data.Entity.Validation;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http;


namespace HD_SUPPORT.Controllers
{
    [Authorize(Roles = "HelpDesk, RH")]
    public class CadastroFuncController : Controller
    {
        private readonly BancoContexto _contexto;

        public CadastroFuncController(BancoContexto contexto)
        {
            _contexto = contexto;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            if (HttpContext.Session.GetString("nome") == null)
            {
                return RedirectToAction("LogOut", "Home", new { area = "" });
            }

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

        public bool verificaDigitos(string numero)
        {
            if (numero != null) {
                return numero.Contains('_');
            }
            return true;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> NovoCadastro(CadastroUser cadastro)
        {
            List<string> numeros = [cadastro.Cpf, cadastro.Telefone, cadastro.Telegram];
            var voltar = false;
            numeros.ForEach(e =>
            {
                if (verificaDigitos(e))
                {
                    ModelState.AddModelError(e, "Preencha com todos os valores");
                    voltar = true;
                }
            });
            if (voltar)
            {
                return RedirectToAction("Index", "CadastroFunc", new { area = "" });
            }
            else
            {
                if (ModelState.IsValid) { 
                    await _contexto.CadastroUser.AddAsync(cadastro);
                    await _contexto.SaveChangesAsync();
                    return RedirectToAction("Index", "CadastroFunc", new { area = "" });
                }
                else
                {
                    return RedirectToAction("Index", "CadastroFunc");
                }
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

                List<string> numeros = [cadastro.Cpf, cadastro.Telefone, cadastro.Telegram];
                var voltar = false;
                numeros.ForEach(e =>
                {
                    if (verificaDigitos(e))
                    {
                        ModelState.AddModelError(nameof(e), "Preencha com todos os valores");
                        voltar = true;
                    }
                });
                if (voltar)
                {
                    return RedirectToAction("Index", "CadastroFunc", new { area = "" });
                }

                if (_contexto.CadastroUser.Any(x => x.Email == cadastro.Email && x.Id != cadastro.Id))
                {
                    ModelState.AddModelError(nameof(cadastro.Email), "Email existente");
                    return View("Edit", cadastro);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        _contexto.CadastroUser.Update(cadastro);
                        _contexto.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index", "CadastroFunc");
                    }
                }
            }
            catch (Exception ex)
            {
                // Adicione logs detalhados ou mensagens de console para identificar a causa da exceção.
                Console.WriteLine($"Erro durante a atualização: {ex.Message}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Excluir(CadastroUser funcionario)
        {
            CadastroUser cadastro = await _contexto.CadastroUser.FindAsync(funcionario.Id);
            var emprestimo = _contexto.CadastroEmprestimos.FirstOrDefault(emp => emp.FuncionarioId == funcionario.Id);
            if (emprestimo != null)
            {
                var equipamento = await _contexto.CadastroEquipamentos.FindAsync(emprestimo.EquipamentoId);
                equipamento.Disponivel = true;
            }
            cadastro.profissional_HD = HttpContext.Session.GetString("nome");
            _contexto.CadastroUser.Update(cadastro);
            await _contexto.SaveChangesAsync();
            _contexto.CadastroUser.Remove(cadastro);
            await _contexto.SaveChangesAsync();
            return RedirectToAction("Index", "CadastroFunc", new { area = "" });
        }
    }

    
}