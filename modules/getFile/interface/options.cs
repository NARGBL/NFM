if(isobject(nfm_options))
	nfm_options.delete();
exec("./guis/options.gui");

function nfm_options()
{
	nfm_options.loadvalues();

	//for some reason this can get stuck behind other things
	if(nfm_options.isawake())
		canvas.popdialog(nfm_options);
	canvas.pushdialog(nfm_options);
}

function nfm_options::loadvalues(%this)
{
	//wtf baldspot pls
//	if($LoadingBricks_ColorMethod $= "")
//		$LoadingBricks_ColorMethod = 3;

	nfm_debug("loading values");
	//basic options
	nfm_options_homedirectory.settext($NFM::Home);
	nfm_options_font.settext($NFM::Font);
	%this.settilesize($NFM::ListSize);
	%this.setsort($NFM::DefaultSort);

	//brick load/save options
	nfm_options_saveevents.setvalue($NFM::pref::SaveExtendedBrickInfo);
	nfm_options_saveownership.setvalue($NFM::pref::SaveOwnership);
	%this.setloadowner($NFM::LoadingBricks_Ownership);
	nfm_options_loadoffset.settext($NFM::LoadingBricks_PositionOffset);
	nfm_options_colormode.settext($NFM::LoadingBricks_ColorMethod);

	//advanced options
	nfm_options_brickcount.setvalue($NFM::BrickCount);
	nfm_options_versioncheck.setvalue($NFM::VersionCheck);
	nfm_options_buffer.setvalue($NFM::Buffer);
	nfm_options_cache.setvalue($NFM::Cache);
	nfm_options_warning.setvalue($NFM::Warning);
	nfm_options_buffersize.settext($NFM::BufferCount);
	nfm_options_buffertime.settext($NFM::BufferTime);
	nfm_options_prefspath.settext($NFM::PrefsPath);
	nfm_debug("loaded values");
}

function nfm_options::clickapply(%this)
{
	//some things are harder than updating variables

	if($NFM::Font !$= nfm_options_font.getvalue())
	{
		messageboxyesno("Attention", "Changes to the font require the File Manager to be restarted.  Would you like to close and restart the File Manager now?", "nfm_restartfilemanager();");
	}

	if($NFM::ListSize != %this.nfm_ListSize)
	{
		nfm_debug("Need to rebuild file list due to size change");
		%dorefresh = true;
	}

	//basic options
	$NFM::Home = nfm_options_homedirectory.getvalue();
	$NFM::Font = nfm_options_font.getvalue();
	$NFM::ListSize = %this.nfm_ListSize;
	$NFM::DefaultSort = %this.nfm_DefaultSort;

	//brick load/save options
	//0 - Yours
	//1 - Saved
	//2 - Public
	$NFM::pref::SaveExtendedBrickInfo = nfm_options_saveevents.getvalue();
	$NFM::pref::SaveOwnership = nfm_options_saveownership.getvalue();
	$NFM::LoadingBricks_Ownership = %this.LoadingBricks_Ownership;
    $NFM::LoadingBricks_PositionOffset = nfm_options_loadoffset.getvalue();
    $NFM::LoadingBricks_ColorMethod = nfm_options_colormode.getvalue();

	//advanced options
	if($NFM::BrickCount != nfm_options_brickcount.getvalue())
	{
		%dorefresh = true;
	}
	$NFM::BrickCount = nfm_options_brickcount.getvalue();
	$NFM::VersionCheck = nfm_options_versioncheck.getvalue();
	$NFM::Buffer = nfm_options_buffer.getvalue();
	$NFM::Cache = nfm_options_cache.getvalue();
	$NFM::BufferTime = nfm_options_buffertime.getvalue();
	$NFM::Warning = nfm_options_warning.getvalue();

	if(iswriteablefilename(nfm_options_prefspath.getvalue()))
	{
		$NFM::PrefsPath = nfm_options_prefspath.getvalue();
	}
	else
	{
		messageboxok("Error", "Cannot save preferences to this location.  Please choose a difference path.\n\nFile: " @ nfm_options_prefspath.getvalue());
	}

	if(nfm_options_buffersize.getvalue() > 0)
	{
		$NFM::BufferCount = nfm_options_buffersize.getvalue();
	}
	else
	{
		messageboxok("Error", "Please choose a buffer size greater than zero");
	}
	export("$NFM::*", $NFM::PrefsPath);

	if($NFM::PrefsPath !$= $NFM_DEFAULT::PrefsPath)
		export("$NFM::PrefsPath", $NFM_DEFAULT::PrefsPath);
	nfm_options.loadvalues();

	if(%dorefresh)
		NFMGUI.refresh_list(true);
}

