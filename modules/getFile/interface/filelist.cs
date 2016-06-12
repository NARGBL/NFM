//mouse control event functions for each item in list
function nfm_btnmousecontrol::onmousedragged(%this, %key, %pos, %count)
{
	//to do maybe
}

function nfm_btnmousecontrol::onmousedown(%this, %key, %pos, %count)
{
	%this.getgroup().nfm_setcolor($NFM::ItemColorPress);
}

function nfm_btnmousecontrol::onmouseup(%this, %key, %pos, %count)
{
	if(%this.getgroup().isfolder)
	{
		//folder cannot be the selectedfile
		%this.getgroup().nfm_setcolor($NFM::ItemColorIdle);

		if(%count >= 2)
		{
			NFMGUI.loadpath(%this.getgroup().filepath);
		}
	}
	else
	{
		%this.getgroup().nfm_setcolor($NFM::ItemColorSelect);

		if(NFMGUI.selectedfile !$= %this.getgroup().filepath)
		{
			nfm_debug("mouseup on file with no mouse down, meh");
			if(isobject(NFMGUI.selectedfilebg))
				NFMGUI.selectedfilebg.nfm_setcolor($NFM::ItemColorIdle);
			NFMGUI.setcurrentfile(%this.getgroup().filepath);
			NFMGUI.selectedfilebg = %this.getgroup();
			return;
		}

		if(%count >= 2)
		{
			nfm_debug("file clicked: " @ %this.getgroup().filepath);
			NFMGUI.selectfile(%this.getgroup().filepath);
		}
	}
}

function nfm_btnmousecontrol::onmouseenter(%this, %key, %pos, %count)
{
	%this.getgroup().nfm_setcolor($NFM::ItemColorHighlight);
}

function nfm_btnmousecontrol::onmouseleave(%this, %key, %pos, %count)
{
	if(NFMGUI.selectedfile $= %this.getgroup().filepath)
		%this.getgroup().nfm_setcolor($NFM::ItemColorSelect);
	else
		%this.getgroup().nfm_setcolor($NFM::ItemColorIdle);
}

function nfm_btnmousecontrol::onrightmousedown(%this, %key, %pos, %count)
{
	%this.getgroup().nfm_setcolor($NFM::ItemColorContext);

	if(!%this.getgroup().isfolder)
	{
		if(NFMGUI.selectedfile !$= %this.getgroup().filepath)
		{
			if(isobject(NFMGUI.selectedfilebg))
				NFMGUI.selectedfilebg.nfm_setcolor($NFM::ItemColorIdle);
			NFMGUI.setcurrentfile(%this.getgroup().filepath);
			NFMGUI.selectedfilebg = %this.getgroup();
		}
	}
}

function nfm_btnmousecontrol::onrightmouseup(%this, %key, %pos, %count)
{
	if(%this.getgroup().isfolder)
	{
		//folder cannot be the selectedfile
		%this.getgroup().nfm_setcolor($NFM::ItemColorIdle);
	}
	else
	{
		if(NFMGUI.selectedfile !$= %this.getGroup().filepath)
		{
			nfm_debug("Right click up on not selected context!");
			if(isobject(NFMGUI.selectedfilebg))
				NFMGUI.selectedfilebg.nfm_setcolor($NFM::ItemColorIdle);
			NFMGUI.setcurrentfile(%this.getgroup().filepath);
			NFMGUI.selectedfilebg = %this.getgroup();
		}
		%this.getgroup().nfm_setcolor($NFM::ItemColorSelect);
		%pos = vectoradd(%pos, %this.nfm_getscreenpos);
		nfm_rightclickin.resize(getword(%pos, 0), getword(%pos, 1), getword(nfm_rightclickin.getextent(), 0), getword(nfm_rightclickin.getextent(), 1));
		canvas.pushdialog(nfm_rightclick);
	}
}

function nfm_list_item::nfm_setcolor(%this, %color)
{
	%this.setcolor(%color);
	%this.getobject(1).getobject(1).mcolor = %color;
}

