using System;
using System.Collections.Generic;
using System.Linq;

namespace Thindal.Blazor.BindableParameter;

[System.AttributeUsage(AttributeTargets.Property)]
public class BindableParameterAttribute  : Attribute
{
    internal List<string> ChangedMethods { get; set; }
    internal bool CallSelf { get; set; }
    /*public BindableParameterAttribute()
    {
        
    }*/

    // TODO: Add ability to call more than one Changed method
    public BindableParameterAttribute(bool callSelf = true, params string[] changedMethods)
    {
        CallSelf = callSelf;

        if (changedMethods == null)
        {
            changedMethods = new string[1];
        }
        ChangedMethods = changedMethods.ToList(); // Call the Changed method on all of these properties when the property changes
    }
}
