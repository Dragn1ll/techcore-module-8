using Library.SharedKernel.Enums;
using Library.SharedKernel.Utils;
using Microsoft.Extensions.Logging;

namespace Library.Domain.Services;

public class AuthorService
{
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<AuthorService> _logger;

    public AuthorService(IHttpClientFactory factory, ILogger<AuthorService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<Result<string>> GetCoindeskInfoAsync(CancellationToken cancellationToken = default)
    {
        var client = _factory.CreateClient();

        var url = "https://api.coindesk.com/v1/bpi/currentprice.json";
        try
        {
            using var response = await client.GetAsync(url, cancellationToken);
            
            response.EnsureSuccessStatusCode();
            _logger.LogInformation(
                "Получен ответ от CoinDesk: {StatusCode}", response.StatusCode);

            var result = await response.Content.ReadAsStringAsync(cancellationToken);
                
            return Result<string>.Success(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Ошибка при запросе к CoinDesk");
            return Result<string>.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }
}
