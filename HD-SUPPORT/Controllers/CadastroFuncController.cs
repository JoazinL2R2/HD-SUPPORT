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
            if (ModelState.IsValid)
            {
                List<string> numeros = new List<string> { cadastro.Telefone, cadastro.Telegram };

                if (numeros.Any(e => e == null))
                {
                    TempData["ErroAtualizacao"] = "Preencha todos os campos";
                    return RedirectToAction("Index", "CadastroFunc", new { area = "" });
                }

                bool voltar = false;
                numeros.ForEach(e =>
                {
                    if (verificaDigitos(e))
                    {
                        voltar = true;
                    }
                });
                if (_contexto.CadastroUser.Any(x => x.Email == cadastro.Email && x.Id != cadastro.Id))
                {
                    TempData["ErroAtualizacao"] = "Email já Cadastrado";
                    return RedirectToAction("Index");
                }
                if (_contexto.CadastroUser.Any(x => x.Telefone == cadastro.Telefone && x.Id != cadastro.Id))
                {
                    TempData["ErroAtualizacao"] = "Telefone já cadastrado";
                    return RedirectToAction("Index");
                }
                else
                {
                    if (dadosExistentes(cadastro) && !voltar)
                    {
                        await _contexto.CadastroUser.AddAsync(cadastro);
                        await _contexto.SaveChangesAsync();
                        return RedirectToAction("Index", "CadastroFunc", new { area = "" });
                    }
                    else
                    {
                        TempData["ErroAtualizacao"] = "Preencha todos os dados";
                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "CadastroFunc");
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

                List<string> numeros = [cadastro.Telefone, cadastro.Telegram];
                var voltar = false;
                numeros.ForEach(e =>
                {
                    if (verificaDigitos(e))
                    {
                        voltar = true;
                    }
                });

                if (_contexto.CadastroUser.Any(x => x.Email == cadastro.Email && x.Id != cadastro.Id))
                {
                    TempData["ErroAtualizacao"] = "Email já cadastrado";
                    return RedirectToAction("Index");
                }
                else
                {
                    if (dadosExistentes(cadastro) && !voltar)
                    {

                        _contexto.CadastroUser.Update(cadastro);
                        _contexto.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErroAtualizacao"] = "Preencha todos os dados";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro durante a atualização: {ex.Message}");
                TempData["ErroAtualizacao"] = "Erro interno do servidor";
                return RedirectToAction("Index");
            }
        }

        public bool dadosExistentes(CadastroUser cadastro)
        {
            if(cadastro.Telefone == null || cadastro.Telegram == null || cadastro.Email == null || cadastro.Status == null ||
                cadastro.Nome == null)
            {
                return false;
            }
            return true;
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