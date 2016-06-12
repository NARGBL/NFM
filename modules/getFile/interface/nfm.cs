if(isobject(NFMGUI))
	NFMGUI.delete();
exec("./guis/nfm.gui");

//gui manager functions
function NFMGUI::setmode(%this, %mode, %ext)
{
	%this.mode = %mode;

	switch$(%mode)
	{
		case "savebls":
			nfm_debug("setting mode to bls saving");
			%this.action = "save";
			%this.ext = ".bls";
		case "loadbls":
			nfm_debug("setting mode to bls loading");
			%this.action = "load";
			%this.ext = ".bls";
		case "save":
			nfm_debug("setting mode to generic saving");
			%this.action = "save";
			%this.ext = %ext;
		case "load":
			nfm_debug("setting mode to generic loading");
			%this.action = "load";
			%this.ext = %ext;
		default:
			nfm_debug("other mode option");
			%this.action= "";
			%this.ext = %ext;
	}
	%this.refresh_info();
}

//previously setcurrdirectory()
function NFMGUI::loadpath(%this, %path)
{
	if(%this.searchtext $= %path)
		nfm_debug("this path is already in place: " @ %path);
	else
	{
		%this.selectedfile = "";
		%this.searchtext = %path;
	}
	%pathobj = NARGFileManager.pathcontrol[%path, %this.ext];

	if(!isobject(%pathobj))
	{
		nfm_debug("loadpath() needs to fetch new path: " @ %path);
		%this.startloadanimation();
		NARGFileManager.loadpath(%path, "nfm_completeload", %this.ext);
		return;
	}
	%this.currdirectory = %pathobj.path;
	%this.refresh_info();
	%this.buildfilelist(%pathobj);
}

//refreshes everything except the middle section
//previously NFMGUI::refresh()
function NFMGUI::refresh_info(%this)
{
	if(%this.action $= "save")
	{
		nfmgui_description.profile = "GuiMLTextEditProfile";
		nfmgui_saveload.settext("Save");
	}
	else if(%this.action $= "load")
	{
		nfmgui_description.profile = "GuiMLTextProfile";
		nfmgui_saveload.settext("Load");
	}
	else
	{
		nfmgui_description.profile = "GuiMLTextProfile";
		nfmgui_saveload.settext("Save/Load");
	}
	//we cannot use filePath(%path), it cuts off last / if present
	nfmgui_address.settext(%this.searchtext);

	if(%this.selectedfile !$= "")
	{
		if(%this.ext $= fileext(%this.selectedfile))
		{
			nfmgui_exttext.settext(%this.ext);
			nfmgui_filename.settext(filebase(%this.selectedfile));
		}
		else
		{
			nfmgui_exttext.settext("");
			nfmgui_filename.settext(filename(%this.selectedfile));
		}
	}
	else
	{
		nfm_debug("no file selected");

		//don't show file extensions with wildcards
		if(strpos(%this.ext, "*") > -1)
		{
			nfmgui_exttext.settext("");
		}
		else
		{
			nfmgui_exttext.settext(%this.ext);
		}
	}
}

//forces a redraw, %soft indicates if we can accept a cached result
function NFMGUI::refresh_list(%this, %soft)
{
	%obj = NARGFileManager.pathcontrol[%this.searchtext, %this.ext];

	if(isobject(%obj))
	{
		if(%soft)
		{
			nfm_debug("Soft refresh successful");
			%this.buildfilelist(%obj);
			return;
		}
		%obj.delete();
	}
	%this.loadpath(%this.searchtext);
}

