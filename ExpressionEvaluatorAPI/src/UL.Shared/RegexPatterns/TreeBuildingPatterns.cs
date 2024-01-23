namespace UL.Shared.RegexPatterns;
public static class TreeBuildingPatterns
{
    public static readonly string CONTAINS_ONLY_DIGITS = "(^[0-9]*$)";
    public static readonly string CONTAINS_ONLY_SUM_OR_SUBTRACTION = "([\\+\\-])";
    public static readonly string CONTAINS_ONLY_MULTIPLICATION_OR_DIVISION = "([\\/\\*])";
}
