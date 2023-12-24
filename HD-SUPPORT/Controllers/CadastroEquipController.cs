using HD_SUPPORT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HD_SUPPORT.Controllers
{
    public class CadastroEquipController : Controller
    {
        private readonly BancoContexto _contexto;

        public CadastroEquipController(BancoContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<IActionResult> Index()
        {
            var equipamentos = await _contexto.CadastroEquipamentos.ToListAsync();
            return View(equipamentos);
        }

        [HttpGet]
        public IActionResult NovoCadastro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NovoCadastro(CadastroEquip equipamento)
        {
            if (_contexto.CadastroEquipamentos.Any(x => x.IdPatrimonio == equipamento.IdPatrimonio))
            {
                ModelState.AddModelError(nameof(equipamento.IdPatrimonio), "Máquina já cadastrada");
                return View();
            }
            else
            {
                _contexto.CadastroEquipamentos.Add(equipamento);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "CadastroEquip", new { area = "" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cadastro = await _contexto.CadastroEquipamentos.FindAsync(id);
            return View(cadastro);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(CadastroEquip cadastro)
        {
            if (_contexto.CadastroEquipamentos.Any(x => x.IdPatrimonio == cadastro.IdPatrimonio && x.Id != cadastro.Id))
            {
                ModelState.AddModelError(nameof(cadastro.IdPatrimonio), "Máquina já cadastrada");
                return View("Edit", cadastro);
            }
            else
            {
                _contexto.CadastroEquipamentos.Update(cadastro);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "CadastroEquip", new { area = "" });
            }
        }
    }
}
