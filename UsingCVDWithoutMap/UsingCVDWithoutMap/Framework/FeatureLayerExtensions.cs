using System;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.FeatureService;

namespace UsingCVDWithoutMap.Framework
{
    public static class FeatureLayerExtensions
    {
        /// <summary>
        /// Gets <see cref="CodedValueDomain"/>s from the target layer.
        /// </summary>
        /// <param name="layer">The target <see cref="FeatureLayer"/>.</param>
        /// <returns>Retuns a list of <see cref="CodedValueDomain"/>s that are defined in the layer definitions.</returns>
        public static List<CodedValueDomain> GetCodedValueDomains(this FeatureLayer layer)
        {
            if (layer.IsInitialized == false)
            {
                throw new Exception("Layer is not initialized");
            }

            // Get all fields that contains CVD's
            var fields = layer.LayerInfo.Fields.Where(x => x.Domain != null);

            // Return all CodedValueDomains
            return fields.Select(field => field.Domain).OfType<CodedValueDomain>().ToList();
        }

        /// <summary>
        /// Gets <see cref="CodedValueDomain"/> by domain name.
        /// </summary>
        /// <param name="layer">The target <see cref="FeatureLayer"/>.</param>
        /// <param name="domainName">The name of the domain</param>
        /// <returns>Return <see cref="CodedValueDomain"/> that exists in the target layer. Null if doesn't have the domain with the given domainName.</returns>
        public static CodedValueDomain GetDomainByName(this FeatureLayer layer, string domainName)
        {
            if (layer.IsInitialized == false)
            {
                throw new Exception("Layer is not initialized");
            }

            // Load all CVD's to domains list
            return layer.GetCodedValueDomains().FirstOrDefault(x => x.Name == domainName);
        }

        /// <summary>
        ///  Gets alias name from the <see cref="Field"/> by fields name.
        /// </summary>
        /// <param name="layer">The target <see cref="FeatureLayer"/>.</param>
        /// <param name="fieldName">The field name.</param>
        /// <returns>The alias name.</returns>
        public static string GetFieldAliasName(this FeatureLayer layer, string fieldName)
        {
            if (layer.IsInitialized == false)
            {
                throw new Exception("Layer is not initialized");
            }

            return layer.LayerInfo.Fields.FirstOrDefault(x => x.FieldName == fieldName).Alias;
        }

    }
}
