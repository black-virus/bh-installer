using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace blackhouse.Installer.SQL
{
    public class SqlInstall : InstallBase
    {

        #region Fields

        private readonly string sqlConnectionString;
        private readonly Assembly assembly;
        private IEnumerable<AssemblySqlFile> files;

        #endregion

        #region Properties

        private IEnumerable<AssemblySqlFile> AssemblyFiles {
            get {
                if (this.files != null) return this.files;
                this.files = this.GetSqlFiles();
                return this.files;
            }
        }

        public bool HaveSqlFiles {
            get { return this.AssemblyFiles.Any(); }
        }

        #endregion

        #region Constructors

        public SqlInstall(Assembly assembly, string sqlConnectionString)
            : base("SQL - " + assembly.GetName().Name) {
            this.assembly = assembly;
            this.sqlConnectionString = sqlConnectionString;
        }

        #endregion

        #region Methods
        #endregion

        protected internal override Version GetLatestVersion() {
            return this.AssemblyFiles
                .OrderByDescending(file => file.FileVersion)
                .First()
                .FileVersion;
        }

        protected internal override bool UpToVersion(Version version) {
            var fileToRun = this.AssemblyFiles.FirstOrDefault(file => file.FileVersion == version);
            if (fileToRun == null) return false;
            var script = this.ReadScript(fileToRun.FullFileName);
            if (String.IsNullOrEmpty(script)) return false;
            try {
                this.RunScript(script);
            } catch (SqlVersionException exc) {
                Trace.Write(exc);
                return false;
            }
            return true;
        }

        private string ReadScript(string file) {
            using (var st = this.assembly.GetManifestResourceStream(file)) {
                if (st == null) return String.Empty;
                using (var streamReader = new StreamReader(st)) {
                    return streamReader.ReadToEnd();
                }
            }
        }

        private void RunScript(string script) {
            if (String.IsNullOrEmpty(script)) return;
            using (var connection = new SqlConnection(this.sqlConnectionString)) {
                connection.Open();
                using (var transaction = connection.BeginTransaction()) {
                    try {
                        var sciptParts = script.Split(new[] { "GO\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var scriptPart in sciptParts) {
                            var scriptPartExecute = scriptPart.Trim();
                            if (String.IsNullOrEmpty(scriptPartExecute)) continue;
                            var command = connection.CreateCommand();
                            command.Transaction = transaction;
                            command.CommandText = scriptPartExecute;
                            command.CommandType = CommandType.Text;
                            try {
                                command.ExecuteNonQuery();
                            } catch (SqlException sqlException) {
                                throw new SqlVersionException(sqlException, scriptPartExecute);
                            }
                        }
                        transaction.Commit();
                    } catch (Exception) {
                        transaction.Rollback();
                        throw;
                    } finally {
                        connection.Close();
                    }
                }
            }
        }

        private IEnumerable<AssemblySqlFile> GetSqlFiles() {
            return this.assembly.GetManifestResourceNames()
                .Where(s => s.EndsWith(".sql"))
                .Select(s => new AssemblySqlFile(assembly, s))
                .Where(af => af.IsValid);
        }

    }
}
