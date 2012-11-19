namespace UsingCVDWithoutMap.FeatureService
{
    using System.Collections.Generic;
    using System.Linq;

    using ESRI.ArcGIS.Client.FeatureService;

    /// <summary>
    /// Provider for CodedValueDomain in a static context. 
    /// </summary>
    /// <remarks>
    /// This class is used in the converters etc. to provide access to loaded domain information.
    /// </remarks>
    public class CodedValueDomainsProvider
    {
        private static readonly List<CodedValueDomain> domains = new List<CodedValueDomain>();

        /// <summary>
        /// Gets <see cref="CodedValueDomain"/> by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns the <see cref="CodedValueDomain"/> if there is an entry with given name. Retuns <c>Null</c> if no entry was found.</returns>
        public static CodedValueDomain GetCodedValueDomainsByName(string name)
        {
            return domains.FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        /// Adds <see cref="CodedValueDomain"/> to the provider.
        /// </summary>
        /// <param name="domain">The <see cref="CodedValueDomain"/></param>
        public static void AddCodedValueDomain(CodedValueDomain domain)
        {
            if (domains.Any(x => x.Equals(domain)) || domains.Any(x => x.Name == domain.Name))
            {
                // CVD is already set into the provider.
                return;
            }

            domains.Add(domain);
        }
    }
}
