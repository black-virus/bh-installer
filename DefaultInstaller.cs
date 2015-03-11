using System;
using System.Collections.Generic;
using blackhouse.Installer.InstalledVersionsStorage;

namespace blackhouse.Installer
{
    public class DefaultInstaller
    {

        #region Fields

        private readonly IInstallStorage storage;
        private readonly IList<InstallBase> installs = new List<InstallBase>();

        #endregion

        #region Properties
        #endregion

        #region Constructors

        public DefaultInstaller(IInstallStorage storage) {
            this.storage = storage;
        }

        #endregion

        #region Methods

        public void RegisterInstaller(InstallBase install) {
            this.installs.Add(install);
        }

        public void RegisterInstaller(IInstallers installers) {
            foreach (var install in installs) {
                this.RegisterInstaller(install);
            }
        }

        public void Install() {
            foreach (var install in this.installs) {
                var installedVersion = this.Install(install);
                this.storage.SetInstalledVersion(install.ModuleName, installedVersion);
            }
        }

        private Version Install(InstallBase install) {
            var latestVersion = install.GetLatestVersion();
            var storageInstall = this.storage.GetInstalledVersion(install.ModuleName);
            if (latestVersion == storageInstall) return null;
            var installedVersion = storageInstall;
            var toInstallVersion = IncrementVersion(storageInstall);
            while (toInstallVersion <= latestVersion) {
                if (!install.UpToVersion(toInstallVersion)) return installedVersion;
                installedVersion = toInstallVersion;
                toInstallVersion = IncrementVersion(toInstallVersion);
            }
            return installedVersion;
        }

        private Version IncrementVersion(Version version) {
            if (version == null) return new Version(1, 0);
            var revision = version.Revision;
            var build = version.Build;
            var minor = version.Minor;
            var major = version.Major;
            revision++;
            if (IncNext(ref revision, ref build))
                if (IncNext(ref build, ref minor))
                    IncNext(ref minor, ref major);
            return new Version(major, minor, build, revision);
        }

        private bool IncNext(ref int first, ref int sec) {
            first++;
            if (first == 10) {
                first = 0;
                sec++;
                return true;
            }
            return false;
        }

        #endregion

    }
}
