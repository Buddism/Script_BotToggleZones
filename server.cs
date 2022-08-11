exec("./trigger.cs");
exec("./commands.cs");
exec("./support.cs");
exec("./package.cs");

$BTZ::SavePath = "config/server/BotToggleZones.cs";

//$BTZ::ZonePos[#]
//$BTZ::ZoneScale[#]
//$BTZ::ZoneObj[#]
//	$BTZ::ZoneObj[#].occupants
//$BTZ::ZoneSet[#]
//$BTZ::NumZones

function BTZ_load()
{
	BTZ_deleteAll();

	exec($BTZ::SavePath);

	for(%i = 0; %i < $BTZ::NumZones; %i++)
	{
		%pos = $BTZ::ZonePos[%i];
		%scale = $BTZ::ZoneScale[%i];
		%triggerMultiplier = $BTZ::triggerMultiplier[%i];

		BTZ_createZone(%i, %pos, %scale, %triggerMultiplier);
	}

	talk("BTZ_LOADED");
}

schedule(0, 0, BTZ_load);

function BTZ_save()
{
	//export variable,					SavePath,		append
	export("$BTZ::NumZones",			$BTZ::SavePath, false);
	export("$BTZ::ZonePos*",			$BTZ::SavePath, true );
	export("$BTZ::ZoneScale*",			$BTZ::SavePath, true );
	export("$BTZ::triggerMultiplier*",	$BTZ::SavePath, true );

	talk("BTZ_SAVED");
}

function BTZ_ToggleDebug()
{
	talk("BTZ_DEBUG TOGGLED: " @ (!$BTZ::Debug ? "on" : "off"));
	$BTZ::Debug = !$BTZ::Debug;

	if(!isObject("BTZ_DebugSet"))
		new SimSet(BTZ_DebugSet);

	if(!$BTZ::Debug)
	{
		if(isFunction("Trigger", "setNetFlag"))
		{
			for(%i = 0; %i < $BTZ::NumZones; %i++)
				$BTZ::ZoneObj[%i].setNetFlag(8, false);
		}
		BTZ_DebugSet.deleteAll();
		return;
	}

	for(%i = 0; %i < $BTZ::NumZones; %i++)
	{
		%pos = $BTZ::ZonePos[%i];
		%scale = $BTZ::ZoneScale[%i];
		
		if($BTZ::TriggerMultiplier[%i] > 1)
		{
			%shape = new StaticShape() {
				position = %pos;
				scale = %scale;
				dataBlock = "ND_SelectionBoxOuter";
			};
			%shape.setNodeColor("ALL","0 0 1 0.2");
			BTZ_DebugSet.add(%shape);
		}

		if(isFunction("Trigger", "setNetFlag"))
			$BTZ::ZoneObj[%i].setNetFlag(8, true);
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
	deleteVariables("$BTZ::ZoneSet*");
	deleteVariables("$BTZ::ZoneObj*");
	deleteVariables("$BTZ::TriggerMultiplier*");

	deleteVariables("$BTZ::NumZones");

	if(isObject(BTZ_DebugSet))
		BTZ_DebugSet.deleteAll();
}

function BTZ_createZone(%index, %position, %scale, %triggerMultiplier)
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

	%triggerMultiplier = mClampF(%triggerMultiplier, 1.0, 10000.0);
	%trigger.setTransform(%position);
	%trigger.setScale(vectorScale(%scale, %triggerMultiplier));

	//tagged fields inside the constructor are being buggy?
	%trigger.zoneIndex = %index;
	%trigger.occupants = new SimSet();

	$BTZ::ZoneObj[%index] = %trigger;
	$BTZ::ZoneSet[%index] = new SimSet();

	if($BTZ::Debug)
	{
		if(%triggerMultiplier != 1)
		{
			%shape = new StaticShape() {
				position = %position;
				scale = %scale;
				dataBlock = "ND_SelectionBoxOuter";
			};
			%shape.setNodeColor("ALL","0 0 1 0.2");
		}
		
		if(isFunction("Trigger", "setNetFlag"))
			%trigger.setNetFlag(8, true);

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

function BTZ_addZone(%position, %scale, %triggerMultiplier)
{
	%index = -1;
	for(%i = 0; %i < $BTZ::NumZones; %i++)
		if(!isObject($BTZ::ZoneObj[%i]))
		{
			%index = %i;
			break;
		}
	if(%index == -1)
	{
		%index = $BTZ::NumZones + 0;
		$BTZ::NumZones++;
	}	
	talk("ADDED NODE #" @ %index);

	$BTZ::ZonePos[%index] = %position;
	$BTZ::ZoneScale[%index] = %scale;

	%triggerMultiplier = mClampF(%triggerMultiplier, 1.0, 10000.0);
	$BTZ::triggerMultiplier[%index] = %triggerMultiplier;

	BTZ_createZone(%index, %position, %scale, %triggerMultiplier);
}

function BTZ_removeZone(%index)
{
	$BTZ::ZoneObj[%index].occupants.delete();
	$BTZ::ZoneObj[%index].delete();

	%set = $BTZ::ZoneSet[%index];
	%count = %set.getCount();
	for(%i = 0; %i < %count; %i++)
	{
		%obj = %set.getObject(%i);
		if(%obj.zoneIndex == %index)
			%obj.zoneIndex = "";
	}
	%set.delete();

	%farIndex = $BTZ::NumZones--;

	//do a swap, the deleted zoneindex with the last zone
	if(%farIndex >= 0 && %index != %farIndex) //if we are not at the end of the array
	{
		$BTZ::ZoneObj[%index] = $BTZ::ZoneObj[%farIndex];
		$BTZ::ZoneSet[%index] = $BTZ::ZoneSet[%farIndex];

		$BTZ::ZonePos[%index] = $BTZ::ZonePos[%farIndex];
		$BTZ::ZoneScale[%index] = $BTZ::ZoneScale[%farIndex];

		$BTZ::triggerMultiplier[%index] = $BTZ::triggerMultiplier[%farIndex];
	}

	$BTZ::ZoneObj[%farIndex] = "";
	$BTZ::ZoneSet[%farIndex] = "";
	$BTZ::ZonePos[%farIndex] = "";
	$BTZ::ZoneScale[%farIndex] = "";
	$BTZ::triggerMultiplier[%farIndex] = "";
}

function BTZ_DeleteZone(%position, %scale)
{
	%compPos = %position;
	%compScale = %scale;

	for(%i = $BTZ::NumZones - 1; %i >= 0; %i--)
	{
		%box_pos = $BTZ::ZonePos[%i];
		%box_scale = $BTZ::ZoneScale[%i];

		if(BTZ_getBoxOverlap(%box_pos, %box_scale, %compPos, %compScale))
		{
			BTZ_removeZone(%i);
			talk("removing zone #"@ %i);
		}
	}
}
