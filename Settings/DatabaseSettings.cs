using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionAppAcoes.Settings
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string AcoesCollectionName { get; set; }
    }

    public interface IDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string AcoesCollectionName { get; set; }
    }
}
