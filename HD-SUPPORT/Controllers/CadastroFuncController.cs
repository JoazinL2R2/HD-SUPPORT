using HD_SUPPORT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using System.Data.Entity.Validation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HD_SUPPORT.Controllers
{
    public class CadastroFuncController : Controller
    {
        private readonly BancoContexto _contexto;

        public CadastroFuncController(BancoContexto contexto)
        {
            _contexto = contexto;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _contexto.CadastroUser.ToListAsync());
        }
        [HttpGet]
        public IActionResult NovoCadastro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NovoCadastro(CadastroUser cadastro)
        {
            if (_contexto.CadastroUser.Any(x => x.Email == cadastro.Email)) { 
                ModelState.AddModelError(nameof(cadastro.Email), "Email existente");
                return View();
            }
            else
            {
                await _contexto.CadastroUser.AddAsync(cadastro);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "CadastroFunc", new { area = "" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            CadastroUser cadastro = await _contexto.CadastroUser.FindAsync(id);
            return View(cadastro);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(CadastroUser cadastro)
        {
            if (_contexto.CadastroUser.Any(x => x.Email == cadastro.Email && x.Id != cadastro.Id))
            {
                ModelState.AddModelError(nameof(cadastro.Email), "Email existente");
                return View("Edit", cadastro);
            }
            else
            {
                _contexto.CadastroUser.Update(cadastro);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "CadastroFunc", new { area = "" });
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
            _contexto.CadastroUser.Remove(cadastro);
            await _contexto.SaveChangesAsync();
            return RedirectToAction("Index", "CadastroFunc", new { area = "" });
        }
    }
}