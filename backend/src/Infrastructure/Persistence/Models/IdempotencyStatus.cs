namespace backend.src.Infrastructure.Persistence.Models
{
    public enum IdempotencyStatus
    {
        PROCESSING,
        COMPLETED,
        FAILED
    }
}
