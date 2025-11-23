namespace Eggbox.Models;

public record TrafficLogEntry(
    DateTime Timestamp,
    bool IsTx,
    string Address,
    object[] Arguments,
    bool Handled,
    DateTime? RxTime,
    DateTime? ParseStart,
    DateTime? ParseEnd
);