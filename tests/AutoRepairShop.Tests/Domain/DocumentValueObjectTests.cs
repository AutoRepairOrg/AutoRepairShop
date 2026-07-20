using AutoRepairShop.Domain.Exceptions;
using AutoRepairShop.Domain.ValueObjects;

namespace AutoRepairShop.Tests.Domain;

public class DocumentValueObjectTests
{
    [Fact]
    public void Create_WithValidCpf_ShouldReturnNormalizedDigitsOnlyValue()
    {
        var document = Document.Create("529.982.247-25");

        Assert.Equal("52998224725", document.Value);
    }

    [Fact]
    public void Create_WithValidCnpj_ShouldReturnNormalizedDigitsOnlyValue()
    {
        var document = Document.Create("45.723.174/0001-10");

        Assert.Equal("45723174000110", document.Value);
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldThrowDomainException()
    {
        var action = () => Document.Create(string.Empty);

        var exception = Assert.Throws<DomainException>(action);
        Assert.Equal("Document is required", exception.Message);
    }

    [Fact]
    public void Create_WithInvalidDocument_ShouldThrowDomainException()
    {
        var action = () => Document.Create("11111111111");

        var exception = Assert.Throws<DomainException>(action);
        Assert.Equal("Invalid CPF or CNPJ", exception.Message);
    }
}
