using System.Collections.Generic;

namespace blackhouse.Installer
{
    public interface IInstallers
    {
        IEnumerable<InstallBase> GetInstallers();
    }
}