namespace HomeStore.API.DependencyInjection;

public interface IDatabaseDataSeeder
{
    Task SeedAsync();
}
