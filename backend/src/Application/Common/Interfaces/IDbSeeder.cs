namespace backend.src.Application.Common.Interfaces
{
    public interface IDbSeeder
    {
        Task SeedAsync(CancellationToken cancellationToken = default);
    }
}
