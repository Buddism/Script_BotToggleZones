exec("./trigger.cs");
exec("./commands.cs");
exec("./support.cs");
exec("./package.cs");

$BTZ::SavePath = "config/server/BotToggleZones.cs";

//$BTZ::ZonePos[#]
//$BTZ::ZoneScale[#]
//$BTZ::ZoneObj[#]
//$BTZ::NumZones

function BTZ_load()
{
	BTZ_deleteAll();

	exec($BTZ::SavePath);

	for(%i = 0; %i < $BTZ::NumZones; %i++)
	{
		%pos = $BTZ::ZonePos[%i];
		%scale = $BTZ::ZoneScale[%i];

		BTZ_createZone(%i, %pos, %scale);
	}
}

function BTZ_save()
{
	//export variable,			SavePath,		append
	export("$BTZ::NumZones",	$BTZ::SavePath, false);
	export("$BTZ::ZonePos*",	$BTZ::SavePath, true );
	export("$BTZ::ZoneScale*",	$BTZ::SavePath, true );
}

function BTZ_ToggleDebug()
{
	talk("BTZ_DEBUG TOGGLED: " @ (!$BTZ::Debug ? "on" : "off"));
	$BTZ::Debug = !$BTZ::Debug;

	if(!isObject("BTZ_DebugSet"))
		new SimSet(BTZ_DebugSet);

	if(!$BTZ::Debug)
	{
		BTZ_DebugSet.deleteAll();
		return;
	}

	for(%i = 0; %i < $BTZ::NumZones; %i++)
	{
		%pos = $BTZ::ZonePos[%i];
		%scale = $BTZ::ZoneScale[%i];
		
		%shape = new StaticShape() {
			position = %pos;
			scale = %scale;
			dataBlock = "ND_SelectionBoxOuter";
		};
		%shape.setNodeColor("ALL","0 0 1 0.2");
		BTZ_DebugSet.add(%shape);
	}
}

function BTZ_deleteAll()
{
	//delete the triggers
	for(%i = 0; %i < $BTZ::NumZones; %i++)
	{
		$BTZ::ZoneObj[%i].occupants.delete();
		$BTZ::ZoneObj[%i].delete();
		$BTZ::ZoneSet[%i].delete();
	}

	//delete the relevant variables
	deleteVariables("$BTZ::ZonePos*");
	deleteVariables("$BTZ::ZoneScale*");
	deleteVariables("$BTZ::ZoneObj*");

	deleteVariables("$BTZ::NumZones");

	if(isObject(BTZ_DebugSet))
		BTZ_DebugSet.deleteAll();
}

function BTZ_createZone(%index, %position, %scale)
{
	talk("index: " @ %index);
	%trigger = new Trigger()
	{
		datablock = BotToggleTriggerData;
		//polyhedron = "0 0 0 1 0 0 0 -1 0 0 0 1"; //this determines the shape of the trigger
		polyhedron = "-0.5 0.5 -0.5 1.0 0.0 0.0 0.0 -1.0 0.0 0.0 0.0 1.0";
	};

	if(!isObject(%trigger))
	{
		talk("BAD TRIGGER?");
		return;
	}

	%trigger.setTransform(%position);
	%trigger.setScale(%scale);

	//tagged fields inside the constructor are being buggy?
	%trigger.zoneIndex = %index;
	%trigger.occupants = new SimSet();

	$BTZ::ZoneObj[%index] = %trigger;
	$BTZ::ZoneSet[%index] = new SimSet();

	if($BTZ::Debug)
	{
		%shape = new StaticShape() {
			position = %position;
			scale = %scale;
			dataBlock = "ND_SelectionBoxOuter";
		};
		%shape.setNodeColor("ALL","0 0 1 0.2");
		BTZ_DebugSet.add(%shape);
	}

	%set = nameToID("mainHoleBotBrickSet");
	if(%set == -1)
		return;

	%ZonePos = %position;
	%ZoneScale = %scale;
	%ZoneSet = $BTZ::ZoneSet[%index];

	for(%i = 0; %i < %set.getCount(); %i++)
	{
		%brick = %set.getObject(%i);
		%pos = %brick.getPosition();

		if(BTZ_posInBox3D(%pos, %ZonePos, %ZoneScale))
		{
			%ZoneSet.add(%brick);
			%brick.zoneIndex = %index;
		}
	}
}

function BTZ_addZone(%position, %scale)
{
	%index = $BTZ::NumZones + 0;
	talk("ADDED NODE #" @ %index);

	$BTZ::ZonePos[%index] = %position;
	$BTZ::ZoneScale[%index] = %scale;

	BTZ_createZone(%index, %position, %scale);

	$BTZ::NumZones++;
}

function BTZ_DeleteZone(%position, %scale)
{
	talk("wip");
}

