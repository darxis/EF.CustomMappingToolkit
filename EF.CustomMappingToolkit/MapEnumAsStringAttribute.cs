using System;

namespace EF.CustomMappingToolkit
{
    public class MapEnumAsStringAttribute : Attribute
    {
        public string StringPropertyName { get; }

        public MapEnumAsStringAttribute(string stringProperty)
        {
            this.StringPropertyName = stringProperty;
        }
    }
}
