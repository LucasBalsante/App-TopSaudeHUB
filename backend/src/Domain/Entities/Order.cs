using backend.src.Domain.Enums;

namespace backend.src.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public Customer Customer { get; private set; } = null!;
        public decimal TotalAmount { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

        private Order() { }

        public Order(Guid customerId)
        {
            Id = Guid.NewGuid();
            CustomerId = ValidateCustomerId(customerId);
            Status = OrderStatus.CREATED;
            CreatedAt = DateTime.Now;
            TotalAmount = 0;
        }

        public Order(Guid customerId, decimal totalAmount, OrderStatus status)
        {
            Id = Guid.NewGuid();
            CustomerId = ValidateCustomerId(customerId);
            TotalAmount = ValidateTotalAmount(totalAmount);
            Status = status;
            CreatedAt = DateTime.Now;
        }

        public void AddItem(Product product, int quantity)
        {
            ArgumentNullException.ThrowIfNull(product);

            var orderItem = new OrderItem(Id, product.Id, product.Price, quantity);
            OrderItems.Add(orderItem);
            TotalAmount += orderItem.LineTotal;
        }

        public void UpdateCustomer(Guid customerId)
        {
            CustomerId = ValidateCustomerId(customerId);
        }

        public void ClearItems()
        {
            OrderItems.Clear();
            TotalAmount = 0;
        }

        private static Guid ValidateCustomerId(Guid customerId)
        {
            if (customerId == Guid.Empty)
            {
                throw new ArgumentException("O cliente do pedido é obrigatório.", nameof(customerId));
            }

            return customerId;
        }

        private static decimal ValidateTotalAmount(decimal totalAmount)
        {
            if (totalAmount < 0)
            {
                throw new ArgumentException("O valor total do pedido não pode ser negativo.", nameof(totalAmount));
            }

            return totalAmount;
        }
    }
}
