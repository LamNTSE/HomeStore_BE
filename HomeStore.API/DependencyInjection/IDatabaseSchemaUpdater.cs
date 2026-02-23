namespace HomeStore.API.DependencyInjection;

public interface IDatabaseSchemaUpdater
{
    Task UpdateSchemaAsync();
}
