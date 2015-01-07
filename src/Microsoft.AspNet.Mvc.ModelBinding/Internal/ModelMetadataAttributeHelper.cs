using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.Mvc.ModelBinding.Internal
{
    internal class ModelMetadataAttributeHelper
    {
        internal IEnumerable<Attribute> GetModelMetadataAttributesOnType(Type type, IEnumerable<Attribute> attributes)
        {
            var modelMedatadataType = type.GetTypeInfo().GetCustomAttribute<ModelMetadataTypeAttribute>();
            if (modelMedatadataType != null)
            {
                var modelMedatadataAttributes = modelMedatadataType.MetadataBuddyType.GetTypeInfo().GetCustomAttributes();

                //If same attribute exists in both attributes and modelMedatadataAttributes collection, 
                //pick from attributes collection(view model)
                attributes = attributes.Concat(modelMedatadataAttributes).GroupBy(a => a.GetType())
                          .Select(grp => grp.First());
            }

            return attributes;
        }

        internal IEnumerable<object> GetModelMetadataAttributesOnProperty(Type type, IEnumerable<object> attributes, string propertyName)
        {
            var modelMedatadataType = type.GetTypeInfo().GetCustomAttribute<ModelMetadataTypeAttribute>();
            if (modelMedatadataType != null)
            {
                var modelMedatadataProperty = modelMedatadataType.MetadataBuddyType.GetTypeInfo().GetDeclaredProperty(propertyName);
                if (modelMedatadataProperty != null)
                {
                    var modelMedatadataAttributes = modelMedatadataProperty.GetCustomAttributes();

                    //If same attribute exists in both attributes and modelMedatadataAttributes collection, 
                    //pick from attributes collection(view model)
                    attributes = attributes.Concat(modelMedatadataAttributes).GroupBy(a => a.GetType())
                      .Select(grp => grp.First());
                }
            }
            return attributes;
        }
    }
}