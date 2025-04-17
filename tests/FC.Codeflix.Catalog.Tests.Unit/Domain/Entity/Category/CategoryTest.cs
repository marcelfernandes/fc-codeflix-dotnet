using System;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Tests.Unit.Domain.Entity.Category;

public class CategoryTest
{
    [Fact(DisplayName = nameof(CategoryDomain_ShouldInstantiateSuccessfully_WhenConstructorIsCalled))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldInstantiateSuccessfully_WhenConstructorIsCalled()
    {
        // Arrange
        var validData = new
        {
            Name = "category name",
            Description = "category Description"
        };
        var datetimeBefore = DateTime.Now;
        
        // Act
        var category = new DomainEntity.Category(validData.Name, validData.Description);
        var datetimeAfter = DateTime.Now;
        
        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(validData.Name);
        category.Description.Should().Be(validData.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        category.CreatedAt.Should().BeAfter(datetimeBefore);
        category.CreatedAt.Should().BeBefore(datetimeAfter);
        category.IsActive.Should().BeTrue();
    }
    
    [Theory(DisplayName = nameof(CategoryDomain_ShouldInstantiateWithIsActiveSuccessfully_WhenConstructorIsCalled))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void CategoryDomain_ShouldInstantiateWithIsActiveSuccessfully_WhenConstructorIsCalled(bool isActive)
    {
        // Arrange
        var validData = new
        {
            Name = "category name",
            Description = "category Description",
            IsActive = isActive,
        };
        var datetimeBefore = DateTime.Now;
        
        // Act
        var category = new DomainEntity.Category(validData.Name, validData.Description, isActive);
        var datetimeAfter = DateTime.Now;
        
        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(validData.Name);
        category.Description.Should().Be(validData.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        category.CreatedAt.Should().BeAfter(datetimeBefore);
        category.CreatedAt.Should().BeBefore(datetimeAfter);
        category.IsActive.Should().Be(isActive);
    }
    
    [Theory(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CategoryDomain_ShouldThrow_WhenNameIsEmpty(string? name)
    {
        Action action = () => new DomainEntity.Category(name!, "cat description");
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }
    
    [Fact(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldThrow_WhenDescriptionIsNull()
    {
        Action action = () => new DomainEntity.Category("Nome category", null!);
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should not be empty or null");
    }
    
    [Theory(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenNameIsLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("a")]
    [InlineData("ab")]
    public void CategoryDomain_ShouldThrow_WhenNameIsLessThan3Characters(string? invalidName)
    {
        Action action = () => new DomainEntity.Category(invalidName!, "cat description");
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characters long");
    }

    [Fact(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldThrow_WhenNameIsGreaterThan255Characters()
    {
        var invalidName = string.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());
        
        Action action = () => new DomainEntity.Category(invalidName, "cat description");
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal than 255 characters long");
    }
    
    [Fact(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenDescriptionIsGreaterThan10000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldThrow_WhenDescriptionIsGreaterThan10000Characters()
    {
        var invalidDescription = string.Join(null, Enumerable.Range(0, 10_001).Select(_ => "a").ToArray());
        
        Action action = () => new DomainEntity.Category("Valid name", invalidDescription);
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal than 10.000 characters long");
    }
    
    [Fact(DisplayName = nameof(CategoryDomain_ShouldActivate_WhenInactive))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldActivate_WhenInactive()
    {
        // Arrange
        var validData = new
        {
            Name = "category name",
            Description = "category Description",
        };
        var category = new DomainEntity.Category(validData.Name, validData.Description, false);
        
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
        var validData = new
        {
            Name = "category name",
            Description = "category Description",
        };
        var category = new DomainEntity.Category(validData.Name, validData.Description, true);
        
        // Act
        category.Deactivate();
        
        // Assert
        category.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(CategoryDomain_ShouldUpdateNameAndDescription))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldUpdateNameAndDescription()
    {
        var category = new DomainEntity.Category("category name", "category description");
        var newValues = new { Name = "new category name", Description = "new category description" };

        category.Update(newValues.Name, newValues.Description);
        
        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(newValues.Description);
    }

    [Fact(DisplayName = nameof(CategoryDomain_ShouldUpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldUpdateOnlyName()
    {
        var category = new DomainEntity.Category("category name", "category description");
        var newValues = new { Name = "new category name" };
        var currentDescription = category.Description;

        category.Update(newValues.Name);
        
        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(currentDescription);
    }
    
    [Theory(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenNameIsEmptyOnUpdate))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CategoryDomain_ShouldThrow_WhenNameIsEmptyOnUpdate(string? name)
    {
        var category = new DomainEntity.Category("category name", "category description");
        
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
        var category = new DomainEntity.Category("category name", "category description");
        
        Action action = () => category.Update(invalidName!);
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characters long");
    }

    [Fact(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenNameIsGreaterThan255CharactersOnUpdate))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldThrow_WhenNameIsGreaterThan255CharactersOnUpdate()
    {
        var category = new DomainEntity.Category("category name", "category description");
        
        var invalidName = string.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());
        
        Action action = () => category.Update(invalidName, "cat description");

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal than 255 characters long");
    }

    [Fact(DisplayName = nameof(CategoryDomain_ShouldThrow_WhenDescriptionIsGreaterThan10000CharactersOnUpdate))]
    [Trait("Domain", "Category - Aggregates")]
    public void CategoryDomain_ShouldThrow_WhenDescriptionIsGreaterThan10000CharactersOnUpdate()
    {
        var category = new DomainEntity.Category("category name", "category description");

        var invalidDescription = string.Join(null, Enumerable.Range(0, 10_001).Select(_ => "a").ToArray());
        
        Action action = () => category.Update("Valid new name", invalidDescription);
        
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal than 10.000 characters long");
    }

}