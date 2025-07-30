using Extensions.FluentValidation.Brazil.Extensions;
using FluentValidation;
using System.Linq;
using Xunit;

namespace Extensions.FluentValidation.Brazil.Tests.Extensions;

public class CpfTestModel
{
    public string? CPF { get; set; }
    public string? OptionalCPF { get; set; }
    public string? CustomMessageCPF { get; set; }
}

public class CpfTestModelValidator : AbstractValidator<CpfTestModel>
{
    public CpfTestModelValidator()
    {
        RuleFor(x => x.CPF).IsValidCPF();
        RuleFor(x => x.OptionalCPF).IsValidCPF(allowEmpty: true);
        RuleFor(x => x.CustomMessageCPF).IsValidCPF("Mensagem personalizada para CPF inválido");
    }
}

public class CpfOnlyValidator : AbstractValidator<CpfTestModel>
{
    public CpfOnlyValidator()
    {
        RuleFor(x => x.CPF).IsValidCPF();
    }
}

public class OptionalCpfOnlyValidator : AbstractValidator<CpfTestModel>
{
    public OptionalCpfOnlyValidator()
    {
        RuleFor(x => x.OptionalCPF).IsValidCPF(allowEmpty: true);
    }
}

public class CustomMessageCpfOnlyValidator : AbstractValidator<CpfTestModel>
{
    public CustomMessageCpfOnlyValidator()
    {
        RuleFor(x => x.CustomMessageCPF).IsValidCPF("Mensagem personalizada para CPF inválido");
    }
}

