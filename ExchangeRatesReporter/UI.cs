using static System.Environment;
using static System.String;

class ReportFormatter
{
    public string Format(IEnumerable<Conversion> conversions) =>
        Join(NewLine, conversions.Select(FormatReport));

    string FormatReport(Conversion conversion) =>
        $"{conversion.On:yyyy-MM-dd}    " +
        $"{conversion.From.Amount:0.00} " +
        $"{conversion.From.Currency}    " +
        $"{conversion.To.Amount:0.00} " +
        $"{conversion.To.Currency}";
}