//gui construction functions
//----------
function nfm_getsmallitem(%num, %name, %icon, %rest)
{
	%height = 1 + 22*%num;

	%gui = new GuiSwatchCtrl(nfm_list_item)
	{
		profile = "GuiDefaultProfile";
		horizSizing = "width";
		vertSizing = "bottom";
		position = "1" SPC %height;
		extent = "310 20";
		minExtent = "8 2";
		enabled = "1";
		visible = "1";
		clipToParent = "1";
		color = $NFM::ItemColorIdle;

		new GuiBitmapCtrl()
		{
			profile = "GuiDefaultProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "4 1";
			extent = "16 16";
			minExtent = "8 2";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			bitmap = %icon;
			wrap = "0";
			lockAspectRatio = "0";
			alignLeft = "0";
			alignTop = "0";
			overflowImage = "0";
			keepCached = "0";
			mColor = "255 255 255 255";
			mMultiply = "0";
		};

		new GuiSwatchCtrl()
		{
			profile = "GuiDefaultProfile";
			horizSizing = "width";
			vertSizing = "bottom";
			position = "26 1";
			extent = "164 18";
			minExtent = "16 16";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			color = "0 0 0 0";

			new GuiTextCtrl()
			{
				profile = "NFMTextProfile";
				horizSizing = "width";
				vertSizing = "bottom";
				position = "0 0";
				extent = "164 18";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				text = %name;
				maxLength = "255";
			};
	
			new GuiBitmapCtrl()
			{
				profile = "GuiDefaultProfile";
				horizSizing = "left";
				vertSizing = "height";
				position = "148 0";
				extent = "16 18";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				bitmap = nfm_getUIPath()@ "transition.png";
				wrap = "0";
				lockAspectRatio = "0";
				alignLeft = "0";
				alignTop = "0";
				overflowImage = "0";
				keepCached = "0";
				mColor = $NFM::ItemColorIdle;
				mMultiply = "0";
			};
		};

		new GuiTextCtrl()
		{
			profile = "GuiTextProfile";
			horizSizing = "left";
			vertSizing = "bottom";
			position = "194 1";
			extent = "29 18";
			minExtent = "8 2";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			text = %rest;
			maxLength = "255";
		};

		new GuiMouseEventCtrl(nfm_btnmousecontrol)
		{
			profile = "GuiDefaultProfile";
			horizSizing = "width";
			vertSizing = "height";
			position = "0 0";
			extent = "310 20";
			minExtent = "8 2";
			visible = "1";
			lockMouse = "0";
		};
	};
	return %gui;
}

function nfm_getmediumitem(%num, %name, %icon, %rest)
{
	%height = 1 + 34*%num;

	%gui = new GuiSwatchCtrl(nfm_list_item)
	{
		profile = "GuiDefaultProfile";
		horizSizing = "width";
		vertSizing = "bottom";
		position = "1" SPC %height;
		extent = "310 32";
		minExtent = "8 2";
		enabled = "1";
		visible = "1";
		clipToParent = "1";
		color = $NFM::ItemColorIdle;

		new GuiBitmapCtrl()
		{
			profile = "GuiDefaultProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "4 4";
			extent = "24 24";
			minExtent = "8 2";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			bitmap = %icon;
			wrap = "0";
			lockAspectRatio = "0";
			alignLeft = "0";
			alignTop = "0";
			overflowImage = "0";
			keepCached = "0";
			mColor = "255 255 255 255";
			mMultiply = "0";
		};

		new GuiSwatchCtrl()
		{
			profile = "GuiDefaultProfile";
			horizSizing = "width";
			vertSizing = "bottom";
			position = "33 8";
			extent = "157 16";
			minExtent = "16 16";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			color = "0 0 0 0";

			new GuiMLTextCtrl()
			{
				profile = "GuiMLTextProfile";
				horizSizing = "width";
				vertSizing = "bottom";
				position = "0 0";
				extent = "285 16";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				lineSpacing = "2";
				allowColorChars = "0";
				maxChars = "-1";
				text = "<font:" @ $NFM::Font @ ":16>" @ %name;
				maxBitmapHeight = "-1";
				selectable = "1";
				autoResize = "0";
			};
	
			new GuiBitmapCtrl()
			{
				profile = "GuiDefaultProfile";
				horizSizing = "left";
				vertSizing = "height";
				position = "141 0";
				extent = "16 16";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				bitmap = nfm_getUIPath()@ "transition.png";
				wrap = "0";
				lockAspectRatio = "0";
				alignLeft = "0";
				alignTop = "0";
				overflowImage = "0";
				keepCached = "0";
				mColor = $NFM::ItemColorIdle;
				mMultiply = "0";
			};
		};

		new GuiTextCtrl()
		{
			profile = "GuiTextProfile";
			horizSizing = "left";
			vertSizing = "bottom";
			position = "194 7";
			extent = "29 18";
			minExtent = "8 2";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			text = %rest;
			maxLength = "255";
		};

		new GuiMouseEventCtrl(nfm_btnmousecontrol)
		{
			profile = "GuiDefaultProfile";
			horizSizing = "width";
			vertSizing = "height";
			position = "0 0";
			extent = "310 32";
			minExtent = "8 2";
			visible = "1";
			lockMouse = "0";
		};
	};
	return %gui;
}

