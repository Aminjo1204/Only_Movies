using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using OnlyMoviesProject.Webapi.Infrastructure;
using OnlyMoviesProject.Webapi.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OnlyMoviesProject.Webapi.Controllers
{
    /// <summary>
    /// Controller to handle OAuth2 Sign-in for the mailer deamon account.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AzureAdClient _adClient;
        private readonly OnlyMoviesContext _db;
        private readonly byte[] _key;
        private string AuthorizeMailUrl => $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/account/authorizeMailAccount";

        public AccountController(AzureAdClient adClient, OnlyMoviesContext db, IConfiguration _config)
        {
            _adClient = adClient;
            _db = db;
            _key = Convert.FromBase64String(_config["TokenEncryptionKey"] ?? throw new ApplicationException("Secret is not set."));
        }

        /// <summary>
        /// GET /account/signinMailaccount
        /// </summary>
        /// <returns></returns>
        [HttpGet("signinMailaccount")]
        public IActionResult SigninMailaccount()
        {
            return Redirect(_adClient.LoginUrl(AuthorizeMailUrl));
        }

        [HttpGet("signoutMailaccount")]
        public async Task<IActionResult> SignoutMailaccount()
        {
            var config = await _db.GetConfig();
            config.MailerAccountname = null;
            config.MailerRefreshToken = null;
            await _db.SetConfig(config);
            return Redirect("/");
        }

        /// <summary>
        /// Callback for SigninMailaccount. Route has to be corresponding with callback URL
        /// in the Azure App Registration.
        /// </summary>
        [HttpGet("authorizeMailAccount")]
        public async Task<IActionResult> AuthorizeMailAccount([FromQuery] string code)
        {
            var (authToken, refreshToken) = await _adClient.GetToken(AuthorizeMailUrl, code);
            if (refreshToken is null) { return BadRequest("No refresh token in payload."); }
            var graph = _adClient.GetGraphServiceClientFromToken(authToken);
            var me = await graph.Me.Request().GetAsync();
            await _db.SetMailerAccount(me.Mail, refreshToken, _key);
            await _adClient.SendMail(authToken, "Account registriert", "Du hast deinen Account zum Senden von Statusmeldungen erfolgreich registriert.");
            return Redirect("/");
        }

        /// <summary>
        /// GET /account/sendTestMail
        /// </summary>
        /// <returns></returns>
        [HttpGet("sendTestMail")]
        public async Task<IActionResult> SendTestMail()
        {
            var (accountname, refreshToken) = await _db.GetMailerAccount(_key);
            if (accountname is null) { return Unauthorized("Invalid or missing mail accountname."); }
            if (refreshToken is null) { return Unauthorized("Invalid or missing mail refresh token."); }
            // An auth token is only valid for 60-90 minutes. We have to generate a new auth token from
            // our refresh token.
            var (authToken, newRefreshToken) = await _adClient.GetNewToken(refreshToken);
            if (newRefreshToken is null) { return Unauthorized("No new refresh token provided."); }
            // The old refresh token is now invalid, we have to set the new token.
            await _db.SetMailerAccount(accountname, newRefreshToken, _key);
            await _adClient.SendMail(authToken, "Testmail", "Testmail gesendet mit SendTestMail.");
            return NoContent();
        }
    }
}