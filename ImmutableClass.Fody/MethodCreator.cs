using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public class MethodCreator
{
    private Action<string> _logInfo;
    private ModuleDefinition _moduleDefinition;

    public MethodCreator(ModuleDefinition moduleDefinition, Action<string> logInfo)
    {
        _logInfo = logInfo;
        _moduleDefinition = moduleDefinition;
    }

    /// <summary>
    /// Creates the default constructor.
    /// </summary>
    public MethodDefinition CreateDefaultConstructor(TypeDefinition type)
    {
        _logInfo("AddDefaultConstructor() " + type.Name);

        // Create method for a constructor
        var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        var method = new MethodDefinition(".ctor", methodAttributes, _moduleDefinition.TypeSystem.Void);

        // Add each property in the class as a parameter for the constructor
        foreach (var prop in type.Properties)
        {
            string paramName = Char.ToLowerInvariant(prop.Name [0]) + prop.Name.Substring (1); // convert first letter of name to lowercase
            _logInfo("adding parameter " + paramName + " to ctor");
            method.Parameters.Add(new ParameterDefinition(paramName, ParameterAttributes.None, prop.PropertyType));
        }

        // Add body
        // Set the properties in the class from the constructor parameters
        for (int i = 0; i < type.Properties.Count; i++) {
            _logInfo("setting property " + type.Properties[i].Name);
            method.Body.Instructions.Add (Instruction.Create (OpCodes.Ldarg_0));
            method.Body.Instructions.Add (Instruction.Create (OpCodes.Ldarg, method.Parameters[i]));
            method.Body.Instructions.Add (Instruction.Create (OpCodes.Call, type.Properties [i].SetMethod));
        }
        method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        return method;
    }

    /// <summary>
    /// Finds the empty constructor and removes it
    /// </summary>
    public void DestroyEmptyConstructor(TypeDefinition type)
    {
        var ctor = type.Methods.First (md => md.Name == ".ctor" && md.Parameters.Count == 0);
        if (ctor != null) {
            _logInfo ("DestroyEmptyConstructor() " + ctor.Name + ":" + ctor.Parameters.Count);
            type.Methods.Remove (ctor);
        }
    }

    /// <summary>
    /// Creates a 'With' method for the given property
    /// The 'With' method creates a copy of the current instance, with only the given property changing
    /// </summary>
    /// <returns>The with method for property.</returns>
    public MethodDefinition CreateWithMethodForProperty(TypeDefinition type, PropertyDefinition replacementProp)
    {
        _logInfo("AddMethodUsingProperty() " + replacementProp.Name);
        string paramName = Char.ToLowerInvariant(replacementProp.Name [0]) + replacementProp.Name.Substring (1); // convert first letter of name to lowercase
        var methodAttributes = MethodAttributes.Public;

        // Create method
        var method = new MethodDefinition("With" + replacementProp.Name, methodAttributes, type);
        // Only one parameter: the field to copy to the new instance
        method.Parameters.Add(new ParameterDefinition(paramName, ParameterAttributes.None, replacementProp.PropertyType));

        // Create body -- just a call to the default constructor with all properties copied from this instance except the single one given as parameter

        // Push onto the stack values of all properties from the current instance, to be used as parameters to the constructor
        // The 'with' property is replaced with the parameter used in the method call
        foreach(PropertyDefinition existingProp in type.Properties)
        {
            if (existingProp.Name.ToLower().Equals(replacementProp.Name.ToLower())) {
                _logInfo("Replacing property " + existingProp.Name + " with parameter");
                method.Body.Instructions.Add (Instruction.Create (OpCodes.Ldarg, method.Parameters[0]));
            }
            else
            {
                _logInfo("Copying property " + existingProp.Name + " from current instance");
                method.Body.Instructions.Add (Instruction.Create (OpCodes.Ldarg_0));
                method.Body.Instructions.Add (Instruction.Create (OpCodes.Call, existingProp.GetMethod));
            }
        }
        var ctor = type.Methods.First (md => md.Name == ".ctor" && md.Parameters.Count == type.Properties.Count); 
        method.Body.Instructions.Add(Instruction.Create(OpCodes.Newobj, ctor));
        method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        return method;
    }

    /*
    MethodDefinition CreateEmptyConstructor(TypeDefinition type)
    {
        LogInfo("AddEmptyConstructor() " + type.Name);
        var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        var method = new MethodDefinition(".ctor", methodAttributes, ModuleDefinition.TypeSystem.Void);
        method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        var methodBaseReference = new MethodReference(".ctor", ModuleDefinition.TypeSystem.Void,type.BaseType){HasThis = true};
        method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, methodBaseReference));
        method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        type.Methods.Add(method);
        return method;
    }
    */
}
