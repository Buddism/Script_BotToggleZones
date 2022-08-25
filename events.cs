registerOutputEvent ("Bot",		"BTZ_CanDespawn", "");
function AIPlayer::BTZ_CanDespawn(%this)
{
	if(!isObject(%this.spawnBrick))
		return;
	
	%this.spawnBrick.BTZ_CanDespawn = true;
	%this.spawnBrick.BTZ_setBotDisabled();
}



function fxDTSBrick::BTZ_setBotDisabled(%this)
{
	cancel(%brick.hModS);
	%this.hModS = 0;

	if(isObject(%this.hbot))
	{
		%this.delete();
		%this.hBot = 0;
	}
}

function fxDTSBrick::BTZ_setBotEnabled(%this)
{
	%this.spawnHoleBot();
}