using Cultivation.Dto.Client;
using FourthPro.Dto.Common;

namespace Cultivation.Repository.Client;

public interface IClientRepo
{
    Task<long> AddAsync(ClientFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<ClientDto>>> GetAllAsync(bool? isLocal, string name, int pageSize, int pageNum);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, string phone, string codePhone, string name);
}
