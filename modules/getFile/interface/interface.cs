//interface.cs

exec("./profiles.cs");

//each of these sub-scripts will manage their own guis
exec("./nfm.cs");
exec("./contextmenu.cs");
exec("./filelist.cs");
exec("./options.cs");
exec("./tutorial.cs");

//main objects
//----------
//nfmgui - gui control
//nfmgui_preview - bitmap preview
//nfmgui_description - text description
//nfmgui_filelist - file list swatch
//nfmgui_saveload - save/load button
//nfmgui_filename - text edit control for file name
//nfmgui_address - text edit control for address
//nfm_loadingscreen - swatch that blocks loading screen
//nfm_loadingmessage - text in loading screen
//nfmgui_morebutton - the pulldown button with the v
//nfmgui_exttext - shows current extension filter

//init
function nfm_initgui()
{
	if(!$nfm_versionchecked)
	{
		$nfm_versionchecked = true;
		//nfm_versioncheck(0);
		narg_versioncheck(0, "NFM", $nfm_version);
	}
	//keeps track of last save directory and last load directory independently
	NFMGUI.currdirectory = NFMGUI.searchtext = NFMGUI.savedirectory = NFMGUI.loaddirectory = $NFM::Home;
	NFMGUI.selectedfile = "";
	//NFMGUI.setcurrentfile("");
	NFMGUI.selectedfilebg = "";
	NFMGUI.callback = "";
	NFMGUI.copyfilepath = "";
	NFMGUI.cutting = false;
	NFMGUI.setmode("loadbls");
}

//callbacks
function nfm_completeload()
{
	//idk if you can use call() for object based functions
	nfm_debug("completeload called");
	NFMGUI.completeload();
}

//rename
function nfm_renamefile(%text)
{
	NFMGUI.renameitem(%text);
}

function nfm_newfoldername(%text)
{
	%path = NFMGUI.currdirectory;
	%text = %text @ "/";
	nfm_debug("creating new folder: " @ %text);
	%obj = NARGFileManager.pathcontrol[%path, NFMGUI.ext];

	if(!isobject(%obj))
	{
		nfm_debug("cannot modify a list that doesn't exist");
		messageboxok("Creating new folders requires caching to be enabled.");
		return;
	}
	%obj.folderlist.addrow(%obj.folderlist.rowcount(), %text TAB 0 TAB %obj.path @ %text);
	%obj.sortmode = "";
	NFMGUI.buildfilelist(%obj);
}

//functions for specific buttons
//----------
function nfm_pressup()
{
	NFMGUI.loadpath(nfm_getdirectoryup(NFMGUI.currdirectory));
}

function nfm_pressgo()
{
	%path = nfmgui_address.getvalue();
	NFMGUI.loadpath(%path);
}

function nfm_presssaveload()
{
	%path = NFMGUI.currdirectory @ nfmgui_filename.getvalue();

	nfm_debug("appending " @ nfmgui_exttext.gettext() @ " to file name");
	%path = %path @ nfmgui_exttext.gettext();

	NFMGUI.selectfile(%path);
}

function nfm_presscancel()
{
	canvas.popdialog(NFMGUI);
}

function nfm_presssortname()
{
	%obj = NARGFileManager.pathcontrol[NFMGUI.searchtext, NFMGUI.ext];

	if(!isobject(%obj))
	{
		nfm_debug("cannot sort a list that doesn't exist");
		return;
	}

	if(%obj.sortmode $= "name")
	{
		%obj.sortmode = "";
		%obj.filelist.sort(0, 0);
		%obj.folderlist.sort(0, 0);
	}
	else
	{
		%obj.sortmode = "name";
		%obj.filelist.sort(0, 1);
		%obj.folderlist.sort(0, 1);
	}
	NFMGUI.buildfilelist(%obj);
}

function nfm_presssortdate()
{
	%obj = NARGFileManager.pathcontrol[NFMGUI.searchtext, NFMGUI.ext];

	if(!isobject(%obj))
	{
		nfm_debug("cannot sort a list that doesn't exist");
		return;
	}

	if(%obj.sortmode $= "date")
	{
		%obj.sortmode = "";
		%obj.filelist.sortNumerical(1, 1);
	}
	else
	{
		%obj.sortmode = "date";
		%obj.filelist.sortNumerical(1, 0);
	}
	NFMGUI.buildfilelist(%obj);
}

function nfm_clickchangeext()
{
	promptUserText(NFMGUI.ext, "nfm_changeext");
}

function nfm_changeext(%text)
{
	NFMGUI.changeext(%text);
}

//this is the little "v" button
function nfm_pressmore()
{
	%pos = vectoradd(nfmgui_morebutton.nfm_getscreenpos(), "-4 18");
	nfm_moreclickin.resize(getword(%pos, 0), getword(%pos, 1), getword(nfm_moreclickin.getextent(), 0), getword(nfm_moreclickin.getextent(), 1));
	canvas.pushdialog(nfm_morelist);
}

function nfm_showfilewarning(%path)
{
	messageboxok("Warning!", "This file is in a state of limbo.  It exists in Blockland's directory structure but may or may not actually be on your computer.  Attempting to do anything with this file may have unpredictable behaviour.  Complain to Badspot, not me.\n\nFile: " @ %path);
}

