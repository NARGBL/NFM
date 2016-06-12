//NARG File Manager
//Version 1.42 Full
//  By Nexus 4833
//=================

//just execute necessary files
exec("./modules/getName/getname.cs");
exec("./modules/zoomImage/zoomimage.cs");

//this module is dependent on the other two
exec("./modules/getFile/getfile.cs");

//this is something that kinda should be a part of the getfile module but also kinda shouldn't idk
exec("./warning.cs");

//in my head it was a cool idea to abstract the specific brick save/load system away from the "prompt file" module
//turns out this adds a lot of unnecessary stuff, oh well.

//package
//----------
package NARGFileManager
{
	function LoadBricksGUI::onWake(%this)
	{
		parent::onWake(%this);

		canvas.popdialog(%this);
		$nfm_overridedefault = "";
		promptUserFile("", "nfm_uploadsavefile", "loadbls");
	}

	function escapeMenu::clickSaveBricks(%this)
	{
		$nfm_overridedefault = "";
		promptUserFile("", "nfm_savebricks", "savebls");
	}

	//this is a crappy workaround since baldspot doesn't have a decent api
	function savebricks(%path, %desc)
	{
		parent::savebricks(%path, %desc);

		if($nfm_overridedefault !$= "")
		{
			filecopy(%path, $nfm_overridedefault);
			filedelete(%path);
			filecopy(nfm_trimFileName(%path) @ filebase(%path) @ ".jpg", nfm_trimFileName($nfm_overridedefault) @ filebase($nfm_overridedefault) @ ".jpg");
			filedelete(nfm_trimFileName(%path) @ filebase(%path) @ ".jpg");

			//this isn't just an error check.
			//this forces blockland's file directory to discover these two files and actually add them.
			//for some reason, filecopy() does not do that automatically.

			if(!isfile($nfm_overridedefault) || !isfile(nfm_trimFileName($nfm_overridedefault) @ filebase($nfm_overridedefault) @ ".jpg"))
			{
				//blockland's file system is a buggy piece of shit
				nfm_debug("Save failure:" SPC $nfm_overridedefault);
			}
			savebricksgui.getobject(0).setvisible(1);
			SaveBricks_Window.add(SaveBricks_DownloadWindow);
			$nfm_overridedefault = "";
		}
	}
};
activatePackage(NARGFileManager);

function nfm_uploadsavefile(%path)
{
	canvas.popdialog(EscapeMenu);
	nfm_setbuildingvars();

	if(!isfile(%path) || fileext(%path !$= ".bls"))
	{
		nfm_debug("Warning: File is not a valid blockland save file: " @ %path);
	}
	$LoadingBricks_FileName = %path;

	if($LoadingBricks_ColorMethod $= "")
	{
		$LoadingBricks_ColorMethod = 3;
	}

	if(serverconnection.islocal())
	{
		serverDirectSaveFileLoad($LoadingBricks_FileName, $LoadingBricks_ColorMethod, $LoadingBricks_Silent, $LoadingBricks_Ownership);
	}
	else
	{
		//This is supposed to be used only for dedicated servers, but it seems to work for non dedicated as well.
		//The only side effect that I have found is a line saying that the host cancelled the upload in chat.
		commandtoserver('InitUploadHandshake');
	}
}

function nfm_savebricks(%path, %desc, %bypasswarn)
{
	canvas.popdialog(EscapeMenu);

	if(!%bypasswarn && $NFM::Warning && (!$NFM::pref::SaveExtendedBrickInfo || !$NFM::pref::SaveOwnership))
	{
		canvas.pushdialog(nfm_warning);
		nfm_warning.savepath = %path;
		nfm_warning.savedesc = %desc;
		return;
	}
	nfm_setbuildingvars();

	if(!isWriteableFileName(%path))
	{
		messageboxok("Error", "Cannot write to file: " @ %path);
		return;
	}

	if(serverconnection.islocal())
	{
		schedule(0, 0, "saveBricks", %path, %desc);
	}
	else
	{
		//this is part of a crappy workaround that I can't get around without rewriting the entire saving system
		savebricksgui.getobject(0).setvisible(0);
		saveBricksGui.add(SaveBricks_DownloadWindow);

		//apparently the process for downloading event info is more complicated than I thought
		//the easiest way to do it is to just shove data into the default gui and pretend to click the button
		canvas.pushdialog(savebricksgui);

		while(isfile("saves/" @ (%garbage = getrandom()) @ ".bls")){}

		savebricks_filename.setvalue(%garbage);
		savebricks_description.setvalue(%desc);
		savebricks_ownership.setvalue($pref::SaveOwnership);
		savebricks_extendedinfo.setvalue($pref::SaveExtendedBrickInfo);

		$nfm_overridedefault = %path;
		savebricks_save();
	}
}

function nfm_setbuildingvars()
{
	//brick load/save options
	//0 - Yours
	//1 - Saved
	//2 - Public
	$pref::SaveExtendedBrickInfo = $NFM::pref::SaveExtendedBrickInfo;
	$pref::SaveOwnership = $NFM::pref::SaveOwnership;
	$LoadingBricks_Ownership = $NFM::LoadingBricks_Ownership;
	$LoadingBricks_PositionOffset = $NFM::LoadingBricks_PositionOffset;
	$LoadingBricks_ColorMethod = $NFM::LoadingBricks_ColorMethod;
}

if($nfm_neo_bound)
	return;
$nfm_neo_bound = true;
$neoName[$neoCount] = "NFM Options";
$neoCmd[$neoCount] = "nfm_options();";
$neoIcon[$neoCount] = "add-ons/client_nfm/neoicon.png"; //defaults to a question mark otherwise
$neoPrefClose[$neoCount] = false; //whether we want to overlay to close when this button is pressed
$neoCount++;

