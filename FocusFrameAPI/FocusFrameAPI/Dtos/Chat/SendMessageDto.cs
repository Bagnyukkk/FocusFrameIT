namespace FocusFrameAPI.Dtos.Chat
{
  public class SendMessageDto
  {
    public string MessageText { get; set; } = string.Empty;
    public int? TargetUserId { get; set; }
  }
}