function NFMGUI::buildfilelist(%this, %pathobj)
{
	nfm_debug("building file list...");
	nfmgui_filelist.clear();
	%height = 2;
	%width = getword(nfmgui_filelist.getextent(), 0);
	%uipath = nfm_getUIPath();

	//this is a bit of a hack
	//instead of having to resize every element, just set elements to 'width' and resize container
	nfmgui_filelist.resize(1, 1, 312, 2);

	if(%pathobj.sortmode $= "date")
		%filesfirst = true;

	//maybe a for loop isn't the best way to do this but whatever
	for(%section=0; %section<2; %section++)
	{
		if(!%filesfirst)
		{
			for(%i=0; %i<%pathobj.folderlist.rowCount(); %i++)
			{
				%line = %pathobj.folderlist.getrowtext(%i);
				%name = getfield(%line, 0);
				%files = getfield(%line, 1);
				%path = getfield(%line, 2);
				%name = getsubstr(%name, 0, strlen(%name)-1); //chop off the slash

				if($NFM::ListSize == 1)
				{
					//small list, 0 is to display the %pathobj directly but that is not yet supported
					%gui = nfm_getsmallitem(%i + %j, %name, %uipath @ "folder.png", %files SPC "files");
					%height += 22;
				}
				else if($NFM::ListSize == 2)
				{
					//medium
					%gui = nfm_getmediumitem(%i + %j, %name, %uipath @ "folder.png", %files SPC "files");
					%height += 34;
				}
				else if($NFM::ListSize == 3)
				{
					//large
					%gui = nfm_getlargeitem(%i + %j, %name, %uipath @ "folder.png", %files SPC "files in this folder", "");
					%height += 50;
				}
				else
				{
					nfm_debug("invalid list size, harping major darpage");
				}
				%gui.filepath = %path;
				%gui.isfolder = true;
				nfmgui_filelist.add(%gui);
			}
		}
		else
		{		
			for(%j=0; %j<%pathobj.filelist.rowcount(); %j++)
			{
				%line = %pathobj.filelist.getrowtext(%j);
				%name = getfield(%line, 0);
				%date = getfield(%line, 2);
				%bricks = getfield(%line, 3);
				%path = getfield(%line, 4);
		
				if(%bricks $= "-1")
				{
					%warning = true;
				}
		
				if(%bricks $= "" || %warning)
					%bricks = "???";
		
				if(fileext(%name) $= ".bls")
				{
					if(%this.ext $= ".bls" && !%warning)
						%name = filebase(%name);
					%icon = "brick.png";
					%desc = "Brickcount:" SPC %bricks;
				}
				else if(fileext(%name) $= ".jpg" || fileext(%name) $= ".png")
				{
					%icon = "picture.png";
					%desc = "A pretty picture";
				}
				else if(fileext(%name) $= ".cs" || fileext(%name) $= ".gui")
				{
					%icon = "script.png";
					%desc = "A file containing code.";
				}
				else
				{
					%icon = "file.png";
					%desc = "";
				}
		
				if($NFM::ListSize == 1)
				{
					//small list, 0 is to display the %pathobj directly but that is not yet supported
					%gui = nfm_getsmallitem(%i + %j, %name, %uipath @ %icon, %date);
					%height += 22;
				}
				else if($NFM::ListSize == 2)
				{
					//medium
					%gui = nfm_getmediumitem(%i + %j, %name, %uipath @ %icon, %date);
					%height += 34;
				}
				else if($NFM::ListSize == 3)
				{
					//large
					%gui = nfm_getlargeitem(%i + %j, %name, %uipath @ %icon, %date, %desc);
					%height += 50;
				}
				else
				{
					nfm_debug("invalid list size, harping major darpage");
				}
				%gui.filepath = %path;
				%gui.isfolder = false;
		
				if(%path $= %this.selectedfile)
				{
					nfm_debug("caught file");
					%this.selectedfilebg = %gui;
					%gui.nfm_setcolor($NFM::ItemColorSelect);
				}
		
				if(%warning)
				{
					nfm_debug("fixing file icon");
					%pos = %gui.getobject(0).getposition();
					%ext = %gui.getobject(0).getextent();
					%gui.getobject(0).delete();
		
					%newbitmapobj = new GuiBitmapButtonCtrl()
					{
						profile = "BlockButtonProfile";
						horizSizing = "right";
						vertSizing = "bottom";
						position = %pos;
						extent = %ext;
						minExtent = "8 2";
						enabled = "1";
						visible = "1";
						clipToParent = "1";
						command = "nfm_showfilewarning(\"" @ %path @ "\");";
						text = " ";
						groupNum = "-1";
						buttonType = "PushButton";
						bitmap = %uipath @ "warning";
						mColor = "255 255 255 255";
					};
					%gui.add(%newbitmapobj);
		
					for(%k=0; %k<%gui.getcount(); %k++)
					{
						if(%gui.getobject(%k).getname() $= "nfm_btnmousecontrol")
						{
							//for some reason this needs to be corrected
							%gui.pushtoback(%gui.getobject(%k));
							break;
						}
					}
					%gui.pushtoback(%newbitmapobj);
				}
				nfmgui_filelist.add(%gui);
			}
		}
		%filesfirst = !%filesfirst;
	}
	nfmgui_filelist.resize(1, 1, %width, %height);
}

function NFMGUI::startloadanimation(%this)
{
	nfm_debug("starting load animation");

	if(iseventpending(%this.loadanimationschedule))
		cancel(%this.loadanimationschedule);
	nfm_loadingscreen.setvisible(1);
	%this.tickloadanimation(0);
}

function NFMGUI::tickloadanimation(%this, %num)
{
	%text = "Loading";

	for(%i=0; %i<%num; %i++)
		%text = %text @ ".";
	nfm_loadingmessage.settext(%text);
	%num++;

	if(%num > 4)
		%num = 0;
	%this.loadanimationschedule = %this.schedule(500, "tickloadanimation", %num);
}

