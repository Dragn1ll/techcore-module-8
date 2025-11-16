using Library.SharedKernel.Utils;

namespace Library.Domain.Abstractions.Services;

/// <summary>
/// Сервис для работы с авторами
/// </summary>
public interface IAuthorService
{
    /// <summary>
    /// Получить информацию из Coindesk
    /// </summary>
    /// <param name="cancellationToken">Токен для остановки выполнения метода</param>
    /// <returns>Информация из Coindesk</returns>
    Task<Result<string>> GetCoindeskInfoAsync(CancellationToken cancellationToken = default);
}