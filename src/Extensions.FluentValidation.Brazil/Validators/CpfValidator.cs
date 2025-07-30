using System;
using System.Linq;

namespace Extensions.FluentValidation.Brazil.Validators
{
    public static class CpfValidator
    {
        /// <summary>
        /// Valida se o CPF é válido seguindo as regras da Receita Federal
        /// </summary>
        /// <param name="cpf">CPF a ser validado (com ou sem formatação)</param>
        /// <returns>True se válido, False se inválido</returns>
        public static bool IsValid(string? cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            // Remove formatação (pontos, hífens, espaços)
            cpf = CleanCpf(cpf);

            // Verifica se tem exatamente 11 dígitos
            if (cpf.Length != 11 || !cpf.All(char.IsDigit))
                return false;

            // Verifica se não são todos os dígitos iguais (111.111.111-11, etc.)
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Calcula e valida os dígitos verificadores
            return ValidateCheckDigits(cpf);
        }

        /// <summary>
        /// Remove toda formatação do CPF
        /// </summary>
        private static string CleanCpf(string cpf)
        {
            return cpf.Replace(".", "")
                     .Replace("-", "")
                     .Replace(" ", "")
                     .Replace("/", "")
                     .Trim();
        }

        /// <summary>
        /// Valida os dois dígitos verificadores do CPF
        /// </summary>
        private static bool ValidateCheckDigits(string cpf)
        {
            // Primeiro dígito verificador
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (cpf[i] - '0') * (10 - i);
            }

            int remainder = sum % 11;
            int firstDigit = remainder < 2 ? 0 : 11 - remainder;

            if ((cpf[9] - '0') != firstDigit)
                return false;

            // Segundo dígito verificador
            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += (cpf[i] - '0') * (11 - i);
            }

            remainder = sum % 11;
            int secondDigit = remainder < 2 ? 0 : 11 - remainder;

            return (cpf[10] - '0') == secondDigit;
        }

        /// <summary>
        /// Formata um CPF válido no padrão XXX.XXX.XXX-XX
        /// </summary>
        /// <param name="cpf">CPF sem formatação</param>
        /// <returns>CPF formatado ou string vazia se inválido</returns>
        public static string FormatCpf(string? cpf)
        {
            if (!IsValid(cpf))
                return string.Empty;

            cpf = CleanCpf(cpf!);
            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }

        /// <summary>
        /// Método auxiliar para testar manualmente o cálculo
        /// </summary>
        public static (int firstDigit, int secondDigit) CalculateDigits(string cpf)
        {
            cpf = CleanCpf(cpf);

            // Primeiro dígito
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (cpf[i] - '0') * (10 - i);
            }
            int remainder = sum % 11;
            int firstDigit = remainder < 2 ? 0 : 11 - remainder;

            // Segundo dígito
            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += (cpf[i] - '0') * (11 - i);
            }
            remainder = sum % 11;
            int secondDigit = remainder < 2 ? 0 : 11 - remainder;

            return (firstDigit, secondDigit);
        }
    }
}