using Extensions.FluentValidation.Brazil.Validators;
using Xunit;

namespace Extensions.FluentValidation.Brazil.Tests.Validators;

public class CpfValidatorTests
{
    [Theory]
    [InlineData("111.444.777-35", true)]   // CPF válido formatado
    [InlineData("11144477735", true)]      // CPF válido sem formatação  
    [InlineData("111 444 777 35", true)]   // CPF válido com espaços
    [InlineData("111-444-777-35", true)]   // CPF válido com hífens
    [InlineData("111/444/777/35", true)]   // CPF válido com barras
    public void IsValid_ValidCpfs_ShouldReturnTrue(string cpf, bool expected)
    {
        // Act
        var result = CpfValidator.IsValid(cpf);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("123.456.789-00")]         // CPF com dígitos verificadores inválidos
    [InlineData("123.456.789-10")]         // CPF com segundo dígito inválido
    [InlineData("000.000.000-00")]         // CPF com todos zeros
    [InlineData("111.111.111-11")]         // CPF com todos os dígitos iguais
    [InlineData("222.222.222-22")]         // CPF com todos os dígitos iguais
    [InlineData("999.999.999-99")]         // CPF com todos os dígitos iguais
    [InlineData("12345678901")]            // CPF numérico inválido
    [InlineData("1234567890")]             // CPF com 10 dígitos
    [InlineData("123456789012")]           // CPF com 12 dígitos
    [InlineData("")]                       // String vazia
    [InlineData("   ")]                    // String só com espaços
    [InlineData("abc.def.ghi-jk")]         // CPF com letras
    [InlineData("111.444.777")]            // CPF incompleto
    [InlineData("111.444.777-3")]          // CPF com um dígito verificador
    [InlineData("111.444.777-356")]        // CPF com 3 dígitos verificadores
    [InlineData("000.000.002-01")]         // CPF com dígitos inválidos
    public void IsValid_InvalidCpfs_ShouldReturnFalse(string cpf)
    {
        // Act
        var result = CpfValidator.IsValid(cpf);

        // Assert
        Assert.False(result, $"CPF '{cpf}' deveria ser inválido");
    }

    [Fact]
    public void IsValid_NullCpf_ShouldReturnFalse()
    {
        // Act
        var result = CpfValidator.IsValid(null);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("11144477735", "111.444.777-35")]
    [InlineData("111.444.777-35", "111.444.777-35")]
    [InlineData("111 444 777 35", "111.444.777-35")]
    public void FormatCpf_ValidCpfs_ShouldReturnFormattedCpf(string input, string expected)
    {
        // Act
        var result = CpfValidator.FormatCpf(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("12345678901")]
    [InlineData("")]
    [InlineData("111.111.111-11")]
    public void FormatCpf_InvalidCpfs_ShouldReturnEmptyString(string input)
    {
        // Act
        var result = CpfValidator.FormatCpf(input);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatCpf_NullCpf_ShouldReturnEmptyString()
    {
        // Act
        var result = CpfValidator.FormatCpf(null);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("000.000.001-91")]         // CPF válido especial (dígitos corretos)
    [InlineData("000.000.002-72")]         // Dígitos verificadores corretos
    [InlineData("012.345.678-90")]         // CPF válido comum
    public void IsValid_SpecialValidCpfs_ShouldReturnTrue(string cpf)
    {
        // Act
        var result = CpfValidator.IsValid(cpf);

        // Assert
        Assert.True(result, $"CPF '{cpf}' deveria ser válido");
    }

    [Theory]
    [InlineData("00000000191")]   
    [InlineData("00000000272")]      
    public void IsValid_SpecialValidCpfsUnformatted_ShouldReturnTrue(string cpf)
    {
        var result = CpfValidator.IsValid(cpf);

        Assert.True(result, $"CPF '{cpf}' deveria ser válido");
    }
}