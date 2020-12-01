﻿using PowerSwitcher.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

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

        private void CustomizeContextMenuStrip()
        {
            this.trayIcon.ContextMenuStrip.RenderMode = ToolStripRenderMode.Professional;
        }

        private void InitializeMenuItems()
        {
            var cOpt = this.manager.GetCurrentPowerOption();

            foreach (var option in this.PowerOptions)
            {
                bool IsActive = option.PowerId == cOpt.PowerId;

                var item = this.CreateMenuItem(option, IsActive);

                this.trayIcon.ContextMenuStrip.Items.Add(item);
            }

            // Item separator
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

            // Exit button
            trayIcon.ContextMenuStrip.Items.Add("Exit", null, Exit);
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