#if TOOLS
using Godot;
using System;

namespace ImpishPuppets;

[Tool]
public partial class ImpishPuppetsPlugin : EditorPlugin
{
	private Texture2D Puppet2DIcon = ResourceLoader.Load<Texture2D>("res://addons/ImpishPuppets/Icons/Puppet2DIcon.png","Image");
	private Texture2D Control2DIcon = ResourceLoader.Load<Texture2D>("res://addons/ImpishPuppets/Icons/Control2DIcon.png","Image");
	private Texture2D Bone2DIcon = ResourceLoader.Load<Texture2D>("res://addons/ImpishPuppets/Icons/Bone2DIcon.png","Image");

	private Script Puppet2DScript = ResourceLoader.Load<Script>("res://addons/ImpishPuppets/Puppet2D/Puppet2D.cs", "Script");
	private Script Control2DScript = ResourceLoader.Load<Script>("res://addons/ImpishPuppets/Puppet2D/Puppet2DControl.cs", "Script");
	private Script Bone2DScript = ResourceLoader.Load<Script>("res://addons/ImpishPuppets/Puppet2D/Puppet2DBone.cs", "Script");

	public override void _EnterTree()
	{	
		AddCustomType("Puppet2D", "Node2D", Puppet2DScript, Puppet2DIcon);
		AddCustomType("Puppet2DControl", "Node2D", Control2DScript, Control2DIcon);
		AddCustomType("Puppet2DBone", "Node2D", Bone2DScript, Bone2DIcon);
	}

	public override void _ExitTree()
	{
		RemoveCustomType("Puppet2D");
		RemoveCustomType("Puppet2DBone2D");
		RemoveCustomType("Puppet2DControl");
	}
}
#endif
