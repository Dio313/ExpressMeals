using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using AutoMapper;
using ExpressMeals.Contracts.ViewModels;
using ExpressMeals.Contracts.Wrappers;
using ExpressMeals.Domains.Exceptions;
using ExpressMeals.Domains.helpers;
using ExpressMeals.Infrastructures.Identities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ExpressMeals.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountsController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IMapper mapper, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpPost("register")]
        public async Task<ApiResponse<RegisterVm>> RegisterAsync(RegisterVm request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw  new DuplicateException($"Email: {request.Email} already exists, please choose another email", HttpStatusCode.Conflict);
            }
            if (request.Password != request.ConfirmPassword)
            {
               throw new ForbiddenException("Password and confirm password do not match");
            }
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = true // in production change to false
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var urlConfirmation = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/accounts/emailconfirmation/?userid={HttpUtility.UrlEncode(user.Id)}&code={HttpUtility.UrlEncode(code)}";
                //await _emailSender.SendMail(user.Email, "Email confirmation", $"Please confirm your account by <a href='{urlConfirmation}'>clicking here</a>");

                await _userManager.AddToRoleAsync(user, RoleConstants.UserRole);
            }
            else
            {
                return new ApiResponse<RegisterVm>(false, result.Errors.Select(a => a.Description.ToString()).ToList(),
                    request);
            }

            return new ApiResponse<RegisterVm>(true, null, new RegisterVm());
        }

        [HttpPost("emailConfirmation")]
        public async Task<ApiResponse<bool>> EmailConfirmation( EmailConfirmationVm confirmationRequest)
        {
            var user = await _userManager.FindByIdAsync(confirmationRequest.UserId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User not found");
            }
            var result = await _userManager.ConfirmEmailAsync(user, confirmationRequest.Code);
            return new ApiResponse<bool>(true, null, result.Succeeded);
        }


        [HttpPost("login")]
        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginVm request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new AuthenticationException ( "User not found", HttpStatusCode.Unauthorized);
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {
                throw new AuthenticationException("Invalid Credentials.", HttpStatusCode.Unauthorized);
            }
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            var token = await GenerateJwtAsync(user);
        
            var response = new LoginResponse { AccessToken = token, RefreshToken = user.RefreshToken};

            return new ApiResponse<LoginResponse>(true, null, response);
        }

        [HttpPost("refresh")]
        public async Task<ApiResponse<LoginResponse>> GetRefreshTokenAsync(LoginResponse request)
        {
            ClaimsPrincipal principal = GetPrincipalFromExpiredToken(request.AccessToken);

            var user = await _userManager.FindByNameAsync(principal.Identity?.Name!);

            if (user is null)
            {
                throw new AuthenticationException ( "User not found", HttpStatusCode.Unauthorized);
            }
        
            if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new AuthenticationException("Invalid Credentials.", HttpStatusCode.Unauthorized);
            }

            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            var response = new LoginResponse() { AccessToken = token, RefreshToken = user.RefreshToken};

            return new ApiResponse<LoginResponse>(true, null, response);
        }

        [HttpGet("user/logged")]
        [Authorize]
        public async Task<ApiResponse<UserVm>> GetLoggedUserInfo(ClaimsPrincipal user)
        {
            var userInDb = await _userManager.FindByNameAsync(user.Identity!.Name!);
            if (userInDb is null)
            {
                throw new AuthenticationException ( "User not found", HttpStatusCode.Unauthorized);
            }

            var userDto = _mapper.Map<UserVm>(userInDb);

            return new ApiResponse<UserVm>(true, null, userDto);
        }

        [HttpPost("ForgotPassword")]
        public async Task<ApiResponse<ForgotPasswordVm>> ForgotPassword(ForgotPasswordVm forgotPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordRequest.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                throw new AuthenticationException ( "User not found", HttpStatusCode.Unauthorized);
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var urlConfirmation = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/account/changepassword/?code={HttpUtility.UrlEncode(code)}";
            await _emailSender.SendMail(user.Email, "Reset password", $"Please reset your password by <a href='{urlConfirmation}'>clicking here</a>");
            return new ApiResponse<ForgotPasswordVm>(true, null, new ForgotPasswordVm());
        }


        [HttpPost("changePassword")]
        public async Task<ApiResponse<IdentityResult>> ChangePasswordAsync([FromBody]ChangePasswordVm changePassword, [FromRoute] ClaimsPrincipal user)
        {
            var userInDb = await _userManager.FindByNameAsync(user.Identity!.Name!);
            if (userInDb is null)
            {
                throw new AuthenticationException ( "User not found", HttpStatusCode.Unauthorized);
            }
            var result = await _userManager.ChangePasswordAsync(userInDb,changePassword.OldPassword,changePassword.NewPassword);

            return new ApiResponse<IdentityResult>(true, null, result);
        }




        #region helpers

     private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenHelper.Secret)),
            ValidateIssuer = true,
            ValidateAudience = true,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,

            ValidIssuer = TokenHelper.Issuer,
            ValidAudience = TokenHelper.Audience,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    
    private async Task<string> GenerateJwtAsync(ApplicationUser user)
    {
        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        return token;
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(2),
            signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);
        return encryptedToken;
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        //Adding Claims to the token
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();
        for (int i = 0; i < roles.Count; i++)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, roles[i]));
            var thisRole = await _roleManager.FindByNameAsync(roles[i]);
            var allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole!);
            foreach (var permission in allPermissionsForThisRoles)
            {
                permissionClaims.Add(permission);
            }
        }

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

        return claims;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(TokenHelper.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    #endregion

    }
}
