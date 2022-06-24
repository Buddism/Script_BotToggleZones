function BTZ_posInBox(%position, %boxPos, %boxScale)
{
    if(mAbs(getWord(%position, 0) - getWord(%boxPos, 0)) < getWord(%boxScale, 0) / 2)
        if(mAbs(getWord(%position, 1) - getWord(%boxPos, 1)) < getWord(%boxScale, 1) / 2)
            return true;
    return false;
}
function BTZ_posInWorldbox(%pos, %worldbox)
{
	%neg = getWords(%worldbox, 0, 2); //negative xyz corner of the box
	%pos = getWords(%worldbox, 3, 5); //positive xyz corner of the box

	%px = getWord(%pos, 0);
	%py = getWord(%pos, 1);
	%pz = getWord(%pos, 2);

	return %px > getWord(%neg, 0) && %py > getWord(%neg, 1) && %pz > getWord(%neg, 2)
		&& %px < getWord(%pos, 0) && %py < getWord(%pos, 1) && %pz < getWord(%pos, 2);
}

function BTZ_posInBox3D(%position, %boxPos, %boxScale)
{
    if(mAbs(getWord(%position, 0) - getWord(%boxPos, 0)) < getWord(%boxScale, 0) / 2)
        if(mAbs(getWord(%position, 1) - getWord(%boxPos, 1)) < getWord(%boxScale, 1) / 2)
            return mAbs(getWord(%position, 2) - getWord(%boxPos, 2)) < getWord(%boxScale, 2) / 2;
    return false;
}

function BTZ_getBoxOverlap(%box_posX0, %box_posY0, %box_posZ0, %box_scale0, %box_posX1, %box_posY1, %box_posZ1, %box_scale1)
{
    %x = (mAbs(%box_posX0 - %box_posX1) * 2) <= (getWord(%box_scale0, 0) + getWord(%box_scale1, 0));
    %y = (mAbs(%box_posY0 - %box_posY1) * 2) <= (getWord(%box_scale0, 1) + getWord(%box_scale1, 1));
    %z = (mAbs(%box_posZ0 - %box_posZ1) * 2) <= (getWord(%box_scale0, 2) + getWord(%box_scale1, 2));
    return %x && %y && %z;
}
