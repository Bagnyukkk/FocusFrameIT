namespace FocusFrameAPI.Dtos.Forum
{
  public class CreateTopicDto { public string Title { get; set; } = string.Empty; public string Content { get; set; } = string.Empty; }
  public class CreateCommentDto { public string Content { get; set; } = string.Empty; }
  public class TopicDto
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public DateTime CreatedAt { get; set; }
  }
}
