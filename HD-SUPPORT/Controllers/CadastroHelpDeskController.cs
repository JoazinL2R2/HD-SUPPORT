using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using HD_SUPPORT.Models;

namespace HD_SUPPORT.Controllers
{
    public class CadastroHelpDeskController : Controller
    {
        private readonly BancoContexto _contexto;

        public CadastroHelpDeskController(BancoContexto contexto)
        {
            _contexto = contexto;
        }
        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([Bind("Id,Nome,Email,Senha,Foto")] CadastroHelpDesk cadastro, IFormFile Imagem)
        {
            if (_contexto.CadastroHD.Any(x => x.Email == cadastro.Email))
            {
                ModelState.AddModelError(nameof(cadastro.Email), "Email existente");
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Imagem.CopyTo(ms);
                        cadastro.Foto = ms.ToArray();
                    }

                    await _contexto.CadastroHD.AddAsync(cadastro);
                    await _contexto.SaveChangesAsync();
                    return RedirectToAction("Index", "CadastroFunc", new { area = "" });
                }
                return View(cadastro);
            }
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(CadastroHelpDesk cadastro)
        {
            if (_contexto.CadastroHD.Any(x => x.Email == cadastro.Email && x.Senha == cadastro.Senha))
            {
                var usuario = _contexto.CadastroHD.Where(b => b.Email == cadastro.Email).FirstOrDefault();
                HttpContext.Session.SetString("nome", usuario.Nome);
                HttpContext.Session.Set("foto", usuario.Foto);
                return RedirectToAction("Index", "CadastroFunc", new { area = "" });
            }
            else
            {
                ModelState.AddModelError(nameof(cadastro.Senha), "Email ou senha incorretos!");
                return View();
            }
        }
    }
}