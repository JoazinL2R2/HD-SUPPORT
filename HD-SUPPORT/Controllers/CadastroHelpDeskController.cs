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

        public const int ImageMinimumBytes = 512;

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
                    if (!string.Equals(Imagem.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Imagem.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Imagem.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Imagem.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Imagem.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Imagem.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(nameof(cadastro.Foto), "Formato de imagem incopatível");
                        return View();
                    }
                    var postedFileExtension = Path.GetExtension(Imagem.FileName);
                    if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(nameof(cadastro.Foto), "Formato de imagem incopatível");
                        return View();
                    }
                    if (Imagem.Length < ImageMinimumBytes)
                    {
                        ModelState.AddModelError(nameof(cadastro.Foto), "Arquivo muito grande");
                        return View();
                    }
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

            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("CadastroFunc", "Index");

            if (HttpContext.Session.GetString("nome") != null)
            {
                HttpContext.Session.Clear();
            }

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
                    AllowRefresh = true,

                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),properties
                    );
                return RedirectToAction("Index", "CadastroFunc");
            }

            ViewData["validate"] = "Usuario não encontrado";
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