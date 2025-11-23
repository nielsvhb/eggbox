namespace Eggbox.Osc;

public sealed class OscPattern
{
    private readonly string _template;

    public OscPattern(string template)
    {
        _template = template;
    }

    public string Build(params object[] args)
        => string.Format(_template, args);

    public bool Match(string address, out int p1)
    {
        p1 = -1;

        var patternParts = _template.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var addrParts = address.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (patternParts.Length != addrParts.Length)
            return false;

        for (int i = 0; i < patternParts.Length; i++)
        {
            if (patternParts[i].StartsWith("{"))
            {
                if (!int.TryParse(addrParts[i], out p1))
                    return false;
            }
            else if (!string.Equals(patternParts[i], addrParts[i], StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    public bool Match(string address, out int p1, out int p2)
    {
        p1 = -1;
        p2 = -1;

        var patternParts = _template.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var addrParts = address.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (patternParts.Length != addrParts.Length)
            return false;

        int paramIndex = 0;

        for (int i = 0; i < patternParts.Length; i++)
        {
            if (patternParts[i].StartsWith("{"))
            {
                if (!int.TryParse(addrParts[i], out var parsed))
                    return false;

                if (paramIndex == 0) p1 = parsed;
                else p2 = parsed;

                paramIndex++;
            }
            else if (!string.Equals(patternParts[i], addrParts[i], StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    public override string ToString() => _template;
}