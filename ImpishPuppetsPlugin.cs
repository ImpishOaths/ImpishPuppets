#if TOOLS
using Godot;
using System;

namespace ImpishPuppets;

[Tool]
public partial class ImpishPuppetsPlugin : EditorPlugin
{
	private Texture2D Puppet2DIcon = ResourceLoader.Load<Texture2D>("res://addons/ImpishPuppets/Icons/Puppet2DIcon.png","Image");
	private Texture2D Transform2DIcon = ResourceLoader.Load<Texture2D>("res://addons/ImpishPuppets/Icons/Transform2DIcon.png","Image");
	private Texture2D Bone2DIcon = ResourceLoader.Load<Texture2D>("res://addons/ImpishPuppets/Icons/Bone2DIcon.png","Image");

	private Texture2D Puppet3DIcon = ResourceLoader.Load<Texture2D>("res://addons/ImpishPuppets/Icons/Puppet3DIcon.png","Image");
	private Texture2D Transform3DIcon = ResourceLoader.Load<Texture2D>("res://addons/ImpishPuppets/Icons/Transform3DIcon.png","Image");
	private Texture2D Bone3DIcon = ResourceLoader.Load<Texture2D>("res://addons/ImpishPuppets/Icons/Bone3DIcon.png","Image");
	
	private Script Puppet2DScript = ResourceLoader.Load<Script>("res://addons/ImpishPuppets/Puppet2D/Puppet2D.cs", "Script");
	private Script Transform2DScript = ResourceLoader.Load<Script>("res://addons/ImpishPuppets/Puppet2D/PuppetTransform2D.cs", "Script");
	private Script Bone2DScript = ResourceLoader.Load<Script>("res://addons/ImpishPuppets/Puppet2D/PuppetBone2D.cs", "Script");

	private Script Puppet3DScript = ResourceLoader.Load<Script>("res://addons/ImpishPuppets/Puppet3D/Puppet3D.cs", "Script");
	private Script Transform3DScript = ResourceLoader.Load<Script>("res://addons/ImpishPuppets/Puppet3D/PuppetTransform3D.cs", "Script");
	private Script Bone3DScript = ResourceLoader.Load<Script>("res://addons/ImpishPuppets/Puppet3D/PuppetBone3D.cs", "Script");

	public override void _EnterTree()
	{	
		AddCustomType("Puppet2D", "Node2D", Puppet2DScript, Puppet2DIcon);
		AddCustomType("PuppetBone2D", "Node2D", Bone2DScript, Bone2DIcon);
		AddCustomType("PuppetTransform2D", "Node2D", Transform2DScript, Transform2DIcon);

		AddCustomType("Puppet3D", "Node3D", Puppet3DScript, Puppet3DIcon);
		AddCustomType("PuppetBone3D", "Node3D", Bone3DScript, Bone3DIcon);
		AddCustomType("PuppetTransform3D", "Node3D", Transform3DScript, Transform3DIcon);
	}

	public override void _ExitTree()
	{
		RemoveCustomType("Puppet2D");
		RemoveCustomType("PuppetBone2D");
		RemoveCustomType("PuppetTransform2D");

		RemoveCustomType("Puppet3D");
		RemoveCustomType("PuppetBone3D");
		RemoveCustomType("PuppetTransform3D");
	}
}
#endif
