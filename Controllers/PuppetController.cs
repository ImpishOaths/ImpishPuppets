using Godot;
using Godot.Collections;
using System;

namespace ImpishPuppets;

public abstract partial class PuppetController: Node, Puppet2Dto3DConverter
{
    public abstract void Initialize();
    public virtual Node ConvertTo3D(Puppet3D puppet)
    {
        return Duplicate();
    }
    
    public abstract Array<Dictionary> ControlPropertyList();
    public abstract bool ControlSet(StringName property, Variant variant);
    public abstract Variant ControlGet(StringName property);

}
