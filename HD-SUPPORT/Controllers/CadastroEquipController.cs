using HD_SUPPORT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace HD_SUPPORT.Controllers
{
    [Authorize(Roles = "HelpDesk, RH")]
    public class CadastroEquipController : Controller
    {
        private readonly BancoContexto _contexto;

        public CadastroEquipController(BancoContexto contexto)
        {
            _contexto = contexto;

        }
        public async Task<IActionResult> Index(string searchString)
        {
            if (HttpContext.Session.GetString("nome") == null)
            {
                return RedirectToAction("LogOut", "Home", new { area = "" });
            }

            var equipamentos = await _contexto.CadastroEquipamentos.ToListAsync();
            var equipamentosFiltrados = new List<CadastroEquip>();

            if (!string.IsNullOrEmpty(searchString))
            {
                equipamentosFiltrados = equipamentos
                    .Where(e => e.IdPatrimonio.ToString().Contains(searchString)
                                || e.Modelo.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                || e.Processador.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                || e.SistemaOperacionar.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                equipamentosFiltrados = equipamentos;
            }

            return View(equipamentosFiltrados);
        }

        [HttpGet]
        public IActionResult NovoCadastro()
        {
            return PartialView("_NovoCadastroEquipPartialView");
        }

        [HttpPost]
        public async Task<IActionResult> NovoCadastro(CadastroEquip equipamento)
        {
            if (_contexto.CadastroEquipamentos.Any(x => x.IdPatrimonio == equipamento.IdPatrimonio))
            {
                ModelState.AddModelError(nameof(equipamento.IdPatrimonio), "Máquina já cadastrada");
                return PartialView("_NovoCadastroEquipPartialView");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    _contexto.CadastroEquipamentos.Add(equipamento);
                    await _contexto.SaveChangesAsync();
                    return RedirectToAction("Index", "CadastroEquip", new { area = "" });
                }
                else
                {
                    return RedirectToAction("Index", "CadastroEquip");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            CadastroEquip equipamento = await _contexto.CadastroEquipamentos.Where(x => x.Id == id).FirstOrDefaultAsync();
            return PartialView("_EditEquipPartialView", equipamento);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(CadastroEquip cadastro)
        {
            if (_contexto.CadastroEquipamentos.Any(x => x.IdPatrimonio == cadastro.IdPatrimonio && x.Id != cadastro.Id))
            {
                ModelState.AddModelError(nameof(cadastro.IdPatrimonio), "Máquina já cadastrada");
                return PartialView("_EditEquipPartialView", cadastro);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    _contexto.CadastroEquipamentos.Update(cadastro);
                    await _contexto.SaveChangesAsync();
                    return RedirectToAction("Index", "CadastroEquip", new { area = "" });
                }
                else
                {
                    return RedirectToAction("Index", "CadastroEquip");
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Excluir(CadastroEquip equipamento)
        {
            var cadastro = await _contexto.CadastroEquipamentos.FindAsync(equipamento.Id);
            cadastro.profissional_HD = HttpContext.Session.GetString("nome");
            _contexto.CadastroEquipamentos.Update(cadastro);
            await _contexto.SaveChangesAsync();
            _contexto.CadastroEquipamentos.Remove(cadastro);
            await _contexto.SaveChangesAsync();
            return RedirectToAction("Index", "CadastroEquip", new { area = "" });
        }
    }
}
