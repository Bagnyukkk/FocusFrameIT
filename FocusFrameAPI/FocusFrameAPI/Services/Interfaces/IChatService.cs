using FocusFrameAPI.Dtos.Chat;
using FocusFrameAPI.Enums;

namespace FocusFrameAPI.Services.Interfaces
{
  public interface IChatService
  {
    Task SendMessageAsync(int senderId, UserRole senderRole, SendMessageDto dto);
    Task<IEnumerable<ChatMessageDto>> GetThreadMessagesAsync(int requestorId, UserRole requestorRole, int? targetStudentId = null);
    Task<IEnumerable<ChatThreadDto>> GetAdminThreadsAsync();
  }
}
