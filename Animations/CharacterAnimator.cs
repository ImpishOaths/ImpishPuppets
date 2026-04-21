using Godot;
using System;

[Tool]
public partial class CharacterAnimator: Node
{
    [Export]
    public AnimationLibrary FullLibrary;
    [Export]
    public AnimationLibrary UpperLibrary;
    [Export]
    public AnimationLibrary LowerLibrary;
    [Export]
    public AnimationLibrary FaceLibrary;
    
    private AnimationPlayer Full;
    private AnimationPlayer Upper;
    private AnimationPlayer Lower;
    private AnimationPlayer Face;

    public override void _Ready()
    {
        Full = GetNode<AnimationPlayer>("Full");
        Upper = GetNode<AnimationPlayer>("Upper");
        Lower = GetNode<AnimationPlayer>("Lower");
        Face = GetNode<AnimationPlayer>("Face");
    }

    [ExportToolButton("Split Full")]
    public Callable SplitFullCallable => Callable.From(SplitFull);
    private void SplitFull()
    {
        
    }

}
