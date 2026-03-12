using backend.src.Application.Common.Interfaces;
using backend.src.Application.Common.Models;
using backend.src.Infrastructure.Persistence.Models;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace backend.src.Infrastructure.Services
{
    public sealed class IdempotencyService : IIdempotencyService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<IdempotencyService> _logger;

        public IdempotencyService(AppDbContext context, ILogger<IdempotencyService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IdempotencyCheckResult> ReserveAsync(
            string key,
            string requestPath,
            string requestHash,
            CancellationToken cancellationToken = default)
        {
            var existing = await _context.IdempotencyRequests
                .FirstOrDefaultAsync(
                    x => x.Key == key && x.RequestPath == requestPath,
                    cancellationToken);

            if (existing is not null)
            {
                if (!string.Equals(existing.RequestHash, requestHash, StringComparison.Ordinal))
                {
                    _logger.LogWarning(
                        "Idempotency key reuse with different payload. Key: {Key}, Path: {Path}",
                        key,
                        requestPath);

                    return IdempotencyCheckResult.PayloadMismatch();
                }

                if (existing.Status != IdempotencyStatus.PROCESSING && !string.IsNullOrWhiteSpace(existing.ResponsePayload))
                {
                    return IdempotencyCheckResult.StoredResponse(
                        existing.Id,
                        existing.ResponsePayload,
                        existing.ResponseCode);
                }

                return IdempotencyCheckResult.Processing(existing.Id);
            }

            var request = new IdempotencyRequest
            {
                Id = Guid.NewGuid(),
                Key = key,
                RequestPath = requestPath,
                RequestHash = requestHash,
                Status = IdempotencyStatus.PROCESSING,
                CreatedAt = DateTime.Now
            };

            _context.IdempotencyRequests.Add(request);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return IdempotencyCheckResult.NewRequest(request.Id);
            }
            catch (DbUpdateException)
            {
                var concurrent = await _context.IdempotencyRequests
                    .FirstOrDefaultAsync(
                        x => x.Key == key && x.RequestPath == requestPath,
                        cancellationToken);

                if (concurrent is null)
                    throw;

                if (!string.Equals(concurrent.RequestHash, requestHash, StringComparison.Ordinal))
                    return IdempotencyCheckResult.PayloadMismatch();

                if (concurrent.Status != IdempotencyStatus.PROCESSING && !string.IsNullOrWhiteSpace(concurrent.ResponsePayload))
                {
                    return IdempotencyCheckResult.StoredResponse(
                        concurrent.Id,
                        concurrent.ResponsePayload,
                        concurrent.ResponseCode);
                }

                return IdempotencyCheckResult.Processing(concurrent.Id);
            }
        }

        public async Task CompleteAsync(
            Guid idempotencyRequestId,
            int responseCode,
            string responsePayload,
            Guid? orderId,
            CancellationToken cancellationToken = default)
        {
            var record = await _context.IdempotencyRequests
                .FirstOrDefaultAsync(x => x.Id == idempotencyRequestId, cancellationToken);

            if (record is null)
                return;

            record.Status = IdempotencyStatus.COMPLETED;
            record.ResponseCode = responseCode;
            record.ResponsePayload = responsePayload;
            record.OrderId = orderId;
            record.CompletedAt = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task FailAsync(
            Guid idempotencyRequestId,
            int responseCode,
            string responsePayload,
            CancellationToken cancellationToken = default)
        {
            var record = await _context.IdempotencyRequests
                .FirstOrDefaultAsync(x => x.Id == idempotencyRequestId, cancellationToken);

            if (record is null)
                return;

            record.Status = IdempotencyStatus.FAILED;
            record.ResponseCode = responseCode;
            record.ResponsePayload = responsePayload;
            record.CompletedAt = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
