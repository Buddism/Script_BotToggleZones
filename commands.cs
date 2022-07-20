function serverCmdBTZ(%client, %operator, %arg1, %arg2, %arg3, %arg4, %arg5)
{
	if(!%client.isAdmin)
		return;

	if(%operator $= "")
	{
		%client.chatMessage("\c6invalid operator");
		%client.chatMessage("\c6ops: save, load, deleteAll, debug, Place (p) arg: >1 is size, Delete (d)");
		return;
	}
	

	switch$(%operator)
	{
		case "save":
			BTZ_save();

		case "load":
			BTZ_load();

		case "deleteAll":
			BTZ_deleteAll();

		case "debug":
			BTZ_ToggleDebug();

		default:
			%noOperator = true;
	}

	if(!%noOperator)
		return;


	//selection box operations
	%box = %client.NDSelectionBox.outerBox;
	if(!isObject(%box))
		return;
		
	%position = %box.getPosition();
	%scale = %box.getScale();

	switch$(%operator)
	{
		case "Place" or "P":
			BTZ_addZone(%position, %scale, %arg1);

		case "Delete" or "D":
			BTZ_DeleteZone(%position, vectorSub(%scale, "0.05 0.05 0.05"));
	}
}