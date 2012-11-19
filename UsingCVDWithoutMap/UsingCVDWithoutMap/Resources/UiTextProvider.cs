namespace UsingCVDWithoutMap.Resources
{
    /// <summary>
    /// Used to proxy resources to Views.
    /// </summary>
    /// <remarks>
    /// Used via DataBinding.
    /// </remarks>
    public class UiTextProvider
    {
        private static readonly UiTexts uiTexts = new UiTexts();

        /// <summary>
        /// Gets ui text strings defined in .resx.
        /// </summary>
        public UiTexts UiTexts
        {
            get
            {
                return uiTexts;
            }
        }
    }
}
