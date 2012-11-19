namespace UsingCVDWithoutMap.FeatureService
{
    using System;

    using ESRI.ArcGIS.Client;

    /// <summary>
    /// Event arguments for <see cref="Graphic"/>s.
    /// </summary>
    public class GraphicsEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the <see cref="Graphic"/>s.
        /// </summary>
        public GraphicCollection Graphics { get; set; }
    }
}
