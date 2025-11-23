## Socket Exhaustion — это проблема, возникающая при неправильном использовании HttpClient в .NET.
### В чём суть проблемы?
Если создавать новый HttpClient в блоке using или вручную для каждого HTTP-запроса, то каждый такой клиент создаёт 
новый сокет соединения с сервером. Даже после вызова Dispose сокет не закрывается мгновенно, а находится в состоянии 
TIME_WAIT для корректного завершения TCP-сессии. При большом числе таких открытых сокетов, система исчерпывает лимит 
портов, что приводит к ошибкам соединения.

### Почему using HttpClient — анти-паттерн?
Dispose у HttpClient не освобождает сокет сразу. Повторное создание HttpClient приводит к накоплению "зависших" сокетов 
и исчерпанию доступных.

### Как HttpClientFactory решает проблему?
* HttpClientFactory управляет пулом HttpMessageHandler.
* При создании HttpClient отдаёт "новый" клиент, но он использует переиспользуемый обработчик соединений. Это сохраняет 
и переиспользует открытые TCP-сессии.
* Позволяет безопасно генерировать множество HttpClient, без утечки ресурсов.
* Позволяет настраивать отдельные HttpClient под различные имена ("_named clients_").

### Пример использования HttpClientFactory:
```csharp
public class MyService 
{
    private readonly HttpClient _client;

    public MyService(IHttpClientFactory factory) 
    {
        _client = factory.CreateClient("coinDesk");
    }

    public async Task<string> GetPriceAsync()
    {
        var response = await _client.GetAsync("https://api.coindesk.com/v1/bpi/currentprice.json");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
```