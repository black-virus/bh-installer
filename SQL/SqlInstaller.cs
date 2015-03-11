using System;
using System.Collections.Generic;
using System.Linq;

namespace blackhouse.Installer.SQL
{
    public class SqlInstaller : IInstallers
    {

        #region Fields

        private readonly string sqlConnectionString;

        #endregion

        #region Constructors

        public SqlInstaller(string connectionString) {
            this.sqlConnectionString = connectionString;
        }

        #endregion

        #region Methods

        public IEnumerable<InstallBase> GetInstallers() {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Select(domainAssembly =>
                    new SqlInstall(domainAssembly, this.sqlConnectionString))
                .Where(install => install.HaveSqlFiles);
        }

        #endregion

    }
}
