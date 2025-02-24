namespace Cultivation.Dto.Common;

public class CommonResponseDto<T>
{
    public CommonResponseDto()
    {
    }
    //public CommonResponseDto(T data, string message)
    //{
    //    Data = data;
    //    Message = message;
    //}
    public CommonResponseDto(T data, bool? hasNextPage = null, int? pageSize = null, int? pageNum = null, int? totalRecords = null)
    {
        Data = data;
        PageNum = pageNum;
        PageSize = pageSize;
        HasNextPage = hasNextPage;
        TotalRecords = totalRecords;
    }
    public T Data { get; set; }
    public int? PageSize { get; set; }
    public int? PageNum { get; set; }
    public int? TotalRecords { get; set; }
    public bool? HasNextPage { get; set; }
    public string Message { get; set; }
    public string ErrorMessage { get; set; }
    public object ErrorMessageDetails { get; set; }
}
