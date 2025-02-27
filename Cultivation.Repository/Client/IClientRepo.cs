using Cultivation.Database.Model;
using Cultivation.Dto.Client;
using Cultivation.Dto.Common;
using Microsoft.AspNetCore.Http;

namespace Cultivation.Repository.Client;

public interface IClientRepo
{
    Task<long> AddAsync(ClientFormDto dto);
    Task<bool> CheckIfExistAsync(long id);
    Task<CommonResponseDto<List<ClientDto>>> GetAllAsync(bool? isLocal, string name, int pageSize, int pageNum);
    Task<FileModel> GetImage(int id);
    Task RemoveAsync(long id);
    Task UpdateAsync(long id, ClientFormDto dto);
    Task<long> UploadImage(IFormFile file);
}
