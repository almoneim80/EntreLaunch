namespace EntreLaunch.Services.AuthenticationSvc
{
    public class TokenService : ITokenService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly UserManager<User> _userManager;
        public TokenService(IOptions<JwtConfig> jwtConfig, UserManager<User> userManager)
        {
            _jwtConfig = jwtConfig.Value;
            _userManager = userManager;
        }

        //// <inheritdoc />
        public async Task<JWTokenDto> GenerateTokenWithRefreshTokenAsync(User user, IEnumerable<Claim>? extraClaims = null, TimeSpan? expiresIn = null)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            if (extraClaims != null)
            {
                claims.AddRange(extraClaims);
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var expires = DateTime.UtcNow.Add(expiresIn ?? TimeSpan.FromHours(2));

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                expires: expires,
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            return new JWTokenDto
            {
                Token = tokenString,
                Expiration = expires,
                RefreshToken = refreshToken
            };
        }

        //// <inheritdoc />
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        //// <inheritdoc />
        public string HashRefreshToken(string rawToken)
        {
            if (string.IsNullOrEmpty(rawToken))
            {
                return string.Empty;
            }

            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(rawToken);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
