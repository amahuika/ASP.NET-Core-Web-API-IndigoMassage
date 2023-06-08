using Indigo.Models.DTO;
using Indigo.Models.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace IndigoMassage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {

        private ApiResponseDTO _response;
        private readonly UserManager<ApplicationUser> _userManager;

        private string secretKey;


        public AuthenticateController(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {


            _userManager = userManager;

            _response = new ApiResponseDTO();
            secretKey = configuration.GetValue<string>("JwtKey:SecretKey");

        }


        // handle login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginModel)
        {
            // check ModelState is valid
            if (!ModelState.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response?.ErrorMessages?.Add("Invalid request");

                return BadRequest(_response);
            }


            try
            {

                // get user from db
                ApplicationUser userFromDb = await _userManager.FindByEmailAsync(loginModel.Email);

                if (userFromDb == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }


                // check if password is valid
                bool isValid = await _userManager.CheckPasswordAsync(userFromDb, loginModel.Password);

                if (!isValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response?.ErrorMessages?.Add("*Email or Password is incorrect");

                    return BadRequest(_response);
                }

                // generate jwtToken
                var roles = await _userManager.GetRolesAsync(userFromDb);




                byte[] key = Encoding.ASCII.GetBytes(secretKey);

                JwtSecurityTokenHandler tokenHandler = new();

                SecurityTokenDescriptor tokenDescriptor = new()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, userFromDb.FirstName),
                    new Claim(ClaimTypes.NameIdentifier, userFromDb.Id),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),

                    }),

                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

                };

                SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

                LoginResponseDTO loginResponse = new()
                {
                    Name = userFromDb.FirstName,

                    Token = tokenHandler.WriteToken(token)
                };

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = loginResponse;

                return Ok(_response);
            }
            catch (Exception ex)
            {

                _response.ErrorMessages.Add(ex.Message);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }


        }




    }
}
