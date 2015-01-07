using System;

namespace Microsoft.AspNet.Mvc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModelMetadataTypeAttribute : Attribute
    {
        private readonly Type _metadataBuddyType;

        public ModelMetadataTypeAttribute(Type buddyType)
        {
            _metadataBuddyType = buddyType;
        }

        public Type MetadataBuddyType
        {
            get { return _metadataBuddyType; }
        }
    }
}