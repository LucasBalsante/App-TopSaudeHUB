using backend.src.Domain.Entities;

namespace backend.src.Infrastructure.Persistence.Seed
{
    public class ProductSeed
    {
        public static IReadOnlyCollection<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product("Dipirona 500mg", "SKU-001", 12.50m, 5),
                new Product("Paracetamol 750mg", "SKU-002", 15.90m, 9),
                new Product("Ibuprofeno 600mg", "SKU-003", 18.75m, 70),
                new Product("Amoxicilina 500mg", "SKU-004", 32.40m, 45),
                new Product("Azitromicina 500mg", "SKU-005", 41.20m, 3),
                new Product("Losartana 50mg", "SKU-006", 22.10m, 85),
                new Product("Omeprazol 20mg", "SKU-007", 19.99m, 100, false),
                new Product("Vitamina C 1g", "SKU-008", 24.30m, 60),
                new Product("Vitamina D 2000UI", "SKU-009", 29.90m, 55),
                new Product("Cloridrato de Sertralina 50mg", "SKU-010", 37.80m, 42, false),
                new Product("Clonazepam 2mg", "SKU-011", 16.40m, 5),
                new Product("Metformina 850mg", "SKU-012", 14.25m, 1),
                new Product("Atenolol 25mg", "SKU-013", 13.80m, 80),
                new Product("Hidroclorotiazida 25mg", "SKU-014", 11.60m, 75),
                new Product("Sinvastatina 20mg", "SKU-015", 20.45m, 5),
                new Product("Prednisona 20mg", "SKU-016", 17.90m, 48),
                new Product("Nimesulida 100mg", "SKU-017", 13.50m, 8),
                new Product("Loratadina 10mg", "SKU-018", 9.99m, 95),
                new Product("Cetoconazol Creme", "SKU-019", 21.70m, 12),
                new Product("Neosoro Spray", "SKU-020", 18.20m, 40)
            };
        }
    }
}
