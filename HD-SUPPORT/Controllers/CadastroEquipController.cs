using HD_SUPPORT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
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
        public async Task<IActionResult> Index(string searchString, string disponivel = "2", int paginaAtual = 1)
        {
            if (HttpContext.Session.GetString("nome") == null)
            {
                return RedirectToAction("LogOut", "Home", new { area = "" });
            }

            var equipamentos = await _contexto.CadastroEquipamentos.ToListAsync();
            var equipamentosFiltrados = new List<CadastroEquip>();
            var disponibilidade = true;

            if (disponivel == "0")
            {
                disponibilidade = false;
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                equipamentosFiltrados = equipamentos
                    .Where(e => e.IdPatrimonio.ToString().Contains(searchString)
                                || e.Modelo.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                || e.Processador.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                || e.SistemaOperacionar.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                //disponibilidade
                if (disponivel != "2" && disponivel != null)
                {
                    equipamentosFiltrados = equipamentosFiltrados.Where(e => e.Disponivel == disponibilidade).ToList();
                }
            }
            else if(disponivel!="2" && disponivel != null)
            {
                equipamentosFiltrados = equipamentos.Where(e => e.Disponivel == disponibilidade).ToList();
            }
            else
            {
                equipamentosFiltrados = equipamentos;
            }

            TempData["pesquisa"] = searchString;

            TempData["QuantidadeDados"] = equipamentosFiltrados.Count;

            TempData["paginaAtual"] = paginaAtual;

            TempData["disponivel"] = disponivel;

            var pagina = (paginaAtual - 1) * 6;

            var maximo = 6;

            if (equipamentosFiltrados.Count < 6 + pagina)
            {
                maximo = equipamentosFiltrados.Count - pagina;
            }

            equipamentosFiltrados = equipamentosFiltrados.GetRange(pagina, maximo);

            TempData["QuantidadeDadosTabela"] = equipamentosFiltrados.Count;
            return View(equipamentosFiltrados);
        }

        [HttpGet]
        public IActionResult NovoCadastro()
        {
            return PartialView("_NovoCadastroEquipPartialView");
        }

        public bool dadosExistentes(CadastroEquip equipamento)
        {
            if (equipamento.IdPatrimonio == null || equipamento.Modelo == null || equipamento.Processador == null || 
                equipamento.SistemaOperacionar == null)
            {
                return false;
            }
            return true;
        }

        [HttpPost]
        public async Task<IActionResult> NovoCadastro(CadastroEquip equipamento)
        {
            if (ModelState.IsValid)
            {
                if (_contexto.CadastroEquipamentos.Any(x => x.IdPatrimonio == equipamento.IdPatrimonio && x.Id != equipamento.Id))
                {
                    TempData["ErroAtualizacao"] = "maquina já existente";
                    return RedirectToAction("Index");
                }
                if (dadosExistentes(equipamento))
                {
                    _contexto.CadastroEquipamentos.Add(equipamento);
                    await _contexto.SaveChangesAsync();
                    return RedirectToAction("Index", "CadastroEquip", new { area = "" });
                }
                else
                {
                    TempData["ErroAtualizacao"] = "Preencha todos os dados";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index", "CadastroEquip");
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
                TempData["ErroAtualizacao"] = "maquina já existente";
                return RedirectToAction("Index");
            }
            else
            {
                if (dadosExistentes(cadastro))
                {
                    _contexto.CadastroEquipamentos.Update(cadastro);
                    await _contexto.SaveChangesAsync();
                    return RedirectToAction("Index", "CadastroEquip", new { area = "" });
                }
                else
                {
                    TempData["ErroAtualizacao"] = "Preencha todos os dados";
                    return RedirectToAction("Index");
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Excluir(CadastroEquip equipamento)
        {
            if (HttpContext.Session.GetString("nome") == null)
            {
                return RedirectToAction("LogOut", "Home", new { area = "" });
            }
            var cadastro = await _contexto.CadastroEquipamentos.FindAsync(equipamento.Id);
            cadastro.profissional_HD = HttpContext.Session.GetString("profissional") + " - " + HttpContext.Session.GetString("nome");
            _contexto.CadastroEquipamentos.Update(cadastro);
            await _contexto.SaveChangesAsync();
            _contexto.CadastroEquipamentos.Remove(cadastro);
            await _contexto.SaveChangesAsync();
            return RedirectToAction("Index", "CadastroEquip", new { area = "" });
        }
    }
}
