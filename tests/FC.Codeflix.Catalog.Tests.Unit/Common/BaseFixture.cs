using Bogus;

namespace FC.Codeflix.Catalog.Tests.Unit.Common;

public abstract class BaseFixture
{
    public Faker Faker { get; set; }
    
    protected BaseFixture()
        => Faker = new Faker("pt_BR");

}