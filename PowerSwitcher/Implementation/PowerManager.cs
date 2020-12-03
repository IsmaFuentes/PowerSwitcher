using PowerSwitcher.Implementation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace PowerSwitcher.Implementation
{
    public sealed class PowerManager : IPowerManager
    {
        /// <summary>
        /// Max performance: CPU will be running at max clockspeed
        /// </summary>
        private readonly PowerOption MaxPerformance;

        /// <summary>
        /// Balanced: CPU will increase or decrease clockspeeds according to system load and requirements
        /// </summary>
        private readonly PowerOption Balanced;

        /// <summary>
        /// Optimized: CPU will be running at low clockspeeds to increase power savings
        /// </summary>
        private readonly PowerOption PowerSaving;

        public PowerManager()
        {
            this.MaxPerformance = this.CreatePowerOption("High performance", PowerOptions.MaxPerformance);
            this.PowerSaving = this.CreatePowerOption("Power saver", PowerOptions.PowerSaving);
            this.Balanced = this.CreatePowerOption("Balanced", PowerOptions.Balanced);
        }

        private PowerOption CreatePowerOption(string name, string guid)
        {
            return new PowerOption(name, new Guid(guid));
        }

        private PowerOption CreatePowerOption(string guid)
        {
            var g = new Guid(guid);

            return new PowerOption(this.GetPowerOptionName(g), g);
        }

        private string GetPowerOptionName(Guid guid)
        {
            string name = string.Empty;
            IntPtr buffer = (IntPtr)null;
            uint bufferSize = 0;

            PowerReadFriendlyName((IntPtr)null, ref guid, (IntPtr)null, (IntPtr)null, buffer, ref bufferSize);

            if(bufferSize > 0)
            {
                buffer = Marshal.AllocHGlobal((int)bufferSize);

                if(PowerReadFriendlyName((IntPtr)null, ref guid, (IntPtr)null, (IntPtr)null, buffer, ref bufferSize) == 0)
                {
                    name = Marshal.PtrToStringUni(buffer);
                }

                if(buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }

            return name;
        }

        public PowerOption GetCurrentPowerOption()
        {
            var guid = this.GetActiveGuid();

            return this.GetPowerOptions().Where(o => o.PowerId == guid).FirstOrDefault();
        }

        public List<PowerOption> GetPowerOptions()
        {
            return new List<PowerOption>()
            {
                MaxPerformance, 
                Balanced,
                PowerSaving
            };
        }

        public void SetPowerOption(PowerOption option)
        {
            PowerSetActiveScheme(IntPtr.Zero, ref option.PowerId);
        }

        private Guid GetActiveGuid()
        {
            var ActiveGuid = Guid.Empty;
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));

            if(PowerGetActiveScheme((IntPtr)null, out ptr) == 0)
            {
                ActiveGuid = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));

                if(ptr != null)
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }

            return ActiveGuid;
        }

        #region DLL imports for accessing power plan options

        [DllImport("powrprof.dll", EntryPoint = "PowerSetActiveScheme")]
        public static extern uint PowerSetActiveScheme(IntPtr UserPowerKey, ref Guid ActivePolicyGuid);

        [DllImport("powrprof.dll", EntryPoint = "PowerGetActiveScheme")]
        public static extern uint PowerGetActiveScheme(IntPtr UserPowerKey, out IntPtr ActivePolicyGuid);

        [DllImport("powrprof.dll", EntryPoint = "PowerReadFriendlyName")]
        public static extern uint PowerReadFriendlyName(IntPtr RootPowerKey, ref Guid SchemeGuid, IntPtr SubGroupOfPowerSettingsGuid, IntPtr PowerSettingGuid, IntPtr Buffer, ref uint BufferSize);

        #endregion
    }
}
