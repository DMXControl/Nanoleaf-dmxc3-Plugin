using Lumos.GUI.Plugin;
using Lumos.GUI.Resource;
using Lumos.GUI.Settings;
using Lumos.GUI.Settings.PE;
using Lumos.GUI.Windows;
using Lumos.GUI.Windows.ProjectExplorer;
using LumosLIB.Kernel.Log;
using LumosProtobuf.Resource;
using org.dmxc.lumos.Kernel.Resource;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace NanoleafGUI_Plugin
{
    public class NanoleafGUI_Plugin : GuiPluginBase, IResourceProvider
    {
        internal static readonly ILumosLog Log = LumosLogger.getInstance(nameof(NanoleafGUI_Plugin));
        private const string SETTINGS_CATEGORY_ID = "Settings:Nanoleaf";

        internal const string NANOLEAF_SHOW_IN_INPUTASSIGNMENT = "NANOLEAF.SHOW_IN_INPUTASSIGNMENT";
        internal const string NANOLEAF_DISCOVER = "NANOLEAF.DISCOVER";
        internal const string NANOLEAF_AUTOREQUEST_TOKEN = "NANOLEAF.AUTOREQUEST_TOKEN";
        internal const string NANOLEAF_AUTOCONNECT = "NANOLEAF.AUTOCONNECT";
        internal const string NANOLEAF_REFRESH_RATE = "NANOLEAF.REFRESH_RATE";

        internal const string NANOLEAF_DISCOVER_STATE = "NANOLEAF.DISCOVER_STATE";
        internal const string NANOLEAF_DISCOVERED_CONTROLLERS = "NANOLEAF.DISCOVERED_CONTROLLERS";
        internal const string NANOLEAF_CONTROLLERS = "NANOLEAF.CONTROLLERS";

        internal const string NANOLEAF_REQUEST_TOKEN = "NANOLEAF.REQUEST_TOKEN";
        internal const string NANOLEAF_ADD_CONTROLLER = "NANOLEAF.ADD_CONTROLLER";
        internal const string NANOLEAF_REMOVE_CONTROLLER = "NANOLEAF.REMOVE_CONTROLLER";

        private SettingsBranch settingsBranch;
        public NanoleafGUI_Plugin(): base("295c2ab8-4d2b-4741-a1bd-16de8f4da957", "NanoleafGUI-Plugin")
        {
            //while (!Debugger.IsAttached)
            //{
            //    Thread.Sleep(3000);
            //}
        }
        protected override void initializePlugin()
        {
            try
            {
                ResourceManager.getInstance().registerResourceProvider(this);
                this.settingsBranch = (SettingsBranch)PEManager.getInstance().GetBranchByID("Settings");

                var nanoleafNode = new FormSettingsNode(SETTINGS_CATEGORY_ID, "Nanoleaf", "Nanoleaf");
                nanoleafNode.DisplayWhere = EDisplayCategory.APPLICATION_SETTINGS;
                nanoleafNode.PropertiesForm = typeof(NanoleafSettingsForm);
                settingsBranch.AddRecursive(settingsBranch.ID, nanoleafNode);
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }
        protected override void shutdownPlugin()
        {
        }
        protected override void startupPlugin()
        {
        }
        public override void connectionEstablished()
        {
            base.connectionEstablished();
            SettingsManager sm = SettingsManager.getInstance();
        }
        public override void connectionClosing()
        {
            base.connectionClosing();
        }

        public bool existsResource(EResourceDataType type, string name)
        {
            if (type == EResourceDataType.Symbol)
            {
                if (name.Equals("Nanoleaf")
                    || name.Equals("Nanoleaf_16")
                    || name.Equals("Nanoleaf_32")
                    || name.Equals("Nanoleaf_64")
                    || name.Equals("Nanoleaf_128"))
                    return true;
            }
            return false;
        }
        IReadOnlyList<LumosDataMetadata> IResourceProvider.allResources(EResourceDataType type)
        {
            if (type == EResourceDataType.Symbol)
            {
                List<LumosDataMetadata> ret = new List<LumosDataMetadata>()
                {
                    new LumosDataMetadata("Nanoleaf"),
                    new LumosDataMetadata("Nanoleaf_16"),
                    new LumosDataMetadata("Nanoleaf_32"),
                    new LumosDataMetadata("Nanoleaf_64"),
                    new LumosDataMetadata("Nanoleaf_128"),
                };
                return ret.AsReadOnly();
            }

            return null;
        }
        public Stream loadResource(EResourceDataType type, string name)
        {
            if (type == EResourceDataType.Symbol)
            {
                switch (name)
                {
                    case "Nanoleaf":
                    case "Nanoleaf_32":
                        return toByteArray(Properties.Resources.Nanoleaf_32);

                    case "Nanoleaf_16":
                        return toByteArray(Properties.Resources.Nanoleaf_16);

                    case "Nanoleaf_64":
                        return toByteArray(Properties.Resources.Nanoleaf_64);

                    case "Nanoleaf_128":
                        return toByteArray(Properties.Resources.Nanoleaf_128);
                }
            }

            return null;
        }
        private Stream toByteArray(Bitmap i)
        {
            var m = new System.IO.MemoryStream();
            if (i != null)
            {

                i.Save(m, ImageFormat.Png);
                m.Position = 0;
                return m;
            }
            return null;
        }
    }
}
