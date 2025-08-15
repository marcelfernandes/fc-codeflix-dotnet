using Bogus;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Validation;
using FluentAssertions;

namespace FC.Codeflix.Catalog.Tests.Unit.Domain.Validation;

public class DomainValidationTest
{
    private Faker Faker { get; set; } = new Faker("pt_BR");
    
    #region [ nao ser null ]
    
    [Fact(DisplayName = nameof(NotNull_ShouldNotThrowException_WhenPropertyIsNotNull))]
    [Trait("Category", "DomainValidation - Validation")]
    public void NotNull_ShouldNotThrowException_WhenPropertyIsNotNull()
    {
        // Arrange
        var value = Faker.Commerce.ProductName();
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        
        Action action = () => 
            DomainValidation.NotNull(value, fieldName);
        // Act & Assert
        action.Should().NotThrow();
    }
    
    [Fact(DisplayName = nameof(NotNull_ShouldThrowException_WhenPropertyIsNull))]
    [Trait("Category", "DomainValidation - Validation")]
    public void NotNull_ShouldThrowException_WhenPropertyIsNull()
    {
        // Arrange
        string? value = null;
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        
        Action action = () => 
            DomainValidation.NotNull(value, fieldName);
        // Act & Assert
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be null");
    }
    
    #endregion [ nao ser null ]
    
    #region [ nao ser null ou vazio ]
    
    [Theory(DisplayName = nameof(NotNullOrEmpty_ShouldThrowException_WhenPropertyIsEmpty))]
    [Trait("Category", "DomainValidation - Validation")]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void NotNullOrEmpty_ShouldThrowException_WhenPropertyIsEmpty(string? target)
    {
        // Arrange
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        
        Action action = () => 
            DomainValidation.NotNullOrEmpty(target, fieldName);
        // Act & Assert
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be empty or null");
    }
    
    [Fact(DisplayName = nameof(NotNullOrEmpty_ShouldNotThrowException_WhenPropertyIsNotEmpty))]
    [Trait("Category", "DomainValidation - Validation")]
    public void NotNullOrEmpty_ShouldNotThrowException_WhenPropertyIsNotEmpty()
    {
        // Arrange
        var target = Faker.Commerce.ProductName();
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        
        Action action = () => 
            DomainValidation.NotNullOrEmpty(target, fieldName);
        // Act & Assert
        action.Should()
            .NotThrow();
    }
    
    #endregion [ nao ser null ou vazio ]
    
    #region [ tamanho mínimo ] 
    
    [Theory(DisplayName = nameof(MinLength_ShouldThrowException_WhenPropertyIsLessThanMinimumLength))]
    [Trait("Category", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesSmallerThanMinLength), parameters: 10)]
    public void MinLength_ShouldThrowException_WhenPropertyIsLessThanMinimumLength(string target, int minLength)
    {
        // Arrange
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        
        Action action = () => 
            DomainValidation.MinLength(target, minLength, fieldName);
        
        // Act & Assert
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be at least {minLength} characters long");
    }
    
    public static IEnumerable<object[]> GetValuesSmallerThanMinLength(int numberOfValidStrings)
    {
        yield return new object[] { "ab", 3 };
        var faker = new Faker("pt_BR");
        for (int i = 0; i < (numberOfValidStrings - 1); i++)
        {
            var value = faker.Commerce.ProductName();
            var minLength = value.Length + (new Random()).Next(1, 5);
            
            yield return new object[] { value, minLength };
        }
    }
    
    [Theory(DisplayName = nameof(MinLength_ShouldNotThrowException_WhenPropertyIsGreaterOrEqualThanMinimumLength))]
    [Trait("Category", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesGreaterThanMinLength), parameters: 10)]
    public void MinLength_ShouldNotThrowException_WhenPropertyIsGreaterOrEqualThanMinimumLength(string target, int minLength)
    {
        // Arrange
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        
        Action action = () => 
            DomainValidation.MinLength(target, minLength, fieldName);
        
        // Act & Assert
        action.Should()
            .NotThrow();
    }
    
    public static IEnumerable<object[]> GetValuesGreaterThanMinLength(int numberOfValidStrings)
    {
        yield return new object[] { "abc", 3 };
        var faker = new Faker("pt_BR");
        for (int i = 0; i < (numberOfValidStrings - 1); i++)
        {
            var value = faker.Commerce.ProductName();
            var minLength = new Random().Next(0, value.Length);
            
            yield return new object[] { value, minLength };
        }
    }
    
    #endregion [ tamanho mínimo ]

    #region [ tamanho máximo ]
    
    [Theory(DisplayName = nameof(MaxLength_ShouldThrowException_WhenPropertyIsGreaterThanMaximumLength))]
    [Trait("Category", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesGreaterThanMaxLength), parameters: 10)]
    public void MaxLength_ShouldThrowException_WhenPropertyIsGreaterThanMaximumLength(string target, int maxLength)
    {
        // Arrange
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        
        Action action = () => 
            DomainValidation.MaxLength(target, maxLength, fieldName);
        
        // Act & Assert
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be less or equal than {maxLength} characters long");
    }
    
    public static IEnumerable<object[]> GetValuesGreaterThanMaxLength(int numberOfValidStrings)
    {
        yield return new object[] { "abcdefghij", 5 };
        var faker = new Faker("pt_BR");
        for (int i = 0; i < (numberOfValidStrings - 1); i++)
        {
            var value = faker.Commerce.ProductName();
            var maxLength = value.Length - (new Random()).Next(1, 5);
            
            yield return new object[] { value, maxLength };
        }
    }
    
    [Theory(DisplayName = nameof(MaxLength_ShouldNotThrowException_WhenPropertyIsLessOrEqualThanMaximumLength))]
    [Trait("Category", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesLessOrEqualThanMaxLength), parameters: 10)]
    public void MaxLength_ShouldNotThrowException_WhenPropertyIsLessOrEqualThanMaximumLength(string target, int maxLength)
    {
        // Arrange
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        
        Action action = () => 
            DomainValidation.MaxLength(target, maxLength, fieldName);
        
        // Act & Assert
        action.Should()
            .NotThrow();
    }
    
    public static IEnumerable<object[]> GetValuesLessOrEqualThanMaxLength(int numberOfValidStrings)
    {
        yield return new object[] { "12345", 5 };
        var faker = new Faker("pt_BR");
        for (int i = 0; i < (numberOfValidStrings - 1); i++)
        {
            var value = faker.Commerce.ProductName();
            var maxLength = new Random().Next(value.Length, value.Length + 5);
            
            yield return new object[] { value, maxLength };
        }
    }
    
    #endregion [ tamanho máximo ]
}