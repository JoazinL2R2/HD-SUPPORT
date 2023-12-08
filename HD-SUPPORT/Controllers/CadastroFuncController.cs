using HD_SUPPORT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            await _contexto.CadastroUser.AddAsync(cadastro);
            await _contexto.SaveChangesAsync();
            return RedirectToAction("Index", "CadastroFunc", new { area = "" });
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