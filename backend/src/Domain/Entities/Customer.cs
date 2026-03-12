using System.Net.Mail;

namespace backend.src.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Document { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public ICollection<Order> Orders { get; private set; } = new List<Order>();

        private Customer() { }

        public Customer(string name, string email, string document)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            Update(name, email, document);
        }

        public void Update(string name, string email, string document)
        {
            Name = NormalizeName(name);
            Email = NormalizeEmail(email);
            Document = NormalizeDocument(document);
        }

        private static string NormalizeName(string name)
        {
            var normalizedName = name?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                throw new ArgumentException("O nome do cliente é obrigatório.", nameof(name));
            }

            if (normalizedName.Length > 60)
            {
                throw new ArgumentException("O nome do cliente deve ter no máximo 60 caracteres.", nameof(name));
            }

            return normalizedName;
        }

        private static string NormalizeEmail(string email)
        {
            var normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(normalizedEmail))
            {
                throw new ArgumentException("O e-mail do cliente é obrigatório.", nameof(email));
            }

            if (normalizedEmail.Length > 100)
            {
                throw new ArgumentException("O e-mail do cliente deve ter no máximo 100 caracteres.", nameof(email));
            }

            try
            {
                _ = new MailAddress(normalizedEmail);
            }
            catch (FormatException)
            {
                throw new ArgumentException("O e-mail do cliente é inválido.", nameof(email));
            }

            return normalizedEmail;
        }

        private static string NormalizeDocument(string document)
        {
            var normalizedDocument = document?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(normalizedDocument))
            {
                throw new ArgumentException("O documento do cliente é obrigatório.", nameof(document));
            }

            if (normalizedDocument.Length > 20)
            {
                throw new ArgumentException("O documento do cliente deve ter no máximo 20 caracteres.", nameof(document));
            }

            return normalizedDocument;
        }
    }
}
