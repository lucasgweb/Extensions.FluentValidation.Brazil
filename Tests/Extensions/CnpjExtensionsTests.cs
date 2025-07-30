using Extensions.FluentValidation.Brazil.Extensions;
using FluentValidation;
using System.Linq;
using Xunit;

namespace Extensions.FluentValidation.Brazil.Tests.Extensions;

public class CnpjTestModel
{
    public string? CNPJ { get; set; }
    public string? OptionalCNPJ { get; set; }
    public string? CustomMessageCNPJ { get; set; }
    public string? Document { get; set; }
}

public class CnpjTestModelValidator : AbstractValidator<CnpjTestModel>
{
    public CnpjTestModelValidator()
    {
        RuleFor(x => x.CNPJ).IsValidCNPJ();
        RuleFor(x => x.OptionalCNPJ).IsValidCNPJ(allowEmpty: true);
        RuleFor(x => x.CustomMessageCNPJ).IsValidCNPJ("Mensagem personalizada para CNPJ inválido");
        RuleFor(x => x.Document).IsValidCpfOrCnpj();
    }
}

public class CnpjOnlyValidator : AbstractValidator<CnpjTestModel>
{
    public CnpjOnlyValidator()
    {
        RuleFor(x => x.CNPJ).IsValidCNPJ();
    }
}

public class OptionalCnpjOnlyValidator : AbstractValidator<CnpjTestModel>
{
    public OptionalCnpjOnlyValidator()
    {
        RuleFor(x => x.OptionalCNPJ).IsValidCNPJ(allowEmpty: true);
    }
}

public class CustomMessageCnpjOnlyValidator : AbstractValidator<CnpjTestModel>
{
    public CustomMessageCnpjOnlyValidator()
    {
        RuleFor(x => x.CustomMessageCNPJ).IsValidCNPJ("Mensagem personalizada para CNPJ inválido");
    }
}

public class DocumentOnlyValidator : AbstractValidator<CnpjTestModel>
{
    public DocumentOnlyValidator()
    {
        RuleFor(x => x.Document).IsValidCpfOrCnpj();
    }
}

public class CnpjExtensionsTests
{
    private readonly CnpjTestModelValidator _validator = new();

