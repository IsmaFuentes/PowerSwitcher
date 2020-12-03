using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using PowerSwitcher.Implementation;
using PowerSwitcher.Rendering;
using PowerSwitcher.Utils;

namespace PowerSwitcher
{
    class TrayAppContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private PowerManager manager;
        private List<PowerOption> PowerOptions;

        public TrayAppContext()
        {
            this.manager = new PowerManager();

            // App configuration
            InitializeAppContext();
            InitializeMenuItems();
            CustomizeContextMenuStrip();
        }

        private void InitializeAppContext()
        {
            trayIcon = new NotifyIcon()
            {
                Text = AppVariables.ApplicationName,
                Icon = Properties.Resources.powerswitcher,
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip(),
            };

            this.PowerOptions = manager.GetPowerOptions().Where(p => p.Name != string.Empty).ToList();

            trayIcon.ContextMenuStrip.Opening += OnContextMenuStripOpening;
        }

        #region ContextMenuStrip
        private void InitializeMenuItems()
        {
            var cOpt = this.manager.GetCurrentPowerOption();

            foreach (var option in this.PowerOptions)
            {
                bool IsActive = option.PowerId == cOpt.PowerId;

                var item = this.CreateMenuItem(option, IsActive);

                this.trayIcon.ContextMenuStrip.Items.Add(item);
            }

            trayIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            
            // options menu + exit button
            trayIcon.ContextMenuStrip.Items.Add(this.CreateOptionsMenu());
            trayIcon.ContextMenuStrip.Items.Add("Exit", null, Exit);
        }

        private ToolStripMenuItem CreateOptionsMenu()
        {
            var options = new ToolStripMenuItem("Options");

            var register = new ToolStripMenuItem("Register on startup");
            register.Click += delegate (object sender, EventArgs args)
            {
                this.RegisterOnStartup();
            };

            var unregister = new ToolStripMenuItem("Unregister from startup");
            unregister.Click += delegate (object sender, EventArgs args)
            {
                this.UnRegisterOnStartup();
            };

            options.DropDownItems.AddRange(new ToolStripMenuItem[] { register, unregister });

            return options;
        }

        private ToolStripMenuItem CreateMenuItem(PowerOption aOpt, bool IsActive)
        {
            var item = new ToolStripMenuItem(aOpt.Name);

            item.Checked = IsActive;

            item.Click += delegate (object sender, EventArgs args)
            {
                this.manager.SetPowerOption(aOpt);
            };

            return item;
        }

        private void OnContextMenuStripOpening(object sender, CancelEventArgs e)
        {
            // Update selected power plan to checked = true
            int index = this.PowerOptions.IndexOf(manager.GetCurrentPowerOption());

            foreach(var item in trayIcon.ContextMenuStrip.Items)
            {
                if(item is ToolStripMenuItem)
                {
                    ((ToolStripMenuItem)item).Checked = false;
                }
            }

            ((ToolStripMenuItem)trayIcon.ContextMenuStrip.Items[index]).Checked = true;
        }

        private void CustomizeContextMenuStrip()
        {
            this.trayIcon.ContextMenuStrip.Renderer = new StyleRenderer();
        }

        #endregion

        #region App startup Registration / Unregistration

        private void RegisterOnStartup()
        {
            using(var helper = new RegistryHelper())
            {
                if (!helper.IsRegisteredOnStartup)
                {
                    helper.RegisterOnStartup(AppVariables.ApplicationName);
                }
            }
        }

        private void UnRegisterOnStartup()
        {
            using (var helper = new RegistryHelper())
            {
                if (helper.IsRegisteredOnStartup)
                {
                    helper.UnRegisterOnStartup(AppVariables.ApplicationName);
                }
            }
        }

        #endregion

        void Exit(object sender, EventArgs e)
        {
            // set icon visibility to false
            trayIcon.Visible = false;

            // unsubscribe from previous subscribed events
            trayIcon.ContextMenuStrip.Opening -= OnContextMenuStripOpening;

            // close app
            Application.Exit();
        }
    }
}
