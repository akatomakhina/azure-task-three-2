using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductProject.DataAccess.Common.Models;
using ProductProject.DataAccess.ModelConfigurations;

namespace ProductProject.DataAccess.Context
{
    [DbConfigurationType(typeof(ProductProjectContextConfiguration))]
    public class ProductProjectContext : DbContext
    {
        /*private const string _connectionName = "ProductProjectConnection";
        private const string _configFileName = "ProductProjectAzure.DataAccess.dll.config";
        private static readonly string _connectionString;

        static ProductProjectContext()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string fullCfgFileName = $@"{Path.GetDirectoryName(path)}\{_configFileName}";

            var configMap = new ExeConfigurationFileMap() { ExeConfigFilename = fullCfgFileName };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            foreach (ConnectionStringSettings connectionString in config.ConnectionStrings.ConnectionStrings)
            {
                var c1 = connectionString.Name;
                var c2 = _connectionName;
                if (connectionString.Name.Equals(c2, StringComparison.OrdinalIgnoreCase))
                {
                    _connectionString = connectionString.ConnectionString;
                    break;
                }
            }

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ConfigurationErrorsException($"Cannot find connection string with name {_connectionName}");
            }
        }*/

        public ProductProjectContext() : base("ProductProjectConnection")
        {
        }

        public IDbSet<DbProduct> Products { get; set; }
        public IDbSet<TransactionHistory> TransactionHistories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ProductDbModelConfig());
            modelBuilder.Configurations.Add(new TransactionHistoryConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }

    public class ProductProjectContextConfiguration : DbConfiguration
    {
        public ProductProjectContextConfiguration()
        {
            this.SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
        }
    }
}
