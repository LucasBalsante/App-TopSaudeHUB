using System.Text.Json.Serialization;

namespace backend.src.Infrastructure.Services
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("cod_retorno")]
        public int CodRetorno { get; init; }
        [JsonPropertyName("mensagem")]
        public string? Mensagem { get; init; }
        [JsonPropertyName("data")]
        public T? Data { get; init; }

        public static ApiResponse<T> Success(T data, string? mensagem = null) => new()
        {
            CodRetorno = 0,
            Mensagem = mensagem,
            Data = data
        };

        public static ApiResponse<T> Error(string mensagem) => new()
        {
            CodRetorno = 1,
            Mensagem = mensagem,
            Data = default
        };
    }
}
