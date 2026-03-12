using backend.src.Application.Common.Models;

namespace backend.src.Application.Common.Interfaces
{
    public interface IIdempotencyService
    {
        Task<IdempotencyCheckResult> ReserveAsync(
            string key,
            string requestPath,
            string requestHash,
            CancellationToken cancellationToken = default);

        Task CompleteAsync(
            Guid idempotencyRequestId,
            int responseCode,
            string responsePayload,
            Guid? orderId,
            CancellationToken cancellationToken = default);

        Task FailAsync(
            Guid idempotencyRequestId,
            int responseCode,
            string responsePayload,
            CancellationToken cancellationToken = default);
    }
}
