using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PlanQuinquenal.Core.Utilities
{
    public class HashService
    {

        private const string PasswordHash = "pass75dc@avz10";
        private const string SaltKey = "s@lAvz10";
        private const string VIKey = "@1B2c3D4e5F6g7H8";
        private const int KeySize = 128;
        private readonly IConfiguration configuration;
        private readonly string _pathRoot;


        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;
        public HashService(IServiceProvider serviceProvider,IServiceScopeFactory factory)
        {
            var env = serviceProvider.GetService<IHostingEnvironment>();
            _pathRoot = $"{env.ContentRootPath}{Constantes.PathFinanciamientoTemplate}";
            configuration = factory.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>();
        }        
        public string Encriptar(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return default;

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

            var keyBytes = new Rfc2898DeriveBytes(PasswordHash, System.Text.Encoding.ASCII.GetBytes(SaltKey)).GetBytes(KeySize / 8);
            var symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, System.Text.Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }

                memoryStream.Close();
            }

            return Convert.ToBase64String(cipherTextBytes);
        }

        public string Desencriptar(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return default;

            var cipherTextBytes = Convert.FromBase64String(encryptedText);
            var keyBytes = new Rfc2898DeriveBytes(PasswordHash, System.Text.Encoding.ASCII.GetBytes(SaltKey)).GetBytes(KeySize / 8);
            var symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, System.Text.Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            var plainTextBytes = new byte[cipherTextBytes.Length];

            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return System.Text.Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
        public async Task<string> GeneraDobuleFactor()
        {
            var characters = "1234567890";
            var Charsarr = new char[6];
            var random = new Random();

            for (int i = 0; i < Charsarr.Length; i++)
            {
                Charsarr[i] = characters[random.Next(characters.Length)];
            }

            var resultString = new String(Charsarr);
            return resultString;
        }
        public async Task<bool> EnviarDobleFactor<T>(EmailData<T> obj, DobleFactorDTO message, string templateKey)
        {

            string smtp = configuration.GetSection("EmailSettings").GetSection("Smtp").Value;
            string correoEnvioConf = configuration.GetSection("EmailSettings").GetSection("CorreoEnvio").Value;
            string key = configuration.GetSection("EmailSettings").GetSection("Key").Value;
            string port = configuration.GetSection("EmailSettings").GetSection("Port").Value;
            string ruta = "";
            ruta = $@"{_pathRoot}{obj.HtmlTemplateName}";
            string html = System.IO.File.ReadAllText(ruta);
            string body = Engine.Razor.RunCompile(html, $"{templateKey}", typeof(T), message);
            string correoDestino = string.Join(',', obj.EmailList);
            string correoSend = correoEnvioConf;
            string clave = key;
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(smtp, Convert.ToInt32(port));
            mail.From = new System.Net.Mail.MailAddress(correoSend);
            mail.To.Add(correoDestino);
            mail.Subject = "Codigo de verificacion";
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpServer.Port = Convert.ToInt32(port);
            SmtpServer.UseDefaultCredentials = true;
            SmtpServer.EnableSsl = false;
            SmtpServer.Send(mail);
            return true;
        }
        public async Task<JwtResponse> ConstruirToken(Usuario credencialesUsuario, JwtResponse jwt)
        {
            var claims = new List<Claim>()
            {
                new Claim("$I$Us$@I@D", credencialesUsuario.cod_usu.ToString()),
                new Claim("LastSesion",credencialesUsuario.LastSesion.ToString()),
                new Claim("Jti",Guid.NewGuid().ToString())
            };

            var llave = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("ASK9DASDASJD9ASJD9ASJDA9SJDAS9JDAS9JDA9SJD9ASJDAS9JDAS9DJAS9JDAS9DJAS9DJAS9DJAS9DAJS"));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddDays(200);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creds);

            jwt.AuthToken = new JwtSecurityTokenHandler().WriteToken(securityToken);
            jwt.ExpireIn = expiracion;
            return jwt;
          
        }

        public  void Parrafo(iTextSharp.text.Document doc, string tit, iTextSharp.text.Font _standardFont, bool align)
        {

            Paragraph titulo = new Paragraph($"{tit}", _standardFont);
            if (align)
            {
                titulo.Alignment = Element.ALIGN_CENTER;
            }
            else
            {
                titulo.IndentationLeft = 53;
                titulo.IndentationRight = 53;
            }

            doc.Add(titulo);
            doc.Add(Chunk.SPACETABBING);
        }
        public  void Titulo(iTextSharp.text.Document doc)
        {
            string ruta = configuration["LOGO"];
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(ruta);
            logo.ScaleAbsolute(120, 45);
            doc.Add(logo);

        }
        public  void GenerarTextAreaLista(string texto, iTextSharp.text.Document doc)
        {
            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            Paragraph actividadesG = new Paragraph(texto, _standardFont);

            actividadesG.IndentationLeft = 50;
            actividadesG.IndentationRight = 50;
            actividadesG.Alignment = Element.ALIGN_JUSTIFIED;
            PdfPCell cellAc = new PdfPCell(actividadesG);
            cellAc.BorderWidth = 1f;
            cellAc.BorderColor = BaseColor.GRAY;
            cellAc.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            cellAc.PaddingBottom = 6;
            cellAc.PaddingLeft = 12;
            cellAc.PaddingRight = 12;

            // Add the cell to a PdfPTable
            PdfPTable tableAC = new PdfPTable(1);
            tableAC.AddCell(cellAc);

            // Add the table to the document
            doc.Add(tableAC);
        }
        public  void GenerarLista(List<string> lista, iTextSharp.text.Document doc)
        {
            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            string pasosConcatena = "";
            int contador = 0;
            foreach (var item in lista)
            {
                
                pasosConcatena +=  item + "\n";
            }
            Paragraph proximos_pasos = new Paragraph($"{pasosConcatena}", _standardFont);
            proximos_pasos.IndentationLeft = 53;
            proximos_pasos.IndentationRight = 53;
            proximos_pasos.Alignment = Element.ALIGN_JUSTIFIED;
            doc.Add(proximos_pasos);
            doc.Add(Chunk.NEWLINE);
        }
    }
}
