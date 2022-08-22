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

	%set = $BTZ::ZoneSet[%index];
	%count = %set.getCount();
	for(%i = 0; %i < %count; %i++)
	{
		%obj = %set.getObject(%i);

		if(isObject(%obj.hBot))
		{
			%obj.hBot.scopeToClient(%client);

			if(%obj.hBot.numScopes == 0)
				%obj.hBot.setNetFlag(1, 0); //enable state

			%obj.hBot.numScopes++;
		}
	}

	%trigger.occupants.add(%client);
	
	if($BTZ::Debug)
		%client.bottomprint("enter" SPC %index, 3);
}

function BotToggleTriggerData::onLeaveTrigger(%this, %trigger, %obj)
{
	%client = %obj.client;
	talk(leave SPC %obj SPC %client);
	if(!isObject(%client) || %client.getClassName() !$= "GameConnection")
		return;

	%index = %trigger.zoneIndex;
	if($BTZ::ZoneObj[%index] != %trigger)
		return talk(bad SPC %trigger);

	%set = $BTZ::ZoneSet[%index];
	%count = %set.getCount();
	for(%i = 0; %i < %count; %i++)
	{
		%obj = %set.getObject(%i);

		if(isObject(%obj.hBot))
		{
			%obj.hBot.clearScopeToClient(%client);

			if(%obj.hBot.numScopes-- <= 0)
				%obj.hBot.setNetFlag(1, 1); //disable state
		}
	}

	%trigger.occupants.remove(%client);

	if($BTZ::Debug)
		%client.bottomprint("leave" SPC %index, 3);
}
