using static System.Decimal;
using static System.String;

record ExchangeRateProxy(Uri BaseUri) : ExchangeRateServer
{
    public override async Task<Conversion?> TryFetchAsync(Money from, DateOnly on, Currency to) =>
        await TryFetchRateAsync(from, on, to) is decimal rate
            ? new(from, on, from.Exchange(rate, to))
            : null;

    Uri CreateUri(Money from, DateOnly on, Currency to) => new
    (
        $"{BaseUri.AbsoluteUri}/" +
        $"{from.Currency}/" +
        $"{to}/" +
        $"{on.Year}/" +
        $"{on.Month}/" +
        $"{on.Day}"
    );

    async Task<string> FetchContentUncheckedAsync(Money from, DateOnly on, Currency to)
    {
        var stream = await GetResponseStreamAsync(from, on, to);
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }

    async Task<Stream> GetResponseStreamAsync(Money from, DateOnly on, Currency to)
    {
        var uri = CreateUri(from, on, to);
        using HttpClient client = new();
        var response = await client.GetAsync(uri);
        return response.Content.ReadAsStream();
    }

    async Task<string> TryFetchContentAsync(Money from, DateOnly on, Currency to)
    {
        try { return await FetchContentUncheckedAsync(from, on, to); }
        catch { return Empty; }
    }

    async Task<decimal?> TryFetchRateAsync(Money from, DateOnly on, Currency to)
    {
        var content = await TryFetchContentAsync(from, on, to);
        return TryParse(content, out decimal rate) ? rate : null;
    }
}
