# Extensions.FluentValidation.Brazil

🇧🇷 Extensões FluentValidation para validações brasileiras

[![NuGet](https://img.shields.io/nuget/v/Extensions.FluentValidation.Brazil.svg)](https://www.nuget.org/packages/Extensions.FluentValidation.Brazil)
[![Downloads](https://img.shields.io/nuget/dt/Extensions.FluentValidation.Brazil.svg)](https://www.nuget.org/packages/Extensions.FluentValidation.Brazil)

## 📦 Instalação

```bash
dotnet add package Extensions.FluentValidation.Brazil
```

## 🚀 Início Rápido

```csharp
using FluentValidation;
using Extensions.FluentValidation.Brazil.Extensions;

public class Person
{
    public string CPF { get; set; }
    public string CNPJ { get; set; }
}

public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(x => x.CPF).IsValidCPF();
        RuleFor(x => x.CNPJ).IsValidCNPJ();
    }
}
```

## ✅ Validações Disponíveis

### CPF

| Método | Descrição | Exemplo |
|--------|-----------|---------|
| `IsValidCPF()` | CPF obrigatório | `RuleFor(x => x.CPF).IsValidCPF();` |
| `IsValidCPF(allowEmpty: true)` | CPF opcional | `RuleFor(x => x.CPF).IsValidCPF(allowEmpty: true);` |
| `IsValidCPF("mensagem")` | CPF com mensagem customizada | `RuleFor(x => x.CPF).IsValidCPF("CPF inválido");` |

### CNPJ

| Método | Descrição | Exemplo |
|--------|-----------|---------|
| `IsValidCNPJ()` | CNPJ obrigatório | `RuleFor(x => x.CNPJ).IsValidCNPJ();` |
| `IsValidCNPJ(allowEmpty: true)` | CNPJ opcional | `RuleFor(x => x.CNPJ).IsValidCNPJ(allowEmpty: true);` |
| `IsValidCNPJ("mensagem")` | CNPJ com mensagem customizada | `RuleFor(x => x.CNPJ).IsValidCNPJ("CNPJ inválido");` |

### Detecção Automática

| Método | Descrição | Exemplo |
|--------|-----------|---------|
| `IsValidCpfOrCnpj()` | Detecta CPF ou CNPJ automaticamente | `RuleFor(x => x.Document).IsValidCpfOrCnpj();` |
| `IsValidCpfOrCnpj(allowEmpty: true)` | CPF/CNPJ opcional | `RuleFor(x => x.Document).IsValidCpfOrCnpj(allowEmpty: true);` |

## 📋 Exemplos Práticos

### Validação de Pessoa Física

```csharp
public class Customer
{
    public string Name { get; set; }
    public string CPF { get; set; }
    public string? OptionalCPF { get; set; }
}

public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome é obrigatório");

        RuleFor(x => x.CPF)
            .IsValidCPF();

        RuleFor(x => x.OptionalCPF)
            .IsValidCPF(allowEmpty: true);
    }
}
```

### Validação de Empresa

```csharp
public class Company
{
    public string CompanyName { get; set; }
    public string CNPJ { get; set; }
    public string ResponsibleCPF { get; set; }
}

public class CompanyValidator : AbstractValidator<Company>
{
    public CompanyValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Nome da empresa é obrigatório");

        RuleFor(x => x.CNPJ)
            .IsValidCNPJ()
            .WithMessage("CNPJ deve ser válido");

        RuleFor(x => x.ResponsibleCPF)
            .IsValidCPF("CPF do responsável deve ser válido");
    }
}
```

### Validação Automática (CPF ou CNPJ)

```csharp
public class BusinessPartner
{
    public string Name { get; set; }
    public string Document { get; set; } // CPF ou CNPJ
}

public class BusinessPartnerValidator : AbstractValidator<BusinessPartner>
{
    public BusinessPartnerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Document)
            .IsValidCpfOrCnpj()
            .WithMessage("Documento deve ser um CPF ou CNPJ válido");
    }
}
```

### Múltiplas Validações

```csharp
public class Registration
{
    public string PersonCPF { get; set; }
    public string CompanyCNPJ { get; set; }
    public string? OptionalDocument { get; set; }
}

public class RegistrationValidator : AbstractValidator<Registration>
{
    public RegistrationValidator()
    {
        RuleFor(x => x.PersonCPF)
            .IsValidCPF()
            .WithMessage("CPF da pessoa deve ser válido");

        RuleFor(x => x.CompanyCNPJ)
            .IsValidCNPJ()
            .WithMessage("CNPJ da empresa deve ser válido");

        RuleFor(x => x.OptionalDocument)
            .IsValidCpfOrCnpj(allowEmpty: true)
            .WithMessage("Documento opcional deve ser CPF ou CNPJ válido");
    }
}
```

## 🎯 Recursos

- ✅ **Formatação Flexível**: Aceita documentos com ou sem pontuação
- ✅ **Algoritmos Oficiais**: Validação seguindo regras da Receita Federal
- ✅ **Campos Opcionais**: Suporte completo a `allowEmpty: true`
- ✅ **Mensagens Customizadas**: Personalize mensagens de erro
- ✅ **Detecção Automática**: Identifica CPF ou CNPJ pelo tamanho
- ✅ **Performance Otimizada**: Validação rápida e eficiente
- ✅ **Nullable Support**: Compatível com tipos nullable
- ✅ **Clean Input**: Remove automaticamente pontuação

## 📝 Formatos Aceitos

### CPF
```csharp
// Todos estes formatos são válidos:
"111.444.777-35"    // Formato padrão
"11144477735"       // Apenas números
"111 444 777 35"    // Com espaços
"111-444-777-35"    // Com hífens
```

### CNPJ
```csharp
// Todos estes formatos são válidos:
"11.222.333/0001-81"    // Formato padrão
"11222333000181"        // Apenas números
"11 222 333 0001 81"    // Com espaços
"11-222-333-0001-81"    // Com hífens
```

## ⚡ Uso com ASP.NET Core

```csharp
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IValidator<Customer> _validator;

    public CustomersController(IValidator<Customer> validator)
    {
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Customer customer)
    {
        var validationResult = await _validator.ValidateAsync(customer);
        
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new 
            { 
                Field = e.PropertyName, 
                Error = e.ErrorMessage 
            }));
        }

        // Processar customer válido...
        return Ok();
    }
}

// Startup.cs ou Program.cs
services.AddScoped<IValidator<Customer>, CustomerValidator>();
```

## 🔍 Validação Manual

Se precisar validar fora do FluentValidation:

```csharp
using Extensions.FluentValidation.Brazil.Validators;

// Validar CPF
bool cpfValido = CpfValidator.IsValid("111.444.777-35");
string cpfFormatado = CpfValidator.FormatCpf("11144477735");

// Validar CNPJ  
bool cnpjValido = CnpjValidator.IsValid("11.222.333/0001-81");
string cnpjFormatado = CnpjValidator.FormatCnpj("11222333000181");
```

## 🧪 Testando Validações

```csharp
[Test]
public void Should_Validate_CPF_Successfully()
{
    // Arrange
    var customer = new Customer { CPF = "111.444.777-35" };
    var validator = new CustomerValidator();

    // Act
    var result = validator.Validate(customer);

    // Assert
    Assert.True(result.IsValid);
}

[Test]
public void Should_Reject_Invalid_CPF()
{
    // Arrange
    var customer = new Customer { CPF = "123.456.789-00" };
    var validator = new CustomerValidator();

    // Act
    var result = validator.Validate(customer);

    // Assert
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.PropertyName == "CPF");
}
```

## 📊 Mensagens de Erro Padrão

| Validação | Mensagem |
|-----------|----------|
| CPF obrigatório | `'Campo' é obrigatório.` |
| CPF inválido | `'Campo' não é um CPF válido.` |
| CPF opcional inválido | `'Campo' deve ser um CPF válido quando informado.` |
| CNPJ obrigatório | `'Campo' é obrigatório.` |
| CNPJ inválido | `'Campo' não é um CNPJ válido.` |
| CNPJ opcional inválido | `'Campo' deve ser um CNPJ válido quando informado.` |
| CPF/CNPJ inválido | `'Campo' deve ser um CPF ou CNPJ válido.` |

## 🌟 Casos de Uso Comuns

### E-commerce
```csharp
public class CheckoutValidator : AbstractValidator<Checkout>
{
    public CheckoutValidator()
    {
        RuleFor(x => x.CustomerDocument)
            .IsValidCpfOrCnpj()
            .WithMessage("Documento do cliente inválido");
    }
}
```

### Sistema de RH
```csharp
public class EmployeeValidator : AbstractValidator<Employee>
{
    public EmployeeValidator()
    {
        RuleFor(x => x.CPF)
            .IsValidCPF()
            .WithMessage("CPF do funcionário deve ser válido");
    }
}
```

### Sistema Financeiro
```csharp
public class AccountValidator : AbstractValidator<Account>
{
    public AccountValidator()
    {
        RuleFor(x => x.HolderDocument)
            .IsValidCpfOrCnpj()
            .WithMessage("Documento do titular da conta inválido");
    }
}
```

## 📄 Licença

MIT License

---

**Desenvolvido com ❤️ para a comunidade .NET brasileira** 🇧🇷