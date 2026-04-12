using Godot;
using Godot.Collections;
using System;

namespace ImpishPuppets;

public abstract partial class PuppetController: Node
{
    public abstract void Initialize();
    public abstract PuppetController MakeDuplicate3D();

    public abstract Array<Dictionary> ControlPropertyList();
    public abstract bool ControlSet(StringName property, Variant variant);
    public abstract Variant ControlGet(StringName property);
}
