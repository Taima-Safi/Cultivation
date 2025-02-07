using Cultivation.Database.Context;

namespace Cultivation.Repository.Token;

public class TokenRepo : ITokenRepo
{
    private readonly CultivationDbContext context;

    public TokenRepo(CultivationDbContext context)
    {
        this.context = context;
    }

    public async Task<string> AddAsync()
    {

        return " ";
    }
}