    [Theory]
    [InlineData("11.222.333/0001-81")]
    [InlineData("11222333000181")]
    [InlineData("11 222 333 0001 81")]
    public void IsValidCNPJ_ValidCnpjs_ShouldNotHaveValidationError(string cnpj)
    {
        // Arrange
        var specificValidator = new CnpjOnlyValidator();
        var model = new CnpjTestModel { CNPJ = cnpj };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        Assert.True(result.IsValid, $"CNPJ '{cnpj}' deveria ser válido. Erros: {string.Join(", ", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"))}");
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == nameof(CnpjTestModel.CNPJ));
    }

    [Theory]
    [InlineData("11.222.333/0001-00")]
    [InlineData("11.111.111/1111-11")]
    [InlineData("12345678000100")]
    [InlineData("")]
    [InlineData("   ")]
    public void IsValidCNPJ_InvalidCnpjs_ShouldHaveValidationError(string cnpj)
    {
        // Arrange
        var specificValidator = new CnpjOnlyValidator();
        var model = new CnpjTestModel { CNPJ = cnpj };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        Assert.False(result.IsValid, $"CNPJ '{cnpj}' deveria ser inválido");
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CnpjTestModel.CNPJ));
    }

    [Fact]
    public void IsValidCNPJ_NullCnpj_ShouldHaveValidationError()
    {
        // Arrange
        var specificValidator = new CnpjOnlyValidator();
        var model = new CnpjTestModel { CNPJ = null };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CnpjTestModel.CNPJ));
    }

    [Theory]
    [InlineData("111.444.777-35")]
    [InlineData("11.222.333/0001-81")]
    public void IsValidCpfOrCnpj_ValidDocuments_ShouldNotHaveValidationError(string document)
    {
        // Arrange
        var specificValidator = new DocumentOnlyValidator();
        var model = new CnpjTestModel { Document = document };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        Assert.True(result.IsValid, $"Documento '{document}' deveria ser válido. Erros: {string.Join(", ", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"))}");
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == nameof(CnpjTestModel.Document));
    }

    [Theory]
    [InlineData("123.456.789-00")]
    [InlineData("11.222.333/0001-00")]
    [InlineData("12345")]
    public void IsValidCpfOrCnpj_InvalidDocuments_ShouldHaveValidationError(string document)
    {
        // Arrange
        var specificValidator = new DocumentOnlyValidator();
        var model = new CnpjTestModel { Document = document };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        Assert.False(result.IsValid, $"Documento '{document}' deveria ser inválido");
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CnpjTestModel.Document));
    }

    [Theory]
    [InlineData("11.222.333/0001-81")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void IsValidCNPJ_WithAllowEmpty_ValidOrEmpty_ShouldNotHaveValidationError(string? cnpj)
    {
        // Arrange
        var specificValidator = new OptionalCnpjOnlyValidator();
        var model = new CnpjTestModel { OptionalCNPJ = cnpj };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        var hasError = result.Errors.Any(e => e.PropertyName == nameof(CnpjTestModel.OptionalCNPJ));
        Assert.False(hasError, $"CNPJ opcional '{cnpj ?? "null"}' não deveria ter erro de validação. Erros: {string.Join(", ", result.Errors.Where(e => e.PropertyName == nameof(CnpjTestModel.OptionalCNPJ)).Select(e => e.ErrorMessage))}");
    }

    [Theory]
    [InlineData("11.222.333/0001-00")]
    [InlineData("11.111.111/1111-11")]
    public void IsValidCNPJ_WithAllowEmpty_InvalidCnpj_ShouldHaveValidationError(string cnpj)
    {
        // Arrange
        var specificValidator = new OptionalCnpjOnlyValidator();
        var model = new CnpjTestModel { OptionalCNPJ = cnpj };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CnpjTestModel.OptionalCNPJ));
    }

    [Fact]
    public void IsValidCNPJ_WithCustomMessage_InvalidCnpj_ShouldHaveCustomErrorMessage()
    {
        // Arrange
        var specificValidator = new CustomMessageCnpjOnlyValidator();
        var model = new CnpjTestModel { CustomMessageCNPJ = "11.222.333/0001-00" };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        var error = result.Errors.FirstOrDefault(e => e.PropertyName == nameof(CnpjTestModel.CustomMessageCNPJ));
        Assert.NotNull(error);
        Assert.Equal("Mensagem personalizada para CNPJ inválido", error.ErrorMessage);
    }

    [Fact]
    public void IsValidCNPJ_DefaultMessage_InvalidCnpj_ShouldHaveDefaultErrorMessage()
    {
        // Arrange
        var specificValidator = new CnpjOnlyValidator();
        var model = new CnpjTestModel { CNPJ = "11.222.333/0001-00" };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        var error = result.Errors.FirstOrDefault(e => e.PropertyName == nameof(CnpjTestModel.CNPJ));
        Assert.NotNull(error);
        Assert.Equal("'CNPJ' não é um CNPJ válido.", error.ErrorMessage);
    }

    [Fact]
    public void IsValidCpfOrCnpj_DefaultMessage_InvalidDocument_ShouldHaveDefaultErrorMessage()
    {
        // Arrange
        var specificValidator = new DocumentOnlyValidator();
        var model = new CnpjTestModel { Document = "12345" };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        var error = result.Errors.FirstOrDefault(e => e.PropertyName == nameof(CnpjTestModel.Document));
        Assert.NotNull(error);
        Assert.Equal("'Document' deve ser um CPF ou CNPJ válido.", error.ErrorMessage);
    }
}