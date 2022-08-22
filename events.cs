function AIPlayer::setProcessingEnabled(%this, %isClientObject)
{
	%this.setNetFlag(1, !%isClientObject);
}