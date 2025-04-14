using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace TaskManager.Services
{
    public class AuthService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly string _jwtKey = "T4skM4n4g3r_S3cur1ty_K3y_2025_Str0ng_S3cur3_K3y"; // Clé plus longue pour la génération de tokens JWT
        private readonly string _refreshTokenPreferenceKey = "RefreshToken";
        private readonly TimeSpan _tokenExpiration = TimeSpan.FromHours(1);
        private readonly TimeSpan _refreshTokenExpiration = TimeSpan.FromDays(30);
        
        private Models.UserDto? _currentUser;
        private string? _currentToken;
        private string? _refreshToken;

        public AuthService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            InitializeFromPreferences();
        }

        private void InitializeFromPreferences()
        {
            // Récupérer le token de rafraîchissement des préférences (si l'utilisateur a coché "Se souvenir de moi")
            _refreshToken = Preferences.Get(_refreshTokenPreferenceKey, null);
        }

        public async Task<AuthResponse> Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Email) || 
                string.IsNullOrWhiteSpace(request.Password) || 
                string.IsNullOrWhiteSpace(request.FirstName) || 
                string.IsNullOrWhiteSpace(request.LastName))
            {
                return new AuthResponse { Success = false, Message = "Tous les champs sont obligatoires" };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new AuthResponse { Success = false, Message = "Les mots de passe ne correspondent pas" };
            }

            if (request.Password.Length < 6)
            {
                return new AuthResponse { Success = false, Message = "Le mot de passe doit contenir au moins 6 caractères" };
            }

            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                // Vérifier si l'utilisateur existe déjà
                var existingUser = await context.UserAccounts
                    .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);

                if (existingUser != null)
                {
                    return new AuthResponse { Success = false, Message = "Ce nom d'utilisateur ou cet email est déjà utilisé" };
                }

                // Créer un nouvel utilisateur
                var passwordSalt = GenerateSalt();
                var passwordHash = HashPassword(request.Password, passwordSalt);

                // Créer la base user pour les informations personnelles
                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                // Créer le compte utilisateur pour l'authentification
                var userAccount = new UserAccount
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    CreatedAt = DateTime.UtcNow,
                    UserId = user.Id
                };

                context.UserAccounts.Add(userAccount);
                await context.SaveChangesAsync();

                // Générer les tokens et retourner la réponse
                var tokens = GenerateTokens(userAccount);
                SaveRefreshToken(tokens.refreshToken);

                _currentUser = new Models.UserDto
                {
                    Id = userAccount.Id,
                    Username = userAccount.Username,
                    Email = userAccount.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                _currentToken = tokens.token;
                _refreshToken = tokens.refreshToken;

                return new AuthResponse
                {
                    Success = true,
                    Message = "Compte créé avec succès",
                    Token = tokens.token,
                    RefreshToken = tokens.refreshToken,
                    User = _currentUser
                };
            }
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UsernameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new AuthResponse { Success = false, Message = "Nom d'utilisateur/email et mot de passe obligatoires" };
            }

            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                // Trouver l'utilisateur par nom d'utilisateur ou email
                var userAccount = await context.UserAccounts
                    .Include(ua => ua.User)
                    .FirstOrDefaultAsync(u => 
                        u.Username == request.UsernameOrEmail || 
                        u.Email == request.UsernameOrEmail);

                if (userAccount == null)
                {
                    return new AuthResponse { Success = false, Message = "Utilisateur non trouvé" };
                }

                // Vérifier le mot de passe
                var hashedPassword = HashPassword(request.Password, userAccount.PasswordSalt);
                if (hashedPassword != userAccount.PasswordHash)
                {
                    return new AuthResponse { Success = false, Message = "Mot de passe incorrect" };
                }

                // Générer les tokens
                var tokens = GenerateTokens(userAccount);
                if (request.RememberMe)
                {
                    SaveRefreshToken(tokens.refreshToken);
                }

                // Mettre à jour les informations de l'utilisateur courant
                _currentUser = new Models.UserDto
                {
                    Id = userAccount.Id,
                    Username = userAccount.Username,
                    Email = userAccount.Email,
                    FirstName = userAccount.User?.FirstName ?? "",
                    LastName = userAccount.User?.LastName ?? ""
                };

                _currentToken = tokens.token;
                _refreshToken = tokens.refreshToken;

                return new AuthResponse
                {
                    Success = true,
                    Message = "Connexion réussie",
                    Token = tokens.token,
                    RefreshToken = tokens.refreshToken,
                    User = _currentUser
                };
            }
        }

        public async Task<AuthResponse?> CheckAuthStatus()
        {
            // Si l'utilisateur est déjà connecté
            if (_currentUser != null && !string.IsNullOrWhiteSpace(_currentToken))
            {
                // Vérifier si le token est valide
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(_currentToken);
                
                if (jwtToken.ValidTo > DateTime.UtcNow)
                {
                    // Token encore valide
                    return new AuthResponse
                    {
                        Success = true,
                        User = _currentUser,
                        Token = _currentToken
                    };
                }
                
                // Token expiré, essayer de rafraîchir
                if (!string.IsNullOrWhiteSpace(_refreshToken))
                {
                    return await RefreshToken();
                }
            }
            else if (!string.IsNullOrWhiteSpace(_refreshToken))
            {
                // Tentative de connexion avec le refresh token
                return await RefreshToken();
            }
            
            // Pas connecté
            return new AuthResponse { Success = false };
        }

        public async Task<AuthResponse> RefreshToken()
        {
            if (string.IsNullOrWhiteSpace(_refreshToken))
            {
                return new AuthResponse { Success = false, Message = "Aucun token de rafraîchissement" };
            }

            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                // Trouver le token
                var tokenEntry = await context.RefreshTokens
                    .Include(rt => rt.UserAccount)
                    .ThenInclude(ua => ua.User)
                    .FirstOrDefaultAsync(rt => rt.Token == _refreshToken && rt.ExpiresAt > DateTime.UtcNow);

                if (tokenEntry == null)
                {
                    // Token invalide ou expiré
                    _refreshToken = null;
                    Preferences.Remove(_refreshTokenPreferenceKey);
                    return new AuthResponse { Success = false, Message = "Session expirée" };
                }

                // Générer de nouveaux tokens
                var userAccount = tokenEntry.UserAccount;
                var tokens = GenerateTokens(userAccount);

                // Supprimer l'ancien token
                context.RefreshTokens.Remove(tokenEntry);
                await context.SaveChangesAsync();

                // Sauvegarder le nouveau refresh token
                SaveRefreshToken(tokens.refreshToken);

                // Mettre à jour les informations utilisateur
                _currentUser = new Models.UserDto
                {
                    Id = userAccount.Id,
                    Username = userAccount.Username,
                    Email = userAccount.Email,
                    FirstName = userAccount.User?.FirstName ?? "",
                    LastName = userAccount.User?.LastName ?? ""
                };

                _currentToken = tokens.token;
                _refreshToken = tokens.refreshToken;

                return new AuthResponse
                {
                    Success = true,
                    Message = "Session renouvelée",
                    Token = tokens.token,
                    RefreshToken = tokens.refreshToken,
                    User = _currentUser
                };
            }
        }

        public async Task Logout()
        {
            // Supprimer le refresh token
            if (!string.IsNullOrWhiteSpace(_refreshToken))
            {
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    var token = await context.RefreshTokens
                        .FirstOrDefaultAsync(rt => rt.Token == _refreshToken);

                    if (token != null)
                    {
                        context.RefreshTokens.Remove(token);
                        await context.SaveChangesAsync();
                    }
                }
            }

            // Supprimer les informations stockées
            _currentUser = null;
            _currentToken = null;
            _refreshToken = null;
            Preferences.Remove(_refreshTokenPreferenceKey);
        }

        private (string token, string refreshToken) GenerateTokens(UserAccount userAccount)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            
            // Créer le JWT token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userAccount.Id.ToString()),
                    new Claim(ClaimTypes.Name, userAccount.Username),
                    new Claim(ClaimTypes.Email, userAccount.Email)
                }),
                Expires = DateTime.UtcNow.Add(_tokenExpiration),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            // Générer un refresh token aléatoire
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            
            // Enregistrer le refresh token dans la base de données
            using (var context = _contextFactory.CreateDbContext())
            {
                var tokenEntry = new RefreshToken
                {
                    Token = refreshToken,
                    UserAccountId = userAccount.Id,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.Add(_refreshTokenExpiration)
                };

                context.RefreshTokens.Add(tokenEntry);
                context.SaveChanges();
            }

            return (jwtToken, refreshToken);
        }

        private void SaveRefreshToken(string refreshToken)
        {
            Preferences.Set(_refreshTokenPreferenceKey, refreshToken);
        }

        private string GenerateSalt()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
        }

        private string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using (var rfc2898 = new Rfc2898DeriveBytes(password, saltBytes, 10000))
            {
                var hashBytes = rfc2898.GetBytes(32);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
} 