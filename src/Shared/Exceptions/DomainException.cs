using Shared.Enums;

namespace Shared.Exceptions
{
    /// <summary>
    /// Exception customizada que carrega informações sobre o tipo de erro customizado
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Tipo de erro customizado
        /// </summary>
        public ErrorType ErrorType { get; }

        /// <summary>
        /// Construtor com valores padrão
        /// </summary>
        /// <param name="message">Mensagem de erro (padrão: "Invalid input")</param>
        /// <param name="errorType">Tipo de erro customizado (padrão: InvalidInput)</param>
        public DomainException(string message = "Invalid input", ErrorType errorType = ErrorType.InvalidInput) 
            : base(message)
        {
            ErrorType = errorType;
        }
    }
}
