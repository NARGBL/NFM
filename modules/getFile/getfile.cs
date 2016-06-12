//NARG File Manager
// getFile Module
//Version 1.42 Full
//  By Nexus 4833
//=================

//dependencies
if($getname_version < 1.0)
{
	echo("Warning! Missing dependency: getFile Module is dependent on getName version 1.0 or newer.");
}

if($zoomimage_version < 1.0)
{
	echo("Warning! Missing dependency: getFile Module is dependent on zoomImage version 1.0 or newer.");
}

%nfm_version_major = "1.42"; //must be a value
%nfm_version_minor = "Full"; //allowed values are "Alpha", "Beta", "Full", "Modified" (in that order of precedence)

//initialize some stuff
if($nfm_version !$= "")
{
	%oldver = getword($nfm_version, 0); //using getWord in logical expressions results in unexpected behaviour!

	if(%oldver > %nfm_version_major || (%oldver $= %nfm_version_major && stricmp(getword($nfm_version, 1), %nfm_version_minor) > -1))
	{
		echo("File Manager [" @ %nfm_version_major SPC %nfm_version_minor @ "] not needed, version [" @ $nfm_version @ "] already in place");
		return;
	}
	echo("Updating to File Manager [" @ %nfm_version_major SPC %nfm_version_minor @ "] from version [" @ $nfm_version @ "]");
}
$nfm_version = %nfm_version_major SPC %nfm_version_minor;
$nfm_debug = false;
$nfm_uipath = "";

//in case of re-execution
$nfm_initialized = false;

//promptUserFile is the only function your program should need to call
//%default - default directory, including a default file if you wish
//%callback - your function that takes arguments %path and %description
//%mode - modes accepted are "save", "load", "savebls", "loadbls", and ""
//%ext - file extension to use as a filter, including the period, wildcards accepted.
//	Default value is "" and input is ignored if %mode is "savebls" or "loadbls".

function promptUserFile(%default, %callback, %mode, %ext)
{
	if(!$nfm_initialized)
	{
		$nfm_initialized = true;
		nfm_initgui();
	}
	NFMGUI.callback = %callback;

	if(%default $= "")
	{
		if(%mode $= "savebls")
			%default = NFMGUI.savedirectory;
		else if(%mode $= "loadbls")
			%default = NFMGUI.loaddirectory;
		else
			%default = $NFM::Home;
	}
//	nfmgui_filename.settext("");
	nfmgui_filename.settext(filename(%default));

	if(isfile(%default))
		NFMGUI.setcurrentfile(%default);
	else
		NFMGUI.setcurrentfile("");
	NFMGUI.copyfilepath = "";
	NFMGUI.selectedfilebg = "";
	NFMGUI.cutting = false;
	NFMGUI.setmode(%mode, %ext);

//	if(isfile(%default))
//	{
//		nfmgui_filename.settext(filename(%default));
//		NFMGUI.loadpath(getsubstr(%defaul
//	}
//	else
//		NFMGUI.loadpath(%default);
	NFMGUI.loadpath(getsubstr(%default, 0, strlen(%default) - strlen(filename(%default))));
	
	canvas.pushdialog(NFMGUI);

	if($nfm_firsttime)
	{
		$nfm_firsttime = false;
		export("$NFM::*", $NFM::PrefsPath);
		canvas.pushdialog(nfm_tutorial);
	}
}

//find out what folder this file is in
//maybe sort of hacky but if people want to use this module in arbitrary locations then it has to be done
//actually maybe ".." can be used to refer to parent directory but I already did it this way, oh well

function nfm_getUIPath()
{
	if($nfm_uipath $= "")
	{
		%fullpath = findfirstfile("./getfile.cs");
		$nfm_uipath = getsubstr(%fullpath, 0, strlen(%fullpath) - strlen(filename(%fullpath))) @ "ui/";
	}
	return $nfm_uipath;
}

//preferences
//----------
function nfm_defaults()
{
	$NFM::BrickCount = $NFM_DEFAULT::BrickCount = false;
	$NFM::Buffer = $NFM_DEFAULT::Buffer = true;
	$NFM::Cache = $NFM_DEFAULT::Cache = true;
	$NFM::BufferCount = $NFM_DEFAULT::BufferCount = 50;
	$NFM::BufferTime = $NFM_DEFAULT::BufferTime = 1;
	$NFM::VersionCheck = $NFM_DEFAULT::VersionCheck = true;
	$NFM::Warning = $NFM_DEFAULT::Warning = true;
	
	$NFM::Home = $NFM_DEFAULT::Home = "saves/";
	$NFM::ListSize = $NFM_DEFAULT::ListSize = 1; //1 2 or 3
	$NFM::Font = $NFM_DEFAULT::Font = "arial bold";
	$NFM::PrefsPath = $NFM_DEFAULT::PrefsPath = "config/client/NARG File Manager/preferences.cs";
	
	$NFM::ItemColorIdle = $NFM_DEFAULT::ItemColorIdle = "230 230 230 255";
	$NFM::ItemColorHighlight = $NFM_DEFAULT::ItemColorHighlight = "255 255 255 255";
	$NFM::ItemColorPress = $NFM_DEFAULT::ItemColorPress = "170 170 170 255";
	$NFM::ItemColorContext = $NFM_DEFAULT::ItemColorContext = "170 170 230 255";
	$NFM::ItemColorSelect = $NFM_DEFAULT::ItemColorSelect = "170 230 170 255";
	
	//default defaults
	$NFM::pref::SaveExtendedBrickInfo = $NFM_DEFAULT::pref::SaveExtendedBrickInfo = 1;
	$NFM::pref::SaveOwnership = $NFM_DEFAULT::pref::SaveOwnership = 1;
	$NFM::LoadingBricks_Ownership = $NFM_DEFAULT::LoadingBricks_Ownership = 1;
	$NFM::LoadingBricks_ColorMethod = $NFM_DEFAULT::LoadingBricks_ColorMethod = 3;
	$NFM::LoadingBricks_PositionOffset = $NFM_DEFAULT::LoadingBricks_PositionOffset = "0 0 0";
	
	//0 - name
	//1 - date
	$NFM::DefaultSort = $NFM_DEFAULT::DefaultSort = 0; //0 or 1
}
nfm_defaults();

function nfm_restartfilemanager()
{
	$nfm_initialized = false;
	exec("./interface/interface.cs");
}

if(isfile($NFM_DEFAULT::PrefsPath))
{
	exec($NFM_DEFAULT::PrefsPath);

	if(isfile($NFM::PrefsPath))
		exec($NFM::PrefsPath);
}
else
{
	$nfm_firsttime = true;
}

exec("./support/support.cs");
exec("./support/versioncheck.cs");
exec("./support/loader.cs");

exec("./interface/interface.cs");

nfm_initmanager();

