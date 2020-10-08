namespace Lambada.Generators.Options
{
    public class CosmosDbOptions
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public string FactoryContainerName { get; set; }
        public string FactoryStatsContainerName { get; set; }
    }
}