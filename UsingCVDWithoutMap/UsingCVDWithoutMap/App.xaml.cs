namespace UsingCVDWithoutMap
{
    using System;
    using System.Diagnostics;

    public partial class App
    {
        public App()
        {
            Debug.WriteLine(string.Format("Starting load application at {0}", DateTime.Now));

            // Use debuggins information if run in Debug-mode
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
                // Display the current frame rate counters.
            //    Current.Host.Settings.EnableFrameRateCounter = true;
            //    Current.Host.Settings.EnableRedrawRegions = true;
            //}

            InitializeComponent();
            Debug.WriteLine(string.Format("Completed initialize base application at {0}", DateTime.Now));
        }
    }
}