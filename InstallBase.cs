using System;

namespace blackhouse.Installer
{
    public abstract class InstallBase
    {

        #region Fields

        private readonly string moduleName;

        #endregion

        #region Properties

        public string ModuleName { get { return this.moduleName; } }

        #endregion

        #region Constructors

        protected InstallBase(string name) {
            this.moduleName = name;
        }

        #endregion

        #region Methods

        internal protected abstract Version GetLatestVersion();

        protected internal abstract bool UpToVersion(Version version);

        #endregion

    }
}
