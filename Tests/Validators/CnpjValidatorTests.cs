using Extensions.FluentValidation.Brazil.Validators;
using Xunit;

namespace Extensions.FluentValidation.Brazil.Tests.Validators;

public class CnpjValidatorTests
{
    [Theory]
    [InlineData("11.222.333/0001-81", true)]   // CNPJ válido formatado
    [InlineData("11222333000181", true)]       // CNPJ válido sem formatação  
    [InlineData("11 222 333 0001 81", true)]   // CNPJ válido com espaços
    [InlineData("11-222-333-0001-81", true)]   // CNPJ válido com hífens
    [InlineData("11/222/333/0001/81", true)]   // CNPJ válido com barras
    public void IsValid_ValidCnpjs_ShouldReturnTrue(string cnpj, bool expected)
    {
        // Act
        var result = CnpjValidator.IsValid(cnpj);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("11.222.333/0001-00")]         // CNPJ com dígitos verificadores inválidos
    [InlineData("11.222.333/0001-82")]         // CNPJ com segundo dígito inválido
    [InlineData("00.000.000/0000-00")]         // CNPJ com todos zeros
    [InlineData("11.111.111/1111-11")]         // CNPJ com todos os dígitos iguais
    [InlineData("22.222.222/2222-22")]         // CNPJ com todos os dígitos iguais
    [InlineData("99.999.999/9999-99")]         // CNPJ com todos os dígitos iguais
    [InlineData("12345678901234")]             // CNPJ numérico inválido
    [InlineData("1234567890123")]              // CNPJ com 13 dígitos
    [InlineData("123456789012345")]            // CNPJ com 15 dígitos
    [InlineData("")]                           // String vazia
    [InlineData("   ")]                        // String só com espaços
    [InlineData("ab.cde.fgh/ijkl-mn")]         // CNPJ com letras
    [InlineData("11.222.333/0001")]            // CNPJ incompleto
    [InlineData("11.222.333/0001-8")]          // CNPJ com um dígito verificador
    [InlineData("11.222.333/0001-816")]        // CNPJ com 3 dígitos verificadores
    public void IsValid_InvalidCnpjs_ShouldReturnFalse(string cnpj)
    {
        // Act
        var result = CnpjValidator.IsValid(cnpj);

        // Assert
        Assert.False(result, $"CNPJ '{cnpj}' deveria ser inválido");
    }

    [Fact]
    public void IsValid_NullCnpj_ShouldReturnFalse()
    {
        // Act
        var result = CnpjValidator.IsValid(null);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("11222333000181", "11.222.333/0001-81")]
    [InlineData("11.222.333/0001-81", "11.222.333/0001-81")]
    [InlineData("11 222 333 0001 81", "11.222.333/0001-81")]
    public void FormatCnpj_ValidCnpjs_ShouldReturnFormattedCnpj(string input, string expected)
    {
        // Act
        var result = CnpjValidator.FormatCnpj(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("12345678901234")]
    [InlineData("")]
    [InlineData("11.111.111/1111-11")]
    public void FormatCnpj_InvalidCnpjs_ShouldReturnEmptyString(string input)
    {
        // Act
        var result = CnpjValidator.FormatCnpj(input);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatCnpj_NullCnpj_ShouldReturnEmptyString()
    {
        // Act
        var result = CnpjValidator.FormatCnpj(null);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}