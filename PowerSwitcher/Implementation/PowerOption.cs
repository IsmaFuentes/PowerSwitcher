using System;

namespace PowerSwitcher.Implementation
{
    public class PowerOption
    {
        public readonly string Name;
        public Guid PowerId;

        public PowerOption(string Name, Guid PowerId)
        {
            this.Name = Name;
            this.PowerId = PowerId;
        }
    }
}
