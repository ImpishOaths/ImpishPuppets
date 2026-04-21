using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
public partial class RemoteController: Node
{
    [Export]
    private string ControllerPath;
    private PuppetController Controller;

    public void SetReceiver(Node puppet)
    {
        if(puppet != null)
            Controller = puppet.GetNodeOrNull<PuppetController>(ControllerPath);
    }

    public override Array<Dictionary> _GetPropertyList()
    {
        if(Controller == null)
            return [];
        return Controller.ControlPropertyList();
    }

    public override Variant _Get(StringName property)
    {
        if(Controller == null)
            return default;
        return Controller.ControlGet(property);
    }

    public override bool _Set(StringName property, Variant value)
    {
        if(Controller == null)
            return false;
        return Controller.ControlSet(property, value);
    }
}
