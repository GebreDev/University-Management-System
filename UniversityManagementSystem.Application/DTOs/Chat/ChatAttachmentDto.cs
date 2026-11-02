namespace UniversityManagementSystem.Application.DTOs.Chat
{
    public class ChatAttachmentDto
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; } // image, pdf, word, etc.
    }
}