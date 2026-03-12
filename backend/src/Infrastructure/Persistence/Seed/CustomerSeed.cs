using backend.src.Domain.Entities;

namespace backend.src.Infrastructure.Persistence.Seed
{
    public class CustomerSeed
    {
        public static IReadOnlyCollection<Customer> GetCustomers()
        {
            return new List<Customer>
        {
            new("Ana Souza", "ana.souza@email.com", "12345678901"),
            new("Bruno Lima", "bruno.lima@email.com", "12345678902"),
            new("Carla Mendes", "carla.mendes@email.com", "12345678903"),
            new("Daniel Rocha", "daniel.rocha@email.com", "12345678904"),
            new("Eduarda Martins", "eduarda.martins@email.com", "12345678905"),
            new("Felipe Gomes", "felipe.gomes@email.com", "12345678906"),
            new("Gabriela Alves", "gabriela.alves@email.com", "12345678907"),
            new("Henrique Costa", "henrique.costa@email.com", "12345678908"),
            new("Isabela Fernandes", "isabela.fernandes@email.com", "12345678909"),
            new("João Pedro Ribeiro", "joao.ribeiro@email.com", "12345678910")
        };
        }
    }
}
