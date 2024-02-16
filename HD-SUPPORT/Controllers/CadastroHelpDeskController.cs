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
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace HD_SUPPORT.Controllers
{
    public class CadastroHelpDeskController : Controller
    {
        private readonly BancoContexto _contexto;
        private readonly Services.IEmailSender emailSender;
        private readonly IMemoryCache _memoryCache;

        public CadastroHelpDeskController(BancoContexto contexto, Services.IEmailSender emailSender, IMemoryCache memoryCache)
        {
            _contexto = contexto;
            this.emailSender = emailSender;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        public string criarHash(string texto)
        {
            var md5 = MD5.Create();
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(texto);
            byte[] hash = md5.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public const int ImageMinimumBytes = 512;

        [HttpPost]
        public async Task<IActionResult> Cadastrar([Bind("Id,Nome,Email,Senha, Foto")] CadastroHelpDesk cadastro, IFormFile Imagem, string funcao, string verificado, string img_verificado = "")
        {
            if (ModelState.IsValid || verificado == "True")
            {
                if (verificado != "True")
                {
                    if (_contexto.CadastroHD.Any(x => x.Email == cadastro.Email))
                    {
                        ModelState.AddModelError(nameof(cadastro.Email), "Email existente");
                        return View();
                    }
                    else if (cadastro.Email.Split("@")[1] != "employer.com.br")
                    {
                        ModelState.AddModelError(nameof(cadastro.Email), "Email deve ter @employer.com.br");
                        Console.WriteLine("email paia");
                        return View();
                    }
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
                    TempData["podeverificar"] = "pode";
                    ViewData["imagem"] = Convert.ToBase64String(cadastro.Foto);
                    return View();
                }
                else
                {
                    cadastro.Nome = funcao + "-" + cadastro.Nome;
                    cadastro.Foto = Convert.FromBase64String(img_verificado);
                    cadastro.Senha = criarHash(cadastro.Senha);
                    await _contexto.CadastroHD.AddAsync(cadastro);
                    await _contexto.SaveChangesAsync();
                    return RedirectToAction("Index", "CadastroFunc", new { area = "" });
                }
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
                //Criação de sessão apenas para não aparecer "Sessão expirada" na página de login
                HttpContext.Session.SetString("nome", "teste");
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
            if (_contexto.CadastroHD.Any(x => x.Email == cadastro.Email && x.Senha == criarHash(cadastro.Senha)))
            {
                var usuario = _contexto.CadastroHD.Where(b => b.Email == cadastro.Email).FirstOrDefault();
                var NomeSeparado = usuario.Nome.Split("-");
                var profissional = "HelpDesk";
                if (NomeSeparado[0].ToUpper().Contains("RH"))
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
            HttpContext.Session.SetString("nome", usuario.Nome.Split("-")[1]);
            HttpContext.Session.Set("foto", usuario.Foto);
            HttpContext.Session.SetString("emailTodo", usuario.Email);
            var emailSeparado = usuario.Email.Split("@");
            emailSeparado[0] = emailSeparado[0].Substring(0, Convert.ToInt16(MathF.Ceiling(emailSeparado[0].Length / 2))).PadRight(emailSeparado[0].Length, '*');
            var email = emailSeparado[0] + "@" + emailSeparado[1];
            HttpContext.Session.SetString("email", email);
            HttpContext.Session.SetInt32("Id", usuario.Id);
            HttpContext.Session.SetString("profissional", profissional);
        }

        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            if (HttpContext.Session.GetString("nome") == null)
            {
                return RedirectToAction("LogOut", "Home", new { area = "" });
            }
            var perfil = await _contexto.CadastroHD.FindAsync(HttpContext.Session.GetInt32("Id"));
            return View(perfil);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("nome") == null)
            {
                return RedirectToAction("LogOut", "Home", new { area = "" });
            }
            CadastroHelpDesk cadastro = _contexto.CadastroHD.Where(x => x.Id == id).FirstOrDefault();
            cadastro.Senha = "";
            return PartialView("_EditarHDPartialView", cadastro);
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
                if (cadastro.Senha == null)
                {
                    return Json(new { success = false, mensage = "Preencha Todos os campos", data = cadastro });
                }
                if (cadastro.Foto == null)
                {
                    return Json(new { success = false, mensage = "Preencha Todos os campos", data = cadastro });
                }

                if (_contexto.CadastroHD.Any(x => x.Email == cadastro.Email && x.Id != cadastro.Id))
                {
                    return Json(new { success = false, mensage = "Email Existente", data = cadastro });
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        if (!ImagemFormatoCorreto(Imagem))
                        {
                            return Json(new { success = false, mensage = "Formato de imagem incopativel", data = cadastro });
                        }
                        if (Imagem.Length < ImageMinimumBytes)
                        {
                            return Json(new { success = false, mensage = "Arquivo muito extenso", data = cadastro });
                        }
                        using (MemoryStream ms = new MemoryStream())
                        {
                            Imagem.CopyTo(ms);
                            cadastro.Foto = ms.ToArray();
                        }
                        cadastro.Senha = criarHash(cadastro.Senha);
                        _contexto.CadastroHD.Update(cadastro);
                        _contexto.SaveChanges();
                        var usuario = _contexto.CadastroHD.Where(b => b.Email == cadastro.Email).FirstOrDefault();
                        criarSessao(usuario, HttpContext.Session.GetString("profissional"));
                        return Json(new { success = true, mensage = "Editado com sucesso", data = cadastro });
                    }
                    else
                    {
                        return Json(new { success = false, data = cadastro });
                    }
                }
            }
            catch (Exception ex)
            {
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
        [HttpPost]
        public IActionResult EnviarEmail(string email)
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            int comprimento = 5;
            char[] codigo = new char[comprimento];
            for (int i = 0; i < comprimento; i++)
            {
                codigo[i] = caracteres[random.Next(caracteres.Length)];
            }
            var CodigoAleatorio = new string(codigo);
            string mensagem = $"Seu Codigo de verificação é: {CodigoAleatorio}. Não Compartilhe este codigo";
            string subject = "HD - Support  -- Codigo de verificação";

            try
            {
                this.emailSender.SendEmailAsync(email, subject, mensagem);
                TempData["CodigoAleatorio"] = CodigoAleatorio;

                return Json(new { success = true, message = "Código enviado com sucesso." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao enviar o código por e-mail.", error = ex.Message });
            }
        }


        [HttpPost]
        public IActionResult VerificacaoEmail(string codigoVerificacao)
        {
            Console.WriteLine("codigo recebido do front:" + codigoVerificacao);
            var codigoArmazenado = TempData["CodigoAleatorio"] as string;
            if (codigoVerificacao == codigoArmazenado && codigoArmazenado != null)
            {
                TempData["sucessoAtualizacao"] = "Conta criada com sucesso";
                return Json(new { success = true, message = "Cadastro Criado com sucesso." });
            }
            else if (codigoVerificacao == null)
            {
                TempData["ErroAtualizacao"] = "Codigo vazio, tente novamente";
                return Json(new { success = false, message = "Código vazio, tente novamente." });
            }
            else
            {
                TempData["ErroAtualizacao"] = "Codigo Incorreto";
                return Json(new { success = false, message = "Código incorreto, tente novamente." });
            }
        }

        [HttpPost]
        public IActionResult EnviarEmailRecuperacao(string email, CadastroHelpDesk cadastro)
        {

            var EmailExistenteBaseDados = _contexto.CadastroHD.FirstOrDefault(x => x.Email == email);

            if (EmailExistenteBaseDados == null)
            {
                return Json(new { success = false, message = "E-mail não encontrado na base de dados." });
            }
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            int comprimento = 5;
            char[] codigo = new char[comprimento];
            for (int i = 0; i < comprimento; i++)
            {
                codigo[i] = caracteres[random.Next(caracteres.Length)];
            }
            var CodigoAleatorio = new string(codigo);
            string mensagem = $"Seu Código de RECUPERAÇÃO da senha é: {CodigoAleatorio}. Não Compartilhe este código";
            string subject = "HD - Support  -- Codigo de recuperação";

            try
            {
                this.emailSender.SendEmailAsync(email, subject, mensagem);
                TempData["CodigoAleatorioRecuperacao"] = CodigoAleatorio;

                return Json(new { success = true, message = "Código enviado com sucesso." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao enviar o código por e-mail.", error = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult VerificacaoRecuperacaoSenha(string codigoVerificacao)
        {
            Console.WriteLine("codigo recebido do front:" + codigoVerificacao);
            var codigoArmazenado = TempData["CodigoAleatorioRecuperacao"] as string;
            if (codigoVerificacao == codigoArmazenado && codigoArmazenado != null)
            {
                TempData["sucessoAtualizacao"] = "Conta criada com sucesso";
                return Json(new { success = true, message = "Senha alterada com sucesso." });
            }
            else if (codigoVerificacao == null)
            {
                TempData["ErroAtualizacao"] = "Codigo vazio, tente novamente";
                return Json(new { success = false, message = "Código vazio, tente novamente." });
            }
            else
            {
                TempData["ErroAtualizacao"] = "Codigo Incorreto";
                return Json(new { success = false, message = "Código incorreto, tente novamente." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MudarSenhaRecuperacao(string senha, string confirmacaoSenha, string email)
        {
            var cadastro = await _contexto.CadastroHD.FirstOrDefaultAsync(x => x.Email == email);
            if (senha != confirmacaoSenha)
            {
                return Json(new { success = false, message = "As senhas não conferem." });
            }
            cadastro.Senha = criarHash(senha);
            _contexto.CadastroHD.Update(cadastro);
            await _contexto.SaveChangesAsync();
            return Json(new { success = true, message = "Senha alterada com sucesso." });
        }

    }
}
