namespace backend.src.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Order Order { get; private set; } = null!;
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = null!;
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal LineTotal { get; private set; }

        private OrderItem() { }

        public OrderItem(Guid orderId, Guid productId, decimal unitPrice, int quantity)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            ProductId = productId;
            UnitPrice = ValidateUnitPrice(unitPrice);
            Quantity = ValidateQuantity(quantity);
            LineTotal = unitPrice * quantity;
        }

        private static decimal ValidateUnitPrice(decimal unitPrice)
        {
            if (unitPrice <= 0)
            {
                throw new ArgumentException("O preço unitário do item deve ser maior que zero.", nameof(unitPrice));
            }

            return unitPrice;
        }

        private static int ValidateQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("A quantidade do item deve ser maior que zero.", nameof(quantity));
            }

            return quantity;
        }
    }
}
