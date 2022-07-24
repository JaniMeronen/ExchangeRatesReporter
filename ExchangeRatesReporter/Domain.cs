using System.Collections;

record Conversion(Money From, DateOnly On, Money To);

abstract record Currency
{
    public abstract override string ToString();
}

record Euro : Currency
{
    public override string ToString() => "EUR";
}

record PoundSterling : Currency
{
    public override string ToString() => "GBP";
}

record SwissFranc : Currency
{
    public override string ToString() => "CHF";
}

record USDollar : Currency
{
    public override string ToString() => "USD";
}

record DateRange(DateOnly Start, DateOnly End) : IEnumerable<DateOnly>
{
    public IEnumerator<DateOnly> GetEnumerator()
    {
        for (DateOnly date = Start; date <= End; date = date.AddDays(1))
            yield return date;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

record ExchangeRateRequest(Money From, DateRange On, IEnumerable<Currency> To)
{
    IEnumerable<(DateOnly, Currency)> DatedCurrencies =>
        On.SelectMany(on => To.Select(to => (on, to)));

    public async Task<IEnumerable<Conversion>> FetchUsingAsync(ExchangeRateServer server)
    {
        List<Conversion> conversions = new();

        foreach ((DateOnly on, Currency to) in DatedCurrencies)
            if (await server.TryFetchAsync(From, on, to) is Conversion conversion)
                conversions.Add(conversion);

        return conversions;
    }
}

abstract record ExchangeRateServer
{
    public abstract Task<Conversion?> TryFetchAsync(Money from, DateOnly on, Currency to);
}

record Money(decimal Amount, Currency Currency)
{
    public Money Exchange(decimal rate, Currency to) => new(Amount * rate, to);
}
