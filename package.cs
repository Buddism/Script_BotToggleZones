package Script_BotToggleZones
{
	function fxDTSBrick::onHoleSpawnPlanted(%obj)
	{
		%parent = parent::onHoleSpawnPlanted(%obj);

		%pos = %obj.getPosition();
		for(%i = 0; %i < $BTZ::NumZones; %i++)
		{
			%ZonePos = $BTZ::ZonePos[%i];
			%ZoneScale = $BTZ::ZoneScale[%i];

			if(BTZ_posInBox3D(%pos, %ZonePos, %ZoneScale))
			{
				$BTZ::ZoneSet[%i].add(%obj);
				%obj.zoneIndex = %i;
				break;
			}
		}

		return %parent;
	}

	function fxDTSBrick::spawnHoleBot(%obj)
	{
		%parent = parent::spawnHoleBot(%obj);
		//return is 0 or the new playerid
		if(%parent && %obj.zoneIndex !$= "")
		{
			%parent.setNetFlag(6, 1);
			%index = %obj.zoneIndex;

			%set = $BTZ::ZoneObj[%index].occupants;
			%count = %set.getCount();
			if(%count == 0)
				%parent.setNetFlag(1, 1); //disable state


			for(%i = 0; %i < %count; %i++)
				%parent.scopeToClient(%set.getObject(%i));

			%parent.numScopes = %count;
		}

		return %parent;
	}
};
activatePackage(Script_BotToggleZones);