using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class ProcessImmutableClasses
{
    List<TypeDefinition> allTypes;
    public Action<string> _logInfo;
    public ModuleDefinition _moduleDefinition;

    public ProcessImmutableClasses(ModuleDefinition moduleDefinition, Action<string> logInfo)
    {
        _logInfo = logInfo;
        _moduleDefinition = moduleDefinition;
    }

    public void Execute()
    {
        allTypes = _moduleDefinition.GetTypes().ToList();
        foreach (var type in allTypes)
        {
            if (type.IsInterface)
            {
                continue;
            }
            if (type.IsEnum)
            {
                continue;
            }
            if (!type.CustomAttributes.ContainsAttribute("ImmutableClassAttribute"))
            {
                continue;
            }
            ProcessClass(type);
        }
    }

    private void ProcessClass(TypeDefinition type)
    {
        _logInfo("\tProcess immutable class:" + type.FullName);

        // Remove [ImmutableClass] attribute from the class
        var customAttribute = type.CustomAttributes.First(x => x.AttributeType.Name == "ImmutableClassAttribute");
        type.CustomAttributes.Remove(customAttribute);

        // Add a default constructor having parameters for each property in the class
        MethodCreator mc = new MethodCreator(_moduleDefinition, _logInfo);
        type.Methods.Add(mc.CreateDefaultConstructor(type));

        // For each property:
        // 1. If the property has a set method, set it as private
        // 2. Create a 'with' method, so that a copy of the class can be created with only the value of that single property changed
        // Properties marked with the [ImmutableClassIgnore] attribute are ignored
        foreach (var prop in type.Properties)
        {
            if (prop.CustomAttributes.ContainsAttribute("ImmutableClassIgnoreAttribute"))
            {
                _logInfo("Ignoring property " + prop.Name);
                continue;
            }
            if (prop.SetMethod != null)
            {
                prop.SetMethod.IsPrivate = true;
                prop.SetMethod.IsPublic = false;
            }
            type.Methods.Add(mc.CreateWithMethodForProperty(type, prop));
        }

        // Destroy the empty constructor if it exists
        mc.DestroyEmptyConstructor(type);

        // Remove [ImmutableClassIgnore] attribute from the properties
        foreach (var prop in type.Properties)
        {
            var ignoreProp = prop.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "ImmutableClassIgnoreAttribute");
            if (ignoreProp != null)
            {
                {
                    _logInfo("Removing [ImmutableClassIgnore] attribute from property " + prop.Name);
                    prop.CustomAttributes.Remove(ignoreProp);
                }
            }
        }
    }
}


