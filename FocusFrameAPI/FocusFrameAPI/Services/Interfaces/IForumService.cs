using FocusFrameAPI.Dtos.Forum;
using FocusFrameAPI.Enums;

namespace FocusFrameAPI.Services.Interfaces
{
  public interface IForumService
  {
    Task<IEnumerable<TopicDto>> GetTopicsAsync(int userId, string filter);
    Task<TopicDto> CreateTopicAsync(int userId, CreateTopicDto dto);
    Task AddCommentAsync(int userId, int topicId, CreateCommentDto dto);
    Task ToggleLikeAsync(int topicId);
    Task DeleteTopicAsync(int userId, UserRole role, int topicId);
  }
}
