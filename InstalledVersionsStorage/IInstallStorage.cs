using System;

namespace blackhouse.Installer.InstalledVersionsStorage
{
    public interface IInstallStorage 
    {
        Version GetInstalledVersion(string moduleName);
        void SetInstalledVersion(string moduleName, Version installedVersion);
    }
}
