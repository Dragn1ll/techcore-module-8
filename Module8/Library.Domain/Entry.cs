using Library.Domain.Abstractions.Services;
using Library.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Domain;

public static class Entry
{
    public static IServiceCollection AddDomain(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddScoped<IBookService, BookService>();
        serviceCollection.AddSingleton<IReviewService, ReviewService>();
        serviceCollection.AddScoped<IAuthorService, AuthorService>();

        serviceCollection.AddHttpClient<IAuthorService, AuthorService>(c =>
        {
            c.BaseAddress = new Uri("https://api.coindesk.com/");
        });

        return serviceCollection;
    }
}
