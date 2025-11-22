namespace Eggbox.Osc;

public sealed class OscPattern
{
    private readonly string _template;
    private readonly string[] _parts;

    public OscPattern(string template)
    {
        _template = template;
        _parts = template.Split('/', StringSplitOptions.RemoveEmptyEntries);
    }

    public bool Match(string address, out int p1)
    {
        p1 = -1;
        var a = address.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (a.Length != _parts.Length) return false;

        for (int i = 0; i < _parts.Length; i++)
        {
            if (_parts[i].StartsWith("{"))
            {
                if (!int.TryParse(a[i], out p1)) return false;
            }
            else if (_parts[i] != a[i]) return false;
        }
        return true;
    }

    public bool Match(string address, out int p1, out int p2)
    {
        p1 = -1; p2 = -1;
        var a = address.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (a.Length != _parts.Length) return false;

        int paramIndex = 0;

        for (int i = 0; i < _parts.Length; i++)
        {
            if (_parts[i].StartsWith("{"))
            {
                if (!int.TryParse(a[i], out int val)) return false;
                if (paramIndex == 0) p1 = val;
                else p2 = val;
                paramIndex++;
            }
            else if (_parts[i] != a[i]) return false;
        }
        return true;
    }

    public override string ToString() => _template;
}