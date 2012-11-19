using System;

namespace UsingCVDWithoutMap.FeatureService
{
    /// <summary>
    /// Event arguments for failed event.
    /// </summary>
    public class FailedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the occurred <see cref="Exception"/>.
        /// </summary>
        public Exception Error { get; set; }
    }
}
