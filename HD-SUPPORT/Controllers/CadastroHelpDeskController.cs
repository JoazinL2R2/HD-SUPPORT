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
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity.UI.Services;
using HD_SUPPORT.Services;
using Microsoft.Extensions.Caching.Memory;


namespace HD_SUPPORT.Controllers
{
    public class CadastroHelpDeskController : Controller
    {
        private readonly BancoContexto _contexto;
        private readonly IEmailService _email;
        private readonly IMemoryCache _memoryCache;

        public CadastroHelpDeskController(BancoContexto contexto, IEmailService email, IMemoryCache memoryCache)
        {
            _contexto = contexto;
            _email = email;
            _memoryCache = memoryCache;
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
                    if (!ImagemFormatoCorreto(Imagem))
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
                else
                {
                    if (Imagem == null)
                    {
                        ModelState.AddModelError(nameof(cadastro.Foto), "Insira uma foto de perfil");
                        return View();
                    }
                    return View(cadastro);
                }
            }
        }

        public bool ImagemFormatoCorreto(IFormFile Imagem)
        {
            if (!string.Equals(Imagem.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Imagem.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Imagem.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Imagem.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Imagem.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Imagem.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            var postedFileExtension = Path.GetExtension(Imagem.FileName);
            if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        [HttpGet]
        public IActionResult Login()
        {

            if (User.Identity.IsAuthenticated) {
                return RedirectToAction("LogOut", "Home");
            }

            if (HttpContext.Session.GetString("nome") != null)
            {
                HttpContext.Session.Clear();
            }


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(CadastroHelpDesk cadastro)
        {
            if (_contexto.CadastroHD.Any(x => x.Email == cadastro.Email && x.Senha == cadastro.Senha)
                )
            {
                var emailSeparado = cadastro.Email.Split("@");
                var profissional = "HelpDesk";
                if (emailSeparado[1].ToUpper().Contains("RH"))
                {
                    profissional = "RH";
                }

                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, cadastro.Email),
                    new Claim(ClaimTypes.Role, profissional)
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                    );

                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,

                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), properties);
                var usuario = _contexto.CadastroHD.Where(b => b.Email == cadastro.Email).FirstOrDefault();
                criarSessao(usuario, profissional);
                return RedirectToAction("Index", "CadastroFunc", new { area = "" });
            }
            else
            {
                ModelState.AddModelError(nameof(cadastro.Senha), "Email ou senha incorretos!");
                return View();
            }
        }

        public void criarSessao(CadastroHelpDesk usuario, string profissional)
        {
            HttpContext.Session.SetString("nome", usuario.Nome);
            HttpContext.Session.Set("foto", usuario.Foto);
            var emailSeparado = usuario.Email.Split("@");
            emailSeparado[0] = emailSeparado[0].Substring(0, Convert.ToInt16(MathF.Ceiling(emailSeparado[0].Length / 2))).PadRight(emailSeparado[0].Length, '*');
            var email = emailSeparado[0] + "@" + emailSeparado[1];
            HttpContext.Session.SetString("email", email);
            var senha = "".PadRight(usuario.Senha.Length, '*');
            HttpContext.Session.SetString("senha", senha);
            HttpContext.Session.SetInt32("Id", usuario.Id);
            HttpContext.Session.SetString("profissional", profissional);
        }

        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var perfil = await _contexto.CadastroHD.FindAsync(HttpContext.Session.GetInt32("Id"));
            return View(perfil);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            CadastroHelpDesk cadastro = _contexto.CadastroHD.Where(x => x.Id == id).FirstOrDefault();
            cadastro.Senha = "";
            return View(cadastro);
        }
        [HttpPost]
        public IActionResult Atualizar([Bind("Id,Nome,Email,Senha,Foto")] CadastroHelpDesk cadastro, IFormFile Imagem)
        {
            try
            {
                if (cadastro == null)
                {
                    return BadRequest("O objeto CadastroHelpDesk está nulo.");
                }

                if (_contexto.CadastroHD.Any(x => x.Email == cadastro.Email && x.Id != cadastro.Id))
                {
                    ModelState.AddModelError(nameof(cadastro.Email), "Email existente");
                    return View("Edit", cadastro);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        if (!ImagemFormatoCorreto(Imagem))
                        {
                            ModelState.AddModelError(nameof(cadastro.Foto), "Formato de imagem incopatível");
                            return View("Edit", cadastro);
                        }
                        if (Imagem.Length < ImageMinimumBytes)
                        {
                            ModelState.AddModelError(nameof(cadastro.Foto), "Arquivo muito grande");
                            return View("Edit", cadastro);
                        }
                        using (MemoryStream ms = new MemoryStream())
                        {
                            Imagem.CopyTo(ms);
                            cadastro.Foto = ms.ToArray();
                        }
                        _contexto.CadastroHD.Update(cadastro);
                        _contexto.SaveChanges();
                        var usuario = _contexto.CadastroHD.Where(b => b.Email == cadastro.Email).FirstOrDefault();
                        criarSessao(usuario, HttpContext.Session.GetString("profissional"));
                        return RedirectToAction("Perfil");
                    }
                    else
                    {
                        if (Imagem == null)
                        {
                            ModelState.AddModelError(nameof(cadastro.Foto), "Insira uma foto de perfil");
                            return View("Edit", cadastro);
                        }
                        return View("Edit", cadastro);
                    }
                }
            }
            catch (Exception ex)
            {
                // Adicione logs detalhados ou mensagens de console para identificar a causa da exceção.
                Console.WriteLine($"Erro durante a atualização: {ex.Message}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Excluir(CadastroHelpDesk cadastro)
        {
            var perfil = await _contexto.CadastroHD.FindAsync(cadastro.Id);
            _contexto.CadastroHD.Remove(perfil);
            await _contexto.SaveChangesAsync();
            return RedirectToAction("LogOut", "Home", new { area = "" });
        }
        public IActionResult EnviarEmail(string email)
        {
            const string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            int comprimento = 5;
            char[] codigo = new char[comprimento];
            for (int i = 0; i < comprimento; i++)
            {
                codigo[i] = caracteres[random.Next(caracteres.Length)];
            }
            var CodigoAleatorio = new string(codigo);
            string emailUsuario = email;
            string mensagem = $"Seu Codigo de verificação é: {CodigoAleatorio}. Não Compartilhe este codigo";
            bool emailEnviado = _email.Enviar(emailUsuario,"HD - Support  -- Codigo de verificação", mensagem);
            TempData["CodigoAleatorio"] = CodigoAleatorio;
            if (emailEnviado) {
                return View("Cadastrar");
            }
            else
            {
                TempData["MensagemErro"] = "Não foi possivel enviar o email ou o email é invalido. Confira o email e tente novamente";
            }
            return View("Cadastrar");
        }

        public IActionResult VerificacaoEmail(string codigo, string email)
        {
            string codigoArmazenado = TempData["CodigoAleatorio"] as string;

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(email))
            {
                TempData["ErroAtualizacao"] = "Preencha Todos Os Campos";
                return RedirectToAction("Cadastrar", "CadastroCadastroHelpDesk");
            }

            if (codigo == codigoArmazenado)
            {
                TempData["sucessoAtualizacao"] = "Conta criada com sucesso";
                return RedirectToAction("Login", "CadastroHelpDesk");
            }
            else
            {
                TempData["ErroAtualizacao"] = "Codigo Incorreto";
                return RedirectToAction("Cadastrar", "CadastroCadastroHelpDesk");
            }
        }
    }
}