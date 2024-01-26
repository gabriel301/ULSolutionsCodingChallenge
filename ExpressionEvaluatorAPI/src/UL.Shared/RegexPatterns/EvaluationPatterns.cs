namespace UL.Shared.RegexPatterns;
public static class EvaluationPatterns
{
    public static readonly string CONTAINS_ONLY_DIGITS = "(^[0-9]*$)";
    public static readonly string MATCH_ALL_SUM_OR_SUBTRACTION = "([\\+\\-])";
    public static readonly string MATCH_ALL_MULTIPLICATION_OR_DIVISION = "([\\/\\*])";
    public static readonly string FIRST_PART_DIGIT = "^(\\d)*";
    public static readonly string FIRST_PART_SUM_OR_SUBTRACTION = "^([\\+\\-])";
    public static readonly string FIRST_PART_MULTIPLICATION_OR_DIVISION = "^([\\/\\*])";
    public static readonly string ONLY_OPERATORS = "([\\/\\*\\+\\-])";

}