function NFMGUI::stoploadanimation(%this)
{
	nfm_debug("stopping load animation");

	if(iseventpending(%this.loadanimationschedule))
		cancel(%this.loadanimationschedule);
	nfm_loadingscreen.setvisible(0);
}

function NFMGUI::completeload(%this)
{
	nfm_debug("load completed");
	%this.stoploadanimation();
	%this.loadpath(%this.searchtext);
}

function NFMGUI::renameitem(%this, %text)
{
	nfm_debug("name selected: " @ %text);
	%oldpath = %this.selectedfile;
	%newpath = nfm_trimfilename(%oldpath) @ %text;

	if(isfile(%newpath))
	{
		messageboxok("Error", "A file with that name already exists in this folder.  Please choose a different one.");
	}
	else
	{
		if(isfile(%oldpath))
		{
			nfm_filecopy(%oldpath, %newpath);

			if(isfile(%newpath))
			{
				filedelete(%oldpath);
				%this.refresh_list();
			}
			else
			{
				messageboxok("Error", "Unable to rename file.\n\nFrom: "@ %oldpath @"\nTo: "@ %newpath);
			}
		}
		else
		{
			messageboxok("Error", "Invalid file to rename\n\nFile: " @ %oldpath);
		}
	}
}

function NFMGUI::setcurrentfile(%this, %path)
{
	%this.selectedfile = %path;

	//to do:
	//update bitmap
	//update description
	%pathminusext = nfm_trimfilename(%path) @ filebase(%path);

	if(isfile(%pathminusext @ ".jpg"))
		nfmgui_preview.setbitmap(%pathminusext @ ".jpg");
	else if(isfile(%pathminusext @ ".png"))
		nfmgui_preview.setbitmap(%pathminusext @ ".png");
	else
		nfmgui_preview.setbitmap(nfm_getuipath() @ "default.png");

	if(isfile(%path))
	{
		if(fileext(%path) $= ".bls")
		{
			%file = new FileObject();
			%file.openforread(%path);
			%file.readline(); //warning at beginning
			%dcount = %file.readline(); //number telling us length of description

			if(%dcount >= 1)
				%description = %file.readline();

			for(%a=0; %a<%dcount-1; %a++) //description
				%description = %description NL %file.readline();

			for(%a=0; %a<64; %a++) //color table
				%file.readline();
			%brickcount = getword(%file.readline(), 1);
			%file.close();
			%file.delete();

			%description = strreplace(%description, "\\n", "\n");
			%description = strreplace(%description, "\\\"", "\"");
			nfmgui_description.settext(%description);
			nfmgui_brickcount.settext("Estimated Brickcount: " @ %brickcount);
		}
		else
		{
			nfmgui_description.settext("");
			nfmgui_brickcount.settext("File size: " @ nfm_getFileSize(%path));
		}
	}
	else
	{
		nfmgui_description.settext("");
		nfmgui_brickcount.settext("");
	}
	%this.refresh_info();
}

function NFMGUI::selectfile(%this, %path, %override)
{
	if(!%override && %this.action $= "save" && isfile(%path))
	{
		messageboxyesno("Warning", "Are you sure you want to overwrite this file?\n\nFile: "@ %path, "NFMGUI.selectfile(\"" @ %path @ "\", 1);");
		return;
	}

	if(%this.action $= "save")
	{
		if(!%override && !iswriteablefilename(%path))
		{
			messageboxyesno("Warning", "This is not a writable file.  Try to save anyway?\n\nFile: "@ %path, "NFMGUI.selectfile(\"" @ %path @ "\", 1);");
			return;
		}
		%this.savedirectory = %this.currdirectory;

		%obj = NARGFileManager.pathcontrol[%this.searchtext, %this.ext];

		if(isobject(%obj))
		{
			//this will force a reload next time
			%obj.delete();
		}
	}
	else if(%this.action $= "load")
	{
		%this.loaddirectory = %this.currdirectory;
	}

	nfm_debug("final selection: " @ %path);
	%this.stoploadanimation();
	canvas.popdialog(NFMGUI);
	canvas.popdialog(nfm_tutorial);
	canvas.popdialog(nfm_options);
	canvas.popdialog(nfm_rightclick);
	canvas.popdialog(nfm_morelist);

	//wtf baldspot pls
//	if($LoadingBricks_ColorMethod $= "")
//		$LoadingBricks_ColorMethod = 3;
	call(%this.callback, %path, nfmgui_description.getvalue());
}

function NFMGUI::changeext(%this, %ext)
{
	%this.ext = %ext;
	%this.refresh_info();
	%this.refresh_list(1);
}

