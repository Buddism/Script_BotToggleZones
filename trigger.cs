datablock TriggerData(BotToggleTriggerData)
{
	tickPeriodMS = 1000;
};

//i would use these but onremove is overwritten/broken ?
//function BotToggleTriggerData::onRemove(%this, %trigger) { return parent::onRemove(%this, %trigger); }
//function BotToggleTriggerData::onAdd(%this, %trigger) { return parent::onAdd(%this, %trigger); }
function BotToggleTriggerData::onTickTrigger(%this,%trigger,%obj) {}

function BotToggleTriggerData::onEnterTrigger(%this, %trigger, %obj)
{
	%client = %obj.client;
	if(!isObject(%client) || %client.getClassName() !$= "GameConnection")
		return;

	%index = %trigger.zoneIndex;
	if($BTZ::ZoneObj[%index] != %trigger)
		return talk(bad SPC %trigger);

	%trigger.occupants.add(%client);
	%occupancy = %trigger.occupants.getCount();

	%set = $BTZ::ZoneSet[%index];
	%count = %set.getCount();
	for(%i = 0; %i < %count; %i++)
	{
		%obj = %set.getObject(%i);

		if(isObject(%obj.hBot))
		{
			%obj.hBot.scopeToClient(%client);

			if(%occupancy == 1)
				%obj.BTZ_setBotEnabled();

			%obj.hBot.numScopes++;
		}
	}
	
	if($BTZ::Debug)
		%client.bottomprint("enter" SPC %index, 3);
}

function BotToggleTriggerData::onLeaveTrigger(%this, %trigger, %obj)
{
	%client = %obj.client;
	if(!isObject(%client) || %client.getClassName() !$= "GameConnection")
		return;

	%index = %trigger.zoneIndex;
	if($BTZ::ZoneObj[%index] != %trigger)
		return talk(bad SPC %trigger);

	%trigger.occupants.remove(%client);
	%occupancy = %trigger.occupants.getCount();
	
	%set = $BTZ::ZoneSet[%index];
	%count = %set.getCount();
	for(%i = 0; %i < %count; %i++)
	{
		%obj = %set.getObject(%i);

		if(isObject(%obj.hBot))
		{
			%obj.hBot.clearScopeToClient(%client);

			if(%occupancy == 0)
				%obj.BTZ_setBotDisabled();
		}
	}

	if($BTZ::Debug)
		%client.bottomprint("leave" SPC %index, 3);
}
