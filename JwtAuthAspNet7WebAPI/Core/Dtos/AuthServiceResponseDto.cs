namespace JwtAuthAspNet7WebAPI.Core.Dtos;

public class AuthServiceResponseDto
{
    #region Constructor
    public AuthServiceResponseDto() { }

    public AuthServiceResponseDto(bool isSucceed, string message)
    {
        IsSucceed = isSucceed;
        Message = message;
    }
    #endregion

    public bool IsSucceed { get; set; }
    public string Message { get; set; }
}
