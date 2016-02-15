using System;
using System.Linq;
using Mono.Cecil;

public class ReferenceCleaner
{
    ModuleDefinition moduleDefinition;
    Action<string> logInfo;

    public ReferenceCleaner(ModuleDefinition moduleDefinition, Action<string> logInfo )
    {
        this.moduleDefinition = moduleDefinition;
        this.logInfo = logInfo;
    }

    public void Execute()
    {
        var referenceToRemove = moduleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "ImmutableClass");
        if (referenceToRemove == null)
        {
            logInfo("\tNo reference to 'ImmutableClass.dll' found. References not modified.");
            return;
        }

        moduleDefinition.AssemblyReferences.Remove(referenceToRemove);
        logInfo("\tRemoving reference to 'ImmutableClass.dll'.");
    }
}
