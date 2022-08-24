function fxDTSBrick::BTZ_setBotDisabled(%this, %disabled)
{
	if(isObject(%this.hbot))
		%this.delete();
}

function fxDTSBrick::BTZ_setBotEnabled(%this)
{
	%this.spawnHoleBot();
}