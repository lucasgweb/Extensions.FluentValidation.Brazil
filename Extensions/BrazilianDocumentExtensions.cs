using FluentValidation;
using Extensions.FluentValidation.Brazil.Validators;

namespace Extensions.FluentValidation.Brazil.Extensions;

public static class BrazilianDocumentExtensions
{
    /// <summary>
    /// Valida se o CPF é válido seguindo as regras da Receita Federal
    /// </summary>
    /// <typeparam name="T">Tipo do objeto sendo validado</typeparam>
    /// <param name="ruleBuilder">Rule builder do FluentValidation</param>
    /// <param name="allowEmpty">Se deve permitir valores nulos/vazios (padrão: false)</param>
    /// <returns>Rule builder configurado</returns>
    public static IRuleBuilderOptions<T, string?> IsValidCPF<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        bool allowEmpty = false)
    {
        if (allowEmpty)
        {
            return ruleBuilder
                .Must(cpf => string.IsNullOrWhiteSpace(cpf) || CpfValidator.IsValid(cpf))
                .WithMessage("'{PropertyName}' deve ser um CPF válido quando informado.");
        }

        return ruleBuilder
            .NotEmpty()
            .WithMessage("'{PropertyName}' é obrigatório.")
            .Must(CpfValidator.IsValid)
            .WithMessage("'{PropertyName}' não é um CPF válido.");
    }

    /// <summary>
    /// Valida CPF obrigatório com mensagem customizada
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IsValidCPF<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        string customMessage)
    {
        return ruleBuilder
            .NotEmpty()
            .Must(CpfValidator.IsValid)
            .WithMessage(customMessage);
    }

    /// <summary>
    /// Valida se o CNPJ é válido seguindo as regras da Receita Federal
    /// </summary>
    /// <typeparam name="T">Tipo do objeto sendo validado</typeparam>
    /// <param name="ruleBuilder">Rule builder do FluentValidation</param>
    /// <param name="allowEmpty">Se deve permitir valores nulos/vazios (padrão: false)</param>
    /// <returns>Rule builder configurado</returns>
    public static IRuleBuilderOptions<T, string?> IsValidCNPJ<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        bool allowEmpty = false)
    {
        if (allowEmpty)
        {
            return ruleBuilder
                .Must(cnpj => string.IsNullOrWhiteSpace(cnpj) || CnpjValidator.IsValid(cnpj))
                .WithMessage("'{PropertyName}' deve ser um CNPJ válido quando informado.");
        }

        return ruleBuilder
            .NotEmpty()
            .WithMessage("'{PropertyName}' é obrigatório.")
            .Must(CnpjValidator.IsValid)
            .WithMessage("'{PropertyName}' não é um CNPJ válido.");
    }

    /// <summary>
    /// Valida CNPJ obrigatório com mensagem customizada
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IsValidCNPJ<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        string customMessage)
    {
        return ruleBuilder
            .NotEmpty()
            .Must(CnpjValidator.IsValid)
            .WithMessage(customMessage);
    }

    /// <summary>
    /// Valida CPF ou CNPJ automaticamente baseado no tamanho
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IsValidCpfOrCnpj<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        bool allowEmpty = false)
    {
        return ruleBuilder
            .Must(document =>
            {
                if (allowEmpty && string.IsNullOrWhiteSpace(document))
                    return true;

                if (string.IsNullOrWhiteSpace(document))
                    return false;

                var clean = CleanDocument(document);

                return clean.Length switch
                {
                    11 => CpfValidator.IsValid(document),
                    14 => CnpjValidator.IsValid(document),
                    _ => false
                };
            })
            .WithMessage("'{PropertyName}' deve ser um CPF ou CNPJ válido.");
    }

    /// <summary>
    /// Método centralizado para limpeza de documentos
    /// </summary>
    private static string CleanDocument(string document)
    {
        return document.Replace(".", "")
                      .Replace("-", "")
                      .Replace("/", "")
                      .Replace(" ", "")
                      .Trim();
    }

    /// <summary>
    /// Sobrecarga com mensagem customizada para CPF/CNPJ
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IsValidCpfOrCnpj<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        string customMessage)
    {
        return ruleBuilder
            .Must(document =>
            {
                if (string.IsNullOrWhiteSpace(document))
                    return false;

                var clean = CleanDocument(document);

                return clean.Length switch
                {
                    11 => CpfValidator.IsValid(document),
                    14 => CnpjValidator.IsValid(document),
                    _ => false
                };
            })
            .WithMessage(customMessage);
    }
}