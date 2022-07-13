using Api.ViewModels.Token;
using Common.Entities;
using Common.Reposotories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        
        [HttpPut]
        //[Route("Login")]
        public IActionResult Login([FromBody] LoginVM model)
        {
            UserManagementDbContext context = new UserManagementDbContext();
            User loggedUser = context.Users.Where(u => u.Username == model.Username
                                                  && u.Password == model.Password).FirstOrDefault();
            if (loggedUser == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim("LoggedUserId",loggedUser.Id.ToString())
            };

            string jwt = GenerateAccessToken(claims);
            
            RefreshToken refreshToken = new RefreshToken();

            refreshToken.OwnerId = loggedUser.Id;
            refreshToken.Token = GenerateRefreshToken();
            refreshToken.ExpirationTime = DateTime.Now.AddMinutes(30);
            refreshToken.Count = 1;

            context.RefreshTokens.Add(refreshToken);
            context.SaveChanges();


            return Ok(new { success = true, token = jwt ,refreshToken = refreshToken});

        }

        [HttpPost]
        [Route("RefreshToken")]
        public IActionResult RefreshToken([FromBody] RefreshTokenVM model)
        {
            UserManagementDbContext context = new UserManagementDbContext();
            RefreshToken item = context.RefreshTokens.Where(u => u.Token == model.Token).FirstOrDefault();
            if (item == null)
            {
                return NotFound();
            }

            RefreshToken refreshToken = new RefreshToken();

            refreshToken.OwnerId = item.Id;
            refreshToken.Token = GenerateRefreshToken();
            refreshToken.ExpirationTime = DateTime.Now.AddMinutes(30);
            refreshToken.Count += 1;

            context.RefreshTokens.Update(refreshToken);
            context.SaveChanges();

            var claims = new[]
            {
                new Claim("LoggedUserId",item.OwnerId.ToString())
            };

            string jwt = GenerateAccessToken(claims);
            

            return Ok(new { success = true, token = jwt, refreshToken = refreshToken });

        }

        public string GenerateAccessToken(IEnumerable<Claim> claim)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("2s5v8y/B?E(H+MbQ"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: "http://localhost:8230",
                audience: "http://localhost:8230",
                claims: claim,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return tokenString;

        }
        public string GenerateRefreshToken()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return stringChars.ToString();
        }

    }
}
