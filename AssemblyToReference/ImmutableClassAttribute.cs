using System;

namespace ImmutableClass
{
    /// <summary>
    /// Specifies that this class stub will be extended into a fully usable immutable class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ImmutableClassAttribute : Attribute
    {
    }
}
