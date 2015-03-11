using System;
using System.Collections.Generic;
using System.IO;

namespace blackhouse.Installer.InstalledVersionsStorage
{
    public class FileInstallStorage : IInstallStorage
    {

        #region Fields

        private readonly FileInfo storageFile;
        private readonly Dictionary<string, Version> versions;

        #endregion

        #region Constructors

        public FileInstallStorage(string directoryPath) {
            var directory = new DirectoryInfo(directoryPath);
            if (!directory.Exists) directory.Create();
            this.storageFile = new FileInfo(directory.FullName + @"\versions.install");
            if (!this.storageFile.Exists) this.storageFile.Create();
            this.versions = this.ReadFromFile();
        }

        #endregion

        #region Methods

        public Version GetInstalledVersion(string moduleName) {
            return this.versions.ContainsKey(moduleName) ? this.versions[moduleName] : null;
        }

        public void SetInstalledVersion(string moduleName, Version installedVersion) {
            this.versions[moduleName] = installedVersion;
            this.SaveToFile(this.versions);
        }

        private Dictionary<string, Version> ReadFromFile() {
            var result = new Dictionary<string, Version>();
            using (var sr = this.storageFile.OpenText()) {
                while (true) {
                    var line = sr.ReadLine();
                    if (String.IsNullOrEmpty(line)) break;
                    var lineParts = line.Split('\t');
                    if (lineParts.Length != 2) continue;
                    Version version = null;
                    if (!Version.TryParse(lineParts[0], out version)) continue;
                    result.Add(lineParts[1], version);
                }
            }
            return result;
        }

        private void SaveToFile(Dictionary<string, Version> versionData) {
            using (var fs = this.storageFile.Open(FileMode.Truncate))
            using (var sw = new StreamWriter(fs)) {
                foreach (var version in versionData) {
                    sw.WriteLine("{0}\t{1}", version.Value, version.Key);
                }
            }
        }

        #endregion

    }
}
