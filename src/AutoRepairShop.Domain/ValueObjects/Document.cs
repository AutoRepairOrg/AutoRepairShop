using AutoRepairShop.Domain.Exceptions;

namespace AutoRepairShop.Domain.ValueObjects;

public sealed class Document
{
    public string Value { get; } = string.Empty;

    public Document() { }

    public Document(string value)
    {
        Value = value;
    }

    public static Document Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new DomainException("Document is required");

        var digits = OnlyDigits(input);

        if (digits.Length == 11 && IsValidCpf(digits))
            return new Document(digits);

        if (digits.Length == 14 && IsValidCnpj(digits))
            return new Document(digits);

        throw new DomainException("Invalid CPF or CNPJ");
    }

    private static string OnlyDigits(string value) => new(value.Where(char.IsDigit).ToArray());

    #region CPF

    private static bool IsValidCpf(string cpf)
    {
        if (cpf.Distinct().Count() == 1)
            return false;

        var numbers = cpf.Select(c => int.Parse(c.ToString())).ToArray();

        var sum1 = 0;
        for (int i = 0; i < 9; i++)
            sum1 += numbers[i] * (10 - i);

        var digit1 = (sum1 * 10) % 11;
        if (digit1 == 10)
            digit1 = 0;

        if (numbers[9] != digit1)
            return false;

        var sum2 = 0;
        for (int i = 0; i < 10; i++)
            sum2 += numbers[i] * (11 - i);

        var digit2 = (sum2 * 10) % 11;
        if (digit2 == 10)
            digit2 = 0;

        return numbers[10] == digit2;
    }

    #endregion

    #region CNPJ

    private static bool IsValidCnpj(string cnpj)
    {
        if (cnpj.Distinct().Count() == 1)
            return false;

        int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var numbers = cnpj.Select(c => int.Parse(c.ToString())).ToArray();

        var sum1 = 0;
        for (int i = 0; i < 12; i++)
            sum1 += numbers[i] * multiplier1[i];

        var remainder = sum1 % 11;
        var digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (numbers[12] != digit1)
            return false;

        var sum2 = 0;
        for (int i = 0; i < 13; i++)
            sum2 += numbers[i] * multiplier2[i];

        remainder = sum2 % 11;
        var digit2 = remainder < 2 ? 0 : 11 - remainder;

        return numbers[13] == digit2;
    }

    #endregion

    public override bool Equals(object? obj) => obj is Document other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