function nfm_options::clickcancel(%this)
{
	exec($NFM::PrefsPath);
	canvas.popdialog(nfm_options);
}

function nfm_options::clickdefaults(%this)
{
	%this.loadvalues();
	%this.defaults();
}

function nfm_options::clickok(%this)
{
	%this.clickapply();
	canvas.popdialog(%this);
}

function nfm_options::settilesize(%this, %value)
{
	nfm_debug("setting list size to: " @ %value);

	switch(%value)
	{
		case 2:
			%this.nfm_ListSize = 2;
			nfm_options_tilesize1.setvalue(0);
			nfm_options_tilesize2.setvalue(1);
			nfm_options_tilesize3.setvalue(0);
		case 3:
			%this.nfm_ListSize = 3;
			nfm_options_tilesize1.setvalue(0);
			nfm_options_tilesize2.setvalue(0);
			nfm_options_tilesize3.setvalue(1);
		default:
			%this.nfm_ListSize = 1;
			nfm_options_tilesize1.setvalue(1);
			nfm_options_tilesize2.setvalue(0);
			nfm_options_tilesize3.setvalue(0);
	}
}

function nfm_options::setloadowner(%this, %value)
{
	nfm_debug("setting load owner to: " @ %value);

	switch(%value)
	{
		case 1:
			%this.LoadingBricks_Ownership = 1;
			nfm_options_loadowner0.setvalue(0);
			nfm_options_loadowner1.setvalue(1);
			nfm_options_loadowner2.setvalue(0);
		case 2:
			%this.LoadingBricks_Ownership = 2;
			nfm_options_loadowner0.setvalue(0);
			nfm_options_loadowner1.setvalue(0);
			nfm_options_loadowner2.setvalue(1);
		default:
			%this.LoadingBricks_Ownership = 0;
			nfm_options_loadowner0.setvalue(1);
			nfm_options_loadowner1.setvalue(0);
			nfm_options_loadowner2.setvalue(0);
	}
}

function nfm_options::setsort(%this, %value)
{
	//nfm_options_sort0
	if(%value == 1)
	{
		%this.nfm_DefaultSort = 1;
		nfm_options_sort0.setvalue(0);
		nfm_options_sort1.setvalue(1);
	}
	else
	{
		%this.nfm_DefaultSort = 0;
		nfm_options_sort0.setvalue(1);
		nfm_options_sort1.setvalue(0);
	}
}

function nfm_options::defaults(%this)
{
	nfm_options_homedirectory.settext($NFM_DEFAULT::Home);
	nfm_options_font.settext($NFM_DEFAULT::Font);
	%this.settilesize($NFM_DEFAULT::ListSize);
	%this.setsort($NFM_DEFAULT::DefaultSort);

	//brick load/save options
	nfm_options_saveevents.setvalue($NFM_DEFAULT::pref::SaveExtendedBrickInfo);
	nfm_options_saveownership.setvalue($NFM_DEFAULT::pref::SaveOwnership);
	%this.setloadowner($NFM_DEFAULT::LoadingBricks_Ownership);
	nfm_options_loadoffset.settext($NFM_DEFAULT::LoadingBricks_PositionOffset);
	nfm_options_colormode.settext($NFM_DEFAULT::LoadingBricks_ColorMethod);

	//advanced options
	nfm_options_brickcount.setvalue($NFM_DEFAULT::BrickCount);
	nfm_options_versioncheck.setvalue($NFM_DEFAULT::VersionCheck);
	nfm_options_buffer.setvalue($NFM_DEFAULT::Buffer);
	nfm_options_cache.setvalue($NFM_DEFAULT::Cache);
	nfm_options_warning.setvalue($NFM_DEFAULT::Warning);
	nfm_options_buffersize.settext($NFM_DEFAULT::BufferCount);
	nfm_options_buffertime.settext($NFM_DEFAULT::BufferTime);
	nfm_options_prefspath.settext($NFM_DEFAULT::PrefsPath);
}

