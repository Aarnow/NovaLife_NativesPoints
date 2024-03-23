using Life;
using ModKit.Helper;
using ModKit.Interfaces;
using ModKit.Internal;

namespace NativePoints
{
    public class NativePoints : ModKit.ModKit
    {

        public NativePoints(IGameAPI api) : base(api)
        {
            PluginInformations = new PluginInformations(AssemblyHelper.GetName(), "1.0.0", "Aarnow");
        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();
            InitDirectoryAndFiles();
            Logger.LogSuccess($"{PluginInformations.SourceName} v{PluginInformations.Version}", "initialisé");
        }

        private void InitDirectoryAndFiles()
        {
        }
    }
}
