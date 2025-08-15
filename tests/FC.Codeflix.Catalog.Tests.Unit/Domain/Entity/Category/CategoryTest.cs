using System;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Tests.Unit.Domain.Entity.Category;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest
{
    private readonly CategoryTestFixture _categoryTestFixture;
    
    public CategoryTest(CategoryTestFixture categoryTestFixture)
        => _categoryTestFixture = categoryTestFixture;
    
    [Fact(DisplayName = nameof(CategoryDomain_ShouldInstantiateSuccessfully_WhenConstructorIsCalled))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldInstantiateSuccessfully_WhenConstructorIsCalled()
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        var datetimeBefore = DateTime.Now;
        
        // Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
        var datetimeAfter = DateTime.Now.AddMilliseconds(1);
        
        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        category.CreatedAt.Should().BeOnOrAfter(datetimeBefore);
        category.CreatedAt.Should().BeOnOrBefore(datetimeAfter);
        category.IsActive.Should().BeTrue();
    }
    
    [Theory(DisplayName = nameof(CategoryDomain_ShouldInstantiateWithIsActiveSuccessfully_WhenConstructorIsCalled))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void CategoryDomain_ShouldInstantiateWithIsActiveSuccessfully_WhenConstructorIsCalled(bool isActive)
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        var datetimeBefore = DateTime.Now;
        
        // Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);
        var datetimeAfter = DateTime.Now.AddMilliseconds(1);
        
        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        category.CreatedAt.Should().BeOnOrAfter(datetimeBefore);
        category.CreatedAt.Should().BeOnOrBefore(datetimeAfter);
        category.IsActive.Should().Be(isActive);
    }
    
    [Theory(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CategoryDomain_ShouldThrow_WhenNameIsEmpty(string? name)
    {
        var category = _categoryTestFixture.GetValidCategory();
        
        Action action = () => new DomainEntity.Category(name!, category.Description);
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }
    
    [Fact(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldThrow_WhenDescriptionIsNull()
    {
        var category = _categoryTestFixture.GetValidCategory();
        
        Action action = () => new DomainEntity.Category(category.Name, null!);
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should not be null");
    }
    
    [Theory(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenNameIsLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [MemberData(nameof(GetNamesWithLessThan3Characters), parameters: 10)]
    public void CategoryDomain_ShouldThrow_WhenNameIsLessThan3Characters(string? invalidName)
    {
        var category = _categoryTestFixture.GetValidCategory();
        
        Action action = () => new DomainEntity.Category(invalidName!, category.Description);
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characters long");
    }
    
    public static IEnumerable<object[]> GetNamesWithLessThan3Characters(int numberOfTestes = 6)
    {
        var fixture = new CategoryTestFixture();
        for (int i = 0; i < numberOfTestes; i++)
        {
            var isOdd = i % 2 == 0;
            yield return new object[]
            {
                fixture.GetValidCategoryName()[..(isOdd ? 1 : 2)]
            };
        }

        yield return new object[] { "1" };
        yield return new object[] { "12" };
        yield return new object[] { "a" };
        yield return new object[] { "ab" };
    }

    [Fact(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldThrow_WhenNameIsGreaterThan255Characters()
    {
        var invalidName = string.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());
        
        var category = _categoryTestFixture.GetValidCategory();
        
        Action action = () => new DomainEntity.Category(invalidName, category.Description);
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal than 255 characters long");
    }
    
    [Fact(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenDescriptionIsGreaterThan10000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldThrow_WhenDescriptionIsGreaterThan10000Characters()
    {
        var invalidDescription = string.Join(null, Enumerable.Range(0, 10_001).Select(_ => "a").ToArray());
        
        var category = _categoryTestFixture.GetValidCategory();
        
        Action action = () => new DomainEntity.Category(category.Name, invalidDescription);
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal than 10000 characters long");
    }
    
    [Fact(DisplayName = nameof(CategoryDomain_ShouldActivate_WhenInactive))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldActivate_WhenInactive()
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, false);
        
        // Act
        category.Activate();
        
        // Assert
        category.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(CategoryDomain_ShouldInactivate_WhenActive))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldInactivate_WhenActive()
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, true);
        
        // Act
        category.Deactivate();
        
        // Assert
        category.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(CategoryDomain_ShouldUpdateNameAndDescription))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldUpdateNameAndDescription()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var newValues = _categoryTestFixture.GetValidCategory();
        
        category.Update(newValues.Name, newValues.Description);
        
        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(newValues.Description);
    }

    [Fact(DisplayName = nameof(CategoryDomain_ShouldUpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldUpdateOnlyName()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var newName = _categoryTestFixture.GetValidCategoryName();
        var currentDescription = category.Description;

        category.Update(newName);
        
        category.Name.Should().Be(newName);
        category.Description.Should().Be(currentDescription);
    }
    
    [Theory(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenNameIsEmptyOnUpdate))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CategoryDomain_ShouldThrow_WhenNameIsEmptyOnUpdate(string? name)
    {
        var category = _categoryTestFixture.GetValidCategory();
        
        Action action = () => category.Update(name!);
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenNameIsLessThan3CharactersOnUpdate))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("a")]
    [InlineData("ab")]
    public void CategoryDomain_ShouldThrow_WhenNameIsLessThan3CharactersOnUpdate(string? invalidName)
    {
        var category = _categoryTestFixture.GetValidCategory();
        
        Action action = () => category.Update(invalidName!);
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characters long");
    }

    [Fact(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenNameIsGreaterThan255CharactersOnUpdate))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldThrow_WhenNameIsGreaterThan255CharactersOnUpdate()
    {
        var category = _categoryTestFixture.GetValidCategory();
        
        var invalidName = _categoryTestFixture.Faker.Lorem.Letter(256);
        
        Action action = () => category.Update(invalidName, "cat description");

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal than 255 characters long");
    }

    [Fact(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenDescriptionIsGreaterThan10000CharactersOnUpdate))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldThrow_WhenDescriptionIsGreaterThan10000CharactersOnUpdate()
    {
        var category = _categoryTestFixture.GetValidCategory();

        var invalidDescription = _categoryTestFixture.Faker.Commerce.ProductDescription();
        while (invalidDescription.Length < 10_001)
            invalidDescription += $"{invalidDescription}{_categoryTestFixture.Faker.Commerce.ProductDescription()}";
        
        Action action = () => category.Update("Valid new name", invalidDescription);
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal than 10000 characters long");
    }

}