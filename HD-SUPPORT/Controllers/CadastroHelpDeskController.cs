using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using HD_SUPPORT.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


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
        public async Task<IActionResult> Cadastrar(CadastroHelpDesk cadastro)
        {
            if (_contexto.CadastroHD.Any(x => x.Email == cadastro.Email))
            {
                ModelState.AddModelError(nameof(cadastro.Email), "Email existente");
                return View();
            }
            else
            {
                await _contexto.CadastroHD.AddAsync(cadastro);
                await _contexto.SaveChangesAsync();
                return RedirectToAction("Index", "CadastroFunc", new { area = "" });
            }
        }
        [HttpGet]
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("CadastroFunc", "Index");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(CadastroHelpDesk cadastro)
        {
            if (cadastro.Email == "user@example.com" && 
                cadastro.Senha == "123"
                )
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier,cadastro.Email),
                    new Claim("OtherProperties","Example Role")
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                    );

                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true
                    
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),properties
                    );
                return RedirectToAction("Index", "Home");
            }

            ViewData["validate"] = "Usuario não encontrado";
            if (_contexto.CadastroHD.Any(x => x.Email == cadastro.Email && x.Senha == cadastro.Senha))
            {
                var usuario = _contexto.CadastroHD.Where(b => b.Email == cadastro.Email).FirstOrDefault();
                HttpContext.Session.SetString("nome", usuario.Nome);
                //HttpContext.Session.Set("foto", usuario.Foto);
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