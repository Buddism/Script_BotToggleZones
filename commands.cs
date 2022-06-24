function serverCmdBTZ(%client, %operator, %o1, %o2, %o3, %o4, %o5)
{
	if(!%client.isAdmin)
		return;

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
			BTZ_addZone(%position, %scale);

		case "Delete" or "D":
			BTZ_DeleteZone(%position, vectorSub(%scale, "0.05 0.05 0.05"));

		default:
			%client.chatMessage("\c6invalid operator");
			%client.chatMessage("\c6ops: save, load, deleteAll, debug, Place (p), Delete (d)");
	}
}