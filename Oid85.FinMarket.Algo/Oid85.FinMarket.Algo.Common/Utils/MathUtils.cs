namespace Oid85.FinMarket.Algo.Common.Utils;

public static class MathUtils
{
    public static double? DivideNullable(double? arg1, double? arg2)
    {
        if (arg1 is null || arg2 is null) return null;
        if (arg1 == 0.0 || arg2 == 0.0) return 0.0;

        return Math.Round(arg1.Value / arg2.Value, 2);
    }
}