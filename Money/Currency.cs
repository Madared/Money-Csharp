namespace Money;

public abstract class Currency : IEquatable<Currency>
{
    public abstract string Name { get; }
    public abstract string Symbol { get; }
    public abstract string Representation(decimal value);

    public override int GetHashCode() => HashCode.Combine(Name, Symbol);
    public override bool Equals(object? obj) => Equals(obj as Currency);
    public bool Equals(Currency? currency)
    {
        if (currency is null)
        {
            return false;
        }
        if (Object.ReferenceEquals(this, currency))
        {
            return true;
        }
        return (Name == currency.Name) && (Symbol == currency.Symbol);
    }

    public static bool operator == (Currency a, Currency b) 
    {
        if (a is null)
        {
            if (b is null)
                return true;

            return false;
        }
        return a.Equals(b);
    }

    public static bool operator != (Currency a, Currency b) => !(a == b);
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
