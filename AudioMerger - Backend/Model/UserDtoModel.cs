namespace AudioMerger___Backend.Model
{
    public class UserDtoModel
    {
        int UserId { get; set; }
        string UserName { get; set; } = string.Empty;
        string UserEmail { get; set; } = string.Empty;
        string Password { get; set; } = string.Empty;
    }
}