public class CpfExtensionsTests
{
    [Theory]
    [InlineData("111.444.777-35")]
    [InlineData("11144477735")]
    [InlineData("111 444 777 35")]
    public void IsValidCPF_ValidCpfs_ShouldNotHaveValidationError(string cpf)
    {
        // Arrange
        var specificValidator = new CpfOnlyValidator();
        var model = new CpfTestModel { CPF = cpf };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        Assert.True(result.IsValid, $"CPF '{cpf}' deveria ser válido. Erros: {string.Join(", ", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"))}");
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == nameof(CpfTestModel.CPF));
    }

    [Theory]
    [InlineData("123.456.789-00")]
    [InlineData("111.111.111-11")]
    [InlineData("12345678901")]
    [InlineData("")]
    [InlineData("   ")]
    public void IsValidCPF_InvalidCpfs_ShouldHaveValidationError(string cpf)
    {
        // Arrange
        var specificValidator = new CpfOnlyValidator();
        var model = new CpfTestModel { CPF = cpf };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        Assert.False(result.IsValid, $"CPF '{cpf}' deveria ser inválido");
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CpfTestModel.CPF));
    }

    [Fact]
    public void IsValidCPF_NullCpf_ShouldHaveValidationError()
    {
        // Arrange
        var specificValidator = new CpfOnlyValidator();
        var model = new CpfTestModel { CPF = null };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CpfTestModel.CPF));
    }

    [Theory]
    [InlineData("111.444.777-35")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void IsValidCPF_WithAllowEmpty_ValidOrEmpty_ShouldNotHaveValidationError(string? cpf)
    {
        // Arrange
        var specificValidator = new OptionalCpfOnlyValidator();
        var model = new CpfTestModel { OptionalCPF = cpf };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        var hasError = result.Errors.Any(e => e.PropertyName == nameof(CpfTestModel.OptionalCPF));
        Assert.False(hasError, $"CPF opcional '{cpf ?? "null"}' não deveria ter erro de validação. Erros: {string.Join(", ", result.Errors.Where(e => e.PropertyName == nameof(CpfTestModel.OptionalCPF)).Select(e => e.ErrorMessage))}");
    }

    [Theory]
    [InlineData("123.456.789-00")]
    [InlineData("111.111.111-11")]
    public void IsValidCPF_WithAllowEmpty_InvalidCpf_ShouldHaveValidationError(string cpf)
    {
        // Arrange
        var specificValidator = new OptionalCpfOnlyValidator();
        var model = new CpfTestModel { OptionalCPF = cpf };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CpfTestModel.OptionalCPF));
    }

    [Fact]
    public void IsValidCPF_WithCustomMessage_InvalidCpf_ShouldHaveCustomErrorMessage()
    {
        // Arrange
        var specificValidator = new CustomMessageCpfOnlyValidator();
        var model = new CpfTestModel { CustomMessageCPF = "123.456.789-00" };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        var error = result.Errors.FirstOrDefault(e => e.PropertyName == nameof(CpfTestModel.CustomMessageCPF));
        Assert.NotNull(error);
        Assert.Equal("Mensagem personalizada para CPF inválido", error.ErrorMessage);
    }

    [Fact]
    public void IsValidCPF_DefaultMessage_InvalidCpf_ShouldHaveDefaultErrorMessage()
    {
        // Arrange
        var specificValidator = new CpfOnlyValidator();
        var model = new CpfTestModel { CPF = "123.456.789-00" };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        var error = result.Errors.FirstOrDefault(e => e.PropertyName == nameof(CpfTestModel.CPF));
        Assert.NotNull(error);
        Assert.Equal("'CPF' não é um CPF válido.", error.ErrorMessage);
    }

    [Theory]
    [InlineData("111.444.777-35", true)]
    [InlineData("11144477735", true)]
    [InlineData("111 444 777 35", true)]
    [InlineData("123.456.789-00", false)]
    [InlineData("111.111.111-11", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidCPF_VariousInputs_ShouldReturnExpectedResults(string? cpf, bool expectedValid)
    {
        // Arrange
        var specificValidator = new CpfOnlyValidator();
        var model = new CpfTestModel { CPF = cpf };

        // Act
        var result = specificValidator.Validate(model);

        // Assert
        Assert.Equal(expectedValid, result.IsValid);
    }

    [Fact]
    public void IsValidCPF_MultipleCpfValidation_ShouldWorkCorrectly()
    {
        // Arrange
        var validator = new CpfTestModelValidator();
        var model = new CpfTestModel
        {
            CPF = "111.444.777-35",           
            OptionalCPF = "",               
            CustomMessageCPF = "111.444.777-35"
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void IsValidCPF_MultipleCpfValidation_WithErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var validator = new CpfTestModelValidator();
        var model = new CpfTestModel
        {
            CPF = "123.456.789-00",       
            OptionalCPF = "111.111.111-11", 
            CustomMessageCPF = "999.999.999-99"
        };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(3, result.Errors.Count);

        var cpfError = result.Errors.FirstOrDefault(e => e.PropertyName == nameof(CpfTestModel.CPF));
        var optionalCpfError = result.Errors.FirstOrDefault(e => e.PropertyName == nameof(CpfTestModel.OptionalCPF));
        var customMessageCpfError = result.Errors.FirstOrDefault(e => e.PropertyName == nameof(CpfTestModel.CustomMessageCPF));

        Assert.NotNull(cpfError);
        Assert.NotNull(optionalCpfError);
        Assert.NotNull(customMessageCpfError);

        Assert.Equal("'CPF' não é um CPF válido.", cpfError.ErrorMessage);
        Assert.Equal("'Optional CPF' deve ser um CPF válido quando informado.", optionalCpfError.ErrorMessage);
        Assert.Equal("Mensagem personalizada para CPF inválido", customMessageCpfError.ErrorMessage);
    }

    [Theory]
    [InlineData("111.444.777-35")]
    [InlineData("11144477735")]
    [InlineData("000.000.001-91")]
    [InlineData("00000000191")]
    public void IsValidCPF_EdgeCaseValidCpfs_ShouldPass(string cpf)
    {
        // Arrange
        var validator = new CpfOnlyValidator();
        var model = new CpfTestModel { CPF = cpf };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid, $"CPF '{cpf}' deveria ser válido");
    }

    [Theory]
    [InlineData("000.000.000-00")] 
    [InlineData("123.456.789-10")] 
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    [InlineData("abc.def.ghi-jk")] 
    public void IsValidCPF_EdgeCaseInvalidCpfs_ShouldFail(string cpf)
    {
        // Arrange
        var validator = new CpfOnlyValidator();
        var model = new CpfTestModel { CPF = cpf };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid, $"CPF '{cpf}' deveria ser inválido");
    }
}