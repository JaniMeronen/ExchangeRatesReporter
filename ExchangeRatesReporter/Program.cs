using static System.Console;

Money from = new(1M, new Euro());

DateOnly end = new(2022, 7, 24);
DateOnly start = new(2022, 7, 17);
DateRange dates = new(start, end);

Currency[] to = { new PoundSterling(), new SwissFranc(), new USDollar() };

ExchangeRateRequest request = new(from, dates, to);
Uri baseUri = new("http://localhost:50000/rate");
ExchangeRateProxy server = new(baseUri);
var conversions = await request.FetchUsingAsync(server);

ReportFormatter formatter = new();
var report = formatter.Format(conversions);
WriteLine(report);
