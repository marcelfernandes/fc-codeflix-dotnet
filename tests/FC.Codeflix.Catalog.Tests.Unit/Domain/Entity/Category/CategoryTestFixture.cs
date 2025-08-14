using Bogus;
using FC.Codeflix.Catalog.Tests.Unit.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Tests.Unit.Domain.Entity.Category;

public class CategoryTestFixture : BaseFixture
{
    public CategoryTestFixture() 
        : base() {}

    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
            categoryDescription = categoryDescription[..10_000];

        return categoryDescription;
    }
    
    public DomainEntity.Category GetValidCategory()
        => new(GetValidCategoryName(), 
            GetValidCategoryDescription()
        );
}

[CollectionDefinition((nameof(CategoryTestFixture)))]
public class CategoryTestFixtureCollection 
    : ICollectionFixture<CategoryTestFixture>
{
    // This class is used to group the CategoryTestFixture
    // with the CategoryTest class, so that the fixture is
    // created only once for all tests in the CategoryTest class.
    // It is required by xUnit to use the fixture in the test class.
}