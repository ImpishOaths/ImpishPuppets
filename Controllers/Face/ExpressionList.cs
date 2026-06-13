using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class ExpressionList: Resource
{
    [Export]
    public Dictionary<StringName, Expression> Expressions;
}
