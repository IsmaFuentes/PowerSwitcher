using System.Collections.Generic;

namespace PowerSwitcher.Implementation.Interfaces
{
    interface IPowerManager
    {
        List<PowerOption> GetPowerOptions();
        PowerOption GetCurrentPowerOption();
        void SetPowerOption(PowerOption option);
    }
}
