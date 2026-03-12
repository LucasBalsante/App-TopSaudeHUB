namespace backend.src.Application.Common.Models
{
    public sealed class IdempotencyCheckResult
    {
        public bool CanProcess { get; init; }
        public bool ShouldReturnStoredResponse { get; init; }
        public bool IsPayloadMismatch { get; init; }
        public bool IsProcessing { get; init; }
        public Guid? IdempotencyRequestId { get; init; }
        public string? StoredResponsePayload { get; init; }
        public int? StoredResponseCode { get; init; }

        public static IdempotencyCheckResult NewRequest(Guid id) => new()
        {
            CanProcess = true,
            IdempotencyRequestId = id
        };

        public static IdempotencyCheckResult StoredResponse(Guid id, string payload, int responseCode) => new()
        {
            ShouldReturnStoredResponse = true,
            IdempotencyRequestId = id,
            StoredResponsePayload = payload,
            StoredResponseCode = responseCode
        };

        public static IdempotencyCheckResult PayloadMismatch() => new()
        {
            IsPayloadMismatch = true
        };

        public static IdempotencyCheckResult Processing(Guid id) => new()
        {
            IsProcessing = true,
            IdempotencyRequestId = id
        };
    }
}
