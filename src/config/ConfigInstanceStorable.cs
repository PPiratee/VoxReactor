
namespace PPirate.VoxReactor
{
    internal abstract class ConfigInstanceStorable
    {
        private readonly string idPrefix;
        private readonly ConfigInstanceStorable parent;
        
        public ConfigInstanceStorable(string idPrefix) { 
            this.idPrefix = idPrefix;
        }
        public ConfigInstanceStorable(string idPrefix, ConfigInstanceStorable parent)
        {
            this.parent = parent;
            this.idPrefix = parent.GetStorableIdPrefix() + idPrefix;
        }
        public ConfigInstanceStorable(ConfigInstanceStorable parent)
        {
            this.parent = parent;
            this.idPrefix = parent.GetStorableIdPrefix() + idPrefix;
        }
        protected string GetStorableIdPrefix() {
            if (parent != null) { 
                return parent.GetStorableIdPrefix() + idPrefix;
            }
            return idPrefix;
        }
        protected string GetStorableId(string idSuffix)
        {
            return idPrefix + idSuffix;
        }
        
    }
}
