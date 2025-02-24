using Cultivation.Dto.Token;

namespace Cultivation.Repository.Token;

public interface ITokenRepo
{
    Task AddAsync(RefreshTokenDto tokenDto);
    Task<TokenDto> CreateAsync(long userId, string userType = null, string oldJwtId = null, bool? userSameToken = null, string oldRefreshToken = null);
    Task<TokenDto> RefreshTokenAsync(string refreshToken);
}
