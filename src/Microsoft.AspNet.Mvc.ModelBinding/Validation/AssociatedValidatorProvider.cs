// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc.ModelBinding.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.Mvc.ModelBinding
{
    public abstract class AssociatedValidatorProvider : IModelValidatorProvider
    {
        private ModelMetadataAttributeHelper modelMetadataAttrHelper = new ModelMetadataAttributeHelper();

        public IEnumerable<IModelValidator> GetValidators([NotNull] ModelMetadata metadata)
        {
            if (metadata.ContainerType != null && !string.IsNullOrEmpty(metadata.PropertyName))
            {
                return GetValidatorsForProperty(metadata);
            }

            return GetValidatorsForType(metadata);
        }

        protected abstract IEnumerable<IModelValidator> GetValidators(ModelMetadata metadata,
                                                                      IEnumerable<Attribute> attributes);

        private IEnumerable<IModelValidator> GetValidatorsForProperty(ModelMetadata metadata)
        {
            var propertyName = metadata.PropertyName;
            var bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
            var property = metadata.ContainerType
                                   .GetProperty(propertyName, bindingFlags);

            if (property == null)
            {
                throw new ArgumentException(
                    Resources.FormatCommon_PropertyNotFound(
                        metadata.ContainerType.FullName,
                        metadata.PropertyName),
                    "metadata");
            }

            var attributes = property.GetCustomAttributes().Cast<object>();
            attributes = modelMetadataAttrHelper.GetModelMetadataAttributesOnProperty(metadata.ContainerType, attributes, property.Name);

            return GetValidators(metadata, attributes.Cast<Attribute>());
        }

        private IEnumerable<IModelValidator> GetValidatorsForType(ModelMetadata metadata)
        {
            var attributes = metadata.ModelType
                                     .GetTypeInfo()
                                     .GetCustomAttributes();
            attributes = modelMetadataAttrHelper.GetModelMetadataAttributesOnType(metadata.ModelType, attributes);

            return GetValidators(metadata, attributes);
        }
    }
}