function nfm_getlargeitem(%num, %name, %icon, %rest, %moredata)
{
	%height = 1 + 50*%num;

	%gui = new GuiSwatchCtrl(nfm_list_item)
	{
		profile = "GuiDefaultProfile";
		horizSizing = "width";
		vertSizing = "bottom";
		position = "1" SPC %height;
		extent = "310 48";
		minExtent = "8 2";
		enabled = "1";
		visible = "1";
		clipToParent = "1";
		color = $NFM::ItemColorIdle;

		new GuiBitmapCtrl()
		{
			profile = "GuiDefaultProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "5 4";
			extent = "38 38";
			minExtent = "8 2";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			bitmap = %icon;
			wrap = "0";
			lockAspectRatio = "0";
			alignLeft = "0";
			alignTop = "0";
			overflowImage = "0";
			keepCached = "0";
			mColor = "255 255 255 255";
			mMultiply = "0";
		};

		new GuiSwatchCtrl()
		{
			profile = "GuiDefaultProfile";
			horizSizing = "width";
			vertSizing = "bottom";
			position = "50 8";
			extent = "140 18";
			minExtent = "16 16";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			color = "0 0 0 0";

			new GuiMLTextCtrl()
			{
				profile = "GuiMLTextProfile";
				horizSizing = "width";
				vertSizing = "bottom";
				position = "0 0";
				extent = "204 18";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				lineSpacing = "2";
				allowColorChars = "0";
				maxChars = "-1";
				text = "<font:" @ $NFM::Font @ ":18>" @ %name;
				maxBitmapHeight = "-1";
				selectable = "1";
				autoResize = "0";
			};
	
			new GuiBitmapCtrl()
			{
				profile = "GuiDefaultProfile";
				horizSizing = "left";
				vertSizing = "height";
				position = "124 0";
				extent = "16 18";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				bitmap = nfm_getUIPath()@ "transition.png";
				wrap = "0";
				lockAspectRatio = "0";
				alignLeft = "0";
				alignTop = "0";
				overflowImage = "0";
				keepCached = "0";
				mColor = $NFM::ItemColorIdle;
				mMultiply = "0";
			};
		};

		new GuiTextCtrl()
		{
			profile = "GuiTextProfile";
			horizSizing = "left";
			vertSizing = "bottom";
			position = "194 7";
			extent = "29 18";
			minExtent = "8 2";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			text = %rest;
			maxLength = "255";
		};

		new GuiTextCtrl()
		{
			profile = "GuiTextProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "50 24";
			extent = "109 18";
			minExtent = "8 2";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			text = %moredata;
			maxLength = "255";
		};

		new GuiMouseEventCtrl(nfm_btnmousecontrol)
		{
			profile = "GuiDefaultProfile";
			horizSizing = "width";
			vertSizing = "height";
			position = "0 0";
			extent = "310 48";
			minExtent = "8 2";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			lockMouse = "0";
		};
	};
	return %gui;
}

