using Cultivation.Dto.Client;
using FourthPro.Dto.Common;

namespace Cultivation.Repository.Client;

public interface IClientRepo
{
    Task<long> AddAsync(ClientFormDto dto);
    Task<CommonResponseDto<List<ClientDto>>> GetAllAsync(bool? isLocal, string name, int pageSize, int pageNum);
}
