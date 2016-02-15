using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class ModuleWeaver
{
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public Action<string, SequencePoint> LogErrorPoint { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }

    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogWarning = s => { };
        LogErrorPoint = (x, y) => { };
    }

    public void Execute()
    {

        // Find all classes marked with the ImmutableClass attribute
        new ProcessImmutableClasses(ModuleDefinition, LogInfo).Execute();

        // Clean away the reference to the weaver from every project
        new ReferenceCleaner(ModuleDefinition, LogInfo).Execute();
    }
}
