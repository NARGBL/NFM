//extra stuff
//----------
function NFM_debug(%line)
{
	if($nfm_debug)
		echo(%line);
}	

//These next two functions should probably be somewhere else, I'm bad at organizing stuff
function nfm_generic_paste()
{
	if(isfile(NFMGUI.copyfilepath))
	{
		%newpath = NFMGUI.currdirectory @ filename(NFMGUI.copyfilepath);

		if(isfile(%newpath))
		{
			messageboxok("Error", "File already exists, Please pick a different location or delete the existing file.\n\nFile: " @ %newpath);
		}
		else
		{
			//default fileCopy() actually just doesn't work at all apparently
			//homestly it just doesn't do anything like wtf
			nfm_filecopy(NFMGUI.copyfilepath, %newpath);

			if(isfile(%newpath))
			{
				%obj = NARGFileManager.pathcontrol[NFMGUI.searchtext, NFMGUI.ext];

				if(NFMGUI.cutting)
				{
					fileDelete(NFMGUI.copyfilepath);
					%otherobj = NARGFileManager.pathcontrol[nfm_getdirectoryup(NFMGUI.copyfilepath), NFMGUI.ext];

					if(isobject(%otherobj))
					{
						%otherobj.delete();
					}
					NFMGUI.cutting = false;
					NFMGUI.copyfilepath = "";
				}

				if(isobject(%obj))
				{
					%obj.filelist.addrow(%obj.filelist.rowcount(), filename(%newpath) TAB getFileModifiedSortTime(%newpath) TAB getFileModifiedTime(%newpath) TAB "" TAB %newpath);
					%obj.sortmode = "";

					//NFMGUI.buildfilelist(%obj);
				}
				NFMGUI.loadpath(NFMGUI.searchtext);
			}
			else
			{
				messageboxok("Error", "File copy failed.\n\nSource: " @ NFMGUI.copyfilepath @ "\nDest: " @ %newpath);
			}
		}
	}
	else
	{
		messageboxok("Error", "Source file does not exist.\n\nFile: " @ NFMGUI.copyfilepath);
	}
}

function nfm_filecopy(%source, %dest)
{
	nfm_debug("nfm_filecopy(\"" @ %source @ "\", \"" @ %dest @ "\");");
	filecopy(%source, %dest);
}

function guicontrol::nfm_getScreenPos(%this)
{
	%obj = %this;
	%pos = "0 0";

	while(isObject(%obj.getgroup()))
	{
		%pos = vectoradd(%pos, %obj.getposition());
		%obj = %obj.getgroup();
	}
	return %pos;
}

function nfm_trimFileName(%path)
{
	return getsubstr(%path, 0, strlen(%path) - strlen(filename(%path)));
}

//edge cases:
// toplevel.file returns ""
// toplevel/sub.file returns "toplevel/"
// toplevel/sublevel/ returns "toplevel/"
// toplevel/ returns ""
// "" returns ""

function nfm_getDirectoryUp(%fullpath)
{
	nfm_debug("dirup: " @ %fullpath SPC "to" SPC %newpath);
	%filename = filename(%fullpath);

	if(%filename !$= "")
	{
		nfm_debug("easy dirup");
		return getsubstr(%fullpath, 0, strlen(%fullpath) - strlen(%filename));
	}
	%x = 0;
	%path = %fullpath;
	%pieces = getcharcount(%fullpath, "/");

	for(%a=0; %a<%pieces; %a++)
	{
		%path = getsubstr(%path, %x, strlen(%path));
		%x = strpos(%path, "/") + 1;
	}
	%newpath = getsubstr(%fullpath, 0, strlen(%fullpath) - strlen(%path));
	return %newpath;
}

function nfm_getFileSize(%path)
{
	%bytes = getfilelength(%path);

	%kb = mfloor(%bytes / 1000);

	if(%kb >= 10)
		return %kb SPC "KB";
	else
		return %bytes SPC "Bytes";
}

