Quark Framework
====

Quark is a framework focused on building role playing games using modular structures.

For example, a `Spell` that deals 100 damage to nearest player is defined like this:
```csharp
public class BoomSpell : Spell
{
	public override TargetMacro TargetMacro {
		get {
			return new NearestCharacter();
		}
	}

	protected override Effect[] TargetingDoneEffects {
		get {
			return new Effect[] {
				new DamageEffect (100)
			};
		}
	}
}
```

Features
----
Currently, the following list is implemented and running in Quark:

+ Casted Activities (`Spells`)
+ Time or Event Driven Activities (`Buffs`)
+ Targeting (`TargetMacros`)
+ Casting (`CastData`)
+ Object Mutation (`Effects`)
+ Quark Controlled Objects (`Characters` and `Targetables`)

But there are more features coming.

Installation / Using
----
There is no actual installation, you can simply build the project and include `Quark.dll` as an asset in your Quark game, or just run the to-be-provided `Quark.unityasset` file.

For usage part, if you used the Unity Asset way, it is easy. Simply drag the `Quark.Head` prefab to your hierarchy and your scene is ready to use Quark. 

If you included the dll, you should create a `Quark.Head` object. For details, please refer to [this article][1]

Contribute
----
Source and issues of Quark are held in [this GitHub repository][2].

Support
----
If you need help, you can use the GitHub issues or [send me an email][3]

License
----
Quark is licensed under [Apache V2][4] License.

[1]: Usage.md
[2]: https://github.com/FatihBAKIR/Quark
[3]: mailto:fatih@linux.com
[4]: http://www.apache.org/licenses/LICENSE-2.0
