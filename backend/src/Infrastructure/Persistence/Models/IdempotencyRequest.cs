namespace backend.src.Infrastructure.Persistence.Models
{
    public sealed class IdempotencyRequest
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string RequestPath { get; set; } = string.Empty;
        public string RequestHash { get; set; } = string.Empty;
        public IdempotencyStatus Status { get; set; }
        public int ResponseCode { get; set; }
        public string? ResponsePayload { get; set; }
        public Guid? OrderId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
    }
}
