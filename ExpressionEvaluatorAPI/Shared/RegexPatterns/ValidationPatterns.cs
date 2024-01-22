namespace Shared.RegexPatterns;

public static class ValidationPatterns
{
    public static readonly string INVALID_CHARACTERS = "([^0-9^\\/^\\*^\\+^\\-])";
    public static readonly string CONSECUTIVE_OPERATORS = "([\\/\\*\\+\\-]){2,}";
    public static readonly string STARTS_OR_ENDS_WITH_NON_DIGITS = "^([^0-9])|([^0-9])$";
    public static readonly string CONTAINS_ONLY_DIGITS = "(^[0-9]*$)";
}

