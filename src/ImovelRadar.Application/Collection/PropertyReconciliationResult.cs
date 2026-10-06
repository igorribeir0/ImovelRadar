namespace ImovelRadar.Application.Collection;

public class PropertyReconciliationResult
{
    public PropertyReconciliationResult(
        int evaluated,
        int markedAsNotObserved)
    {
        Evaluated = evaluated;
        MarkedAsNotObserved =
            markedAsNotObserved;
    }

    public int Evaluated { get; }

    public int MarkedAsNotObserved { get; }
}