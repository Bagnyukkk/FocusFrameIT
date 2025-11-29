namespace FocusFrameAPI.Dtos.Chat
{
  public class ChatThreadDto
  {
    public int ThreadId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; }
    public bool HasUnread { get; set; }
    public DateTime LastMessageAt { get; set; }
  }
}
