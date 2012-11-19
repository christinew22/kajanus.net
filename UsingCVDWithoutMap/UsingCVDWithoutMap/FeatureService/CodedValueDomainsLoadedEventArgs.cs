namespace UsingCVDWithoutMap.FeatureService
{
    using System;
    using Caliburn.Micro;

    using ESRI.ArcGIS.Client.FeatureService;

    /// <summary>
    /// Event args for <see cref="CodedValueDomain"/>s loaded from the Service REST endpoint.
    /// </summary>
    public class CodedValueDomainsLoadedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the loaded <see cref="CodedValueDomain"/>s.
        /// </summary>
        public BindableCollection<CodedValueDomain> Domains { get; set; } 
    }
}
