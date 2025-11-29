namespace FocusFrameAPI.Dtos.Chat
{
  public class ChatMessageDto
  {
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; }
    public string MessageText { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
  }
}
