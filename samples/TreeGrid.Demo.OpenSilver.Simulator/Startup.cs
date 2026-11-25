using System;
using OpenSilver.Simulator;

namespace TreeGrid.Demo.OpenSilver.Simulator
{
    internal static class Startup
    {
        [STAThread]
        private static int Main()
        {
            return SimulatorLauncher.Start<TreeGrid.Demo.App>();
        }
    }
}
