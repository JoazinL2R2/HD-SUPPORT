
using HD_SUPPORT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HD_SUPPORT.Controllers
{
    public class EmprestimoController : Controller
    {
        private readonly BancoContexto _contexto;
        public EmprestimoController(BancoContexto contexto)
        {
            _contexto = contexto;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult NovoEmprestimo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NovoEmprestimo(EmprestimoViewModel equipamento)
        {
            if (_contexto.CadastroEmprestimos.Any(x => x.Disponivel == false))
            {
                ModelState.AddModelError(nameof(equipamento.Equipamento.IdPatrimonio), "Maquina em emprestimo");
                return View();
            }
            else
            {
                var disponivel = _contexto.Cadas(x => x.Disponivel == false);
                await _contexto.CadastroEmprestimos.AddAsync(equipamento.Equipamento);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "EmprestimoViewModel", new { area = "" });
            }
        }


    }
}
