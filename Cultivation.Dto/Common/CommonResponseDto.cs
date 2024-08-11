namespace FourthPro.Dto.Common;

public class CommonResponseDto<T>
{
    public CommonResponseDto()
    {
    }
    public CommonResponseDto(T data, string message)
    {
        Data = data;
        Message = message;
    }
    public T Data { get; set; }
    public string Message { get; set; }
    public string ErrorMessage { get; set; }
    public object ErrorMessageDetails { get; set; }
}
