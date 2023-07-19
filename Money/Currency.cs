namespace Money;

public abstract class Currency
{
    public abstract string Name { get; }
    public abstract string Symbol { get; }
    public abstract string Representation(decimal value);
}

public class CurrencyFactory
{
    Dollar dollar = new();
    Euro euro = new();
    EscudoCV Cve = new();

    public Dollar GetDollar() => dollar;
    public Euro GetEuro() => euro;
    public EscudoCV GetCve() => Cve;
}

public class Dollar : Currency
{
    private string _name = "Dollar";
    private string _symbol = "$";
    public override string Name => _name;
    public override string Symbol => _symbol;

    public override string Representation(decimal value)
    {
        string[] splitTotal = value
            .ToString()
            .Split(".");
        string beforeDot = splitTotal.ElementAtOrDefault(0) ?? "0";
        string afterDot = splitTotal.ElementAtOrDefault(1) ?? "00";
        return beforeDot + "$" + afterDot;
    }
}

public class Euro : Currency
{
    private string _name = "Euro";
    private string _symbol = "€";
    public override string Name => _name;
    public override string Symbol => _symbol;

    public override string Representation(decimal value)
    {
        string[] splitTotal = value
            .ToString()
            .Split(".");
        string beforeDot = splitTotal.ElementAtOrDefault(0) ?? "0";
        string afterDot = splitTotal.ElementAtOrDefault(1) ?? "00";
        return beforeDot + _symbol + afterDot;
    }
}

public class EscudoCV : Currency
{
    private string _name = "Escudo Cabo Verdeano";
    private string _symbol = "$";

    public override string Name => _name;
    public override string Symbol => _symbol;


    public override string Representation(decimal value)
    {
        string[] splitTotal = Math.Round(value, MidpointRounding.AwayFromZero)
            .ToString()
            .Split(".");
        string beforeDot = splitTotal.ElementAtOrDefault(0) ?? "0";
        return beforeDot + _symbol + "00";
    }
}
