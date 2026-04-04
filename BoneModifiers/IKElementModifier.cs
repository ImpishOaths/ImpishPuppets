using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class IKElementModifier: PuppetBoneModifier
{
    [Export]
    public float Length = 10;
    [Export]
    public float AngleNudge;
    [Export]
    public Vector2 TargetOffset;
    [Export]
    public float LockAngleSign = 0f;

    public override void Apply(float delta) {}

    public override PuppetBoneModifier Make3DDuplicate(Vector2 resize)
    {
        var duplicate = Duplicate() as IKElementModifier;
        duplicate.Length *= Mathf.Min(resize.X,resize.Y);
        return duplicate;
    }

}
