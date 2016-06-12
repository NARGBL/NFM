//init
//----------
function nfm_initmanager()
{
	while(isobject(NARGFileManager))
	{
		NARGFileManager.clear();
		NARGFileManager.delete();
	}

	new GuiControl(NARGFileManager)
	{
		pathcount = 0;
	};
}

//internals
//----------
function NARGFileManager::loadpath(%this, %fullpath, %callback, %ext)
{
	NFM_Debug("loadpath called: " @ %fullpath);

	if(isobject(%this.pathcontrol[%fullpath, %ext]))
	{
		nfm_debug("existing search requested: " @ %fullpath);
		%this.pathcontrol[%fullpath, %ext].loadComplete(%callback);
	}
	else
	{
		%pathmod = filename(%fullpath);
		%path = nfm_trimfilename(%fullpath);

		//first find any wildcards in path
		%x = strpos(%path, "*");

		if(%x > -1)
		{
			nfm_debug("nested wildcard found, hope this works");
			%newpath = nfm_trimfilename(getsubstr(%path, 0, %x));
			%pathmod = filename(getsubstr(%path, 0, %x)) @ getsubstr(%path, %x, strlen(%path) - strlen(%newpath)) @ %pathmod;
			%path = %newpath;
			nfm_debug("compare: " @ %pathmod @ " with " @ getsubstr(%fullpath, strlen(%path), strlen(%fullpath) - strlen(%path)));
		}
		nfm_debug("loading path: " @ %path @ " with mod " @ %pathmod @ " and ext: " @ %ext);

		%obj = new ScriptObject(NFM_PathControl)
		{
			path = %path;
			mod = %pathmod;
			ext = %ext;
		};

		%obj.filelist = new GUITextListCtrl()
		{
			//name, sortdate, date, brickcount, fullpath
			columns = "0 1 2 3 4";
		};
		%this.add(%obj.filelist);

		%obj.folderlist = new GUITextListCtrl()
		{
			//name, filecount, fullpath
			columns = "0 1 3";
		};
		%this.add(%obj.folderlist);
		%this.pathcontrol[%fullpath, %ext] = %obj;
		NFM_Debug("finish object creation");
		%this.path[%this.pathcount] = %fullpath;
		%this.pathcount++;

		%obj.startbuffer(%callback);
	}
}

//support functions for path control
//----------
function NFM_PathControl::startbuffer(%this, %callback)
{
	NFM_Debug("Starting buffer for " @ %this.path @ " with callbackobj: " @ %callback);
	//starts tick based load schedule
	//avoids lagging client heavily while information is being read and cached
	//assumes that the proper gui controls are already in place

	%item = findFirstFile(%this.path @ %this.mod @ "*" @ %this.ext);

	%this.nextbuffer(%item, %callback);
}

function NFM_PathControl::nextBuffer(%this, %next, %callback)
{
	nfm_debug("nextbuffer called");
	%item = findFirstFile(%next); //reset file search in case of hooligans
	//we don't need to check this isn't an empty string because addItem() does this for us

	for(%actions=0; (!$NFM::Buffer || %actions<$NFM::BufferCount); %actions++)
	{
		if(%item $= "")
		{
			//terminate
			%this.nfm_sort();
			%this.loadComplete(%callback);
			return;
		}
		%name = getsubstr(%item, strlen(%this.path), strlen(%item));
		%x=strpos(%name, "/");
	
		if(%x > -1)
		{
			//folder includes the / in the name
			%folder = getsubstr(%name, 0, %x + 1);
	
			if(%this.fldr[%folder])
			{
				//At one point I would read how many files were in a certain folder
				//and then skip over that many files.  It turns out that this doesn't
				//work if any files are changed at all since directory structure is
				//not maintained after initialization.
				%item = findNextFile(%this.path @ %this.mod @ "*" @ %this.ext);
				continue;
			}
			nfm_debug("adding folder: " @ %folder);
			%this.fldr[%folder] = true;

			//APPARENTLY CALLING GETFILECOUNT() BREAKS FILE SEARCHES
			//THANKS A LOT BALDSPOT
			%count = getFileCount(%this.path @ %folder @ "*" @ %this.ext);
			findFirstFile(%item);

			%this.folderlist.addrow(%this.folderlist.rowcount(), %folder TAB %count TAB getsubstr(%item, 0, strlen(%item) - strlen(%name) + %x+1));
		}
		else //file
		{
			if(!isfile(%item))
			{
				nfm_debug("Error with torque's file manager, try to continue...");
				%brickcount = "-1";
			}
			else if($NFM::BrickCount && fileext(%name) $= ".bls") //as you can see, a lot of extra work is involved to get brickcount
			{
				%file = new FileObject();
				%file.openforread(%this.path @ %name);
				%file.readline(); //warning at beginning
				%dcount = %file.readline(); //number telling us length of description

				for(%a=0; %a<%dcount; %a++) //description
					%file.readline();

				for(%a=0; %a<64; %a++) //color table
					%file.readline();
				//here's what we care about
				%brickcount = getword(%file.readline(), 1);
				%file.close();
				%file.delete();
			}
			else
				%brickcount = "";
//			nfm_debug("adding file: " @ %name);
			%this.filelist.addrow(%this.filelist.rowcount(), %name TAB getFileModifiedSortTime(%item) TAB getFileModifiedTime(%item) TAB %brickcount TAB %item);
		}
		%item = findNextFile(%this.path @ %this.mod @ "*" @ %this.ext);
	}
	%this.bufsched = %this.schedule($NFM::BufferTime, "nextbuffer", %item, %callback);
	nfm_debug("next buffer scheduled");
}

function NFM_PathControl::loadComplete(%this, %callback)
{
	NFM_Debug("Load Complete for NFM_PathControl ID: " @ %this);
	nfm_debug("now calling: " @ %callback);
	call(%callback);

	if(!$NFM::Cache)
	{
		nfm_debug("deleting file/folder cache for: " @ %this.path);
		%this.delete();
	}
}

//not actually ever called by anything, but still here because why not
function NFM_PathControl::cancelload(%this)
{
	if(isEventPending(%this.bufsched))
		cancel(%this.bufsched);
}

function NFM_PathControl::nfm_sort(%this)
{
	if($NFM::defaultSort == 1)
	{
		%this.sortmode = "date";
		%this.filelist.sortNumerical(1, 0);
	}
	else
	{
		%this.sortmode = "name";
		%this.filelist.sort(0, 1);
	}
	%this.folderlist.sort(0, 1);
}

