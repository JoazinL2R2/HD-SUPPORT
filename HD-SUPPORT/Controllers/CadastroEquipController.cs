using HD_SUPPORT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HD_SUPPORT.Controllers
{
    [Authorize]
    public class CadastroEquipController : Controller
    {
        private readonly BancoContexto _contexto;

        public CadastroEquipController(BancoContexto contexto)
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
        public async Task<IActionResult> NovoCadastro(CadastroEquip cadastro)
        {
            if (_contexto.CadastroEmprestimos.Any(x => x.IdPatrimonio == cadastro.IdPatrimonio))
            {
                ModelState.AddModelError(nameof(cadastro.IdPatrimonio), "Maquina já cadastrada");
                return View();
            }
            else
            {
                await _contexto.CadastroEmprestimos.AddAsync(cadastro);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "CadastroFunc", new { area = "" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            CadastroEquip cadastro = await _contexto.CadastroEmprestimos.FindAsync(id);
            return View(cadastro);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(CadastroEquip cadastro)
        {
            if (_contexto.CadastroEmprestimos.Any(x => x.IdPatrimonio == cadastro.IdPatrimonio))
            {
                ModelState.AddModelError(nameof(cadastro.IdPatrimonio), "Email existente");
                return View("Edit", cadastro);
            }
            else
            {
                _contexto.CadastroEmprestimos.Update(cadastro);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "CadastroFunc", new { area = "" });
            }
        }
    }
}
