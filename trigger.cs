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
		return talk(bad);

	%set = $BTZ::ZoneSet[%index];
	%count = %set.getCount();
	for(%i = 0; %i < %count; %i++)
	{
		%obj = %set.getObject(%i);

		if(isObject(%obj.hBot))
			%obj.hBot.scopeToClient(%client);
	}

	%trigger.occupants.add(%client);
	%client.bottomprint("enter", 1);
}

function BotToggleTriggerData::onLeaveTrigger(%this, %trigger, %obj)
{
	%client = %obj.client;
	if(!isObject(%client) || %client.getClassName() !$= "GameConnection")
		return;

	%index = %trigger.zoneIndex;
	if($BTZ::ZoneObj[%index] != %trigger)
		return talk(bad);

	%set = $BTZ::ZoneSet[%index];
	%count = %set.getCount();
	for(%i = 0; %i < %count; %i++)
	{
		%obj = %set.getObject(%i);

		if(isObject(%obj.hBot))
			%obj.hBot.clearScopeToClient(%client);
	}

	%trigger.occupants.remove(%client);
	%client.bottomprint("leave", 1);
}