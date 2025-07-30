using System;
using System.Linq;

namespace Extensions.FluentValidation.Brazil.Validators;

public static class CnpjValidator
{
    /// <summary>
    /// Valida se o CNPJ é válido seguindo as regras da Receita Federal
    /// </summary>
    /// <param name="cnpj">CNPJ a ser validado (com ou sem formatação)</param>
    /// <returns>True se válido, False se inválido</returns>
    public static bool IsValid(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        cnpj = CleanCnpj(cnpj);

        if (cnpj.Length != 14 || !cnpj.All(char.IsDigit))
            return false;

        if (cnpj.All(c => c == cnpj[0]))
            return false;

        return ValidateCheckDigits(cnpj);
    }

    /// <summary>
    /// Remove toda formatação do CNPJ
    /// </summary>
    private static string CleanCnpj(string cnpj)
    {
        return cnpj.Replace(".", "")
                  .Replace("-", "")
                  .Replace("/", "")
                  .Replace(" ", "")
                  .Trim();
    }

    /// <summary>
    /// Valida os dois dígitos verificadores do CNPJ
    /// </summary>
    private static bool ValidateCheckDigits(string cnpj)
    {
        int[] multipliers1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int sum = 0;

        for (int i = 0; i < 12; i++)
        {
            sum += (cnpj[i] - '0') * multipliers1[i];
        }

        int remainder = sum % 11;
        int firstDigit = remainder < 2 ? 0 : 11 - remainder;

        if ((cnpj[12] - '0') != firstDigit)
            return false;

        int[] multipliers2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        sum = 0;

        for (int i = 0; i < 13; i++)
        {
            sum += (cnpj[i] - '0') * multipliers2[i];
        }

        remainder = sum % 11;
        int secondDigit = remainder < 2 ? 0 : 11 - remainder;

        return (cnpj[13] - '0') == secondDigit;
    }

    /// <summary>
    /// Formata um CNPJ válido no padrão XX.XXX.XXX/XXXX-XX
    /// </summary>
    /// <param name="cnpj">CNPJ sem formatação</param>
    /// <returns>CNPJ formatado ou string vazia se inválido</returns>
    public static string FormatCnpj(string? cnpj)
    {
        if (!IsValid(cnpj))
            return string.Empty;

        cnpj = CleanCnpj(cnpj!);
        return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
    }

    /// <summary>
    /// Método auxiliar para testar manualmente o cálculo
    /// </summary>
    public static (int firstDigit, int secondDigit) CalculateDigits(string cnpj)
    {
        cnpj = CleanCnpj(cnpj);

        int[] multipliers1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int sum = 0;
        for (int i = 0; i < 12; i++)
        {
            sum += (cnpj[i] - '0') * multipliers1[i];
        }
        int remainder = sum % 11;
        int firstDigit = remainder < 2 ? 0 : 11 - remainder;

        int[] multipliers2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        sum = 0;
        for (int i = 0; i < 13; i++)
        {
            sum += (cnpj[i] - '0') * multipliers2[i];
        }
        remainder = sum % 11;
        int secondDigit = remainder < 2 ? 0 : 11 - remainder;

        return (firstDigit, secondDigit);
    }
}