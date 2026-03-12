namespace backend.src.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Sku { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int StockQty { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

        private Product() { }

        public Product(string name, string sku, decimal price, int stockQty, bool isActive = true)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            Sku = NormalizeSku(sku);
            Update(name, price, stockQty, isActive);
        }

        public void Update(string name, decimal price, int stockQty, bool isActive)
        {
            Name = NormalizeName(name);
            Price = ValidatePrice(price);
            StockQty = ValidateStockQty(stockQty);
            IsActive = isActive;
        }

        public void DecreaseStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("A quantidade para baixa de estoque deve ser maior que zero.", nameof(quantity));
            }

            if (!IsActive)
            {
                throw new ArgumentException($"O produto {Sku} está inativo para venda.", nameof(quantity));
            }

            if (StockQty < quantity)
            {
                throw new ArgumentException($"Estoque insuficiente para o produto {Sku}.", nameof(quantity));
            }

            StockQty -= quantity;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("A quantidade para reposição de estoque deve ser maior que zero.", nameof(quantity));
            }

            StockQty += quantity;
        }

        private static string NormalizeName(string name)
        {
            var normalizedName = name?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                throw new ArgumentException("O nome do produto é obrigatório.", nameof(name));
            }

            if (normalizedName.Length > 100)
            {
                throw new ArgumentException("O nome do produto deve ter no máximo 100 caracteres.", nameof(name));
            }

            return normalizedName;
        }

        private static string NormalizeSku(string sku)
        {
            var normalizedSku = sku?.Trim().ToUpperInvariant() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(normalizedSku))
            {
                throw new ArgumentException("O SKU do produto é obrigatório.", nameof(sku));
            }

            if (normalizedSku.Length > 50)
            {
                throw new ArgumentException("O SKU do produto deve ter no máximo 50 caracteres.", nameof(sku));
            }

            return normalizedSku;
        }

        private static decimal ValidatePrice(decimal price)
        {
            if (price <= 0)
            {
                throw new ArgumentException("O preço do produto deve ser maior que zero.", nameof(price));
            }

            return price;
        }

        private static int ValidateStockQty(int stockQty)
        {
            if (stockQty < 0)
            {
                throw new ArgumentException("O estoque do produto não pode ser negativo.", nameof(stockQty));
            }

            return stockQty;
        }
    }
}
