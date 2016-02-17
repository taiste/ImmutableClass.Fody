using System;

namespace ImmutableClass
{
    /// <summary>
    /// Specifies that this property will be ignored when processing the class as an immutable class
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class ImmutableClassIgnoreAttribute : Attribute
    {
    }
}
