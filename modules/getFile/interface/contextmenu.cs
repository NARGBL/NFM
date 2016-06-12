if(isobject(nfm_rightclick))
	nfm_rightclick.delete();

if(isobject(nfm_morelist))
	nfm_morelist.delete();

exec("./guis/rightclick.gui");
exec("./guis/morelist.gui");

//in a right click menu
function nfm_rightclickout::onmousedown(%this, %key, %pos, %count)
{
	canvas.popdialog(nfm_rightclick);
}

function nfm_rightclickout::onrightmousedown(%this, %key, %pos, %count)
{
	canvas.popdialog(nfm_rightclick);
}

//in the pulldown menu
function nfm_moreclickout::onmousedown(%this, %key, %pos, %count)
{
	canvas.popdialog(nfm_morelist);
}

function nfm_moreclickout::onrightmousedown(%this, %key, %pos, %count)
{
	canvas.popdialog(nfm_morelist);
}

//image zoom
function nfm_imgclickin::onmouseup(%this, %key, %pos, %count)
{
	promptUserImage(nfmgui_preview.bitmap);
}

//right click menu options
function nfm_rightclick_presscopy()
{
	NFMGUI.copyfilepath = NFMGUI.selectedfile;
	NFMGUI.cutting = false;
	canvas.popdialog(nfm_rightclick);
}

function nfm_rightclick_presspaste()
{
	nfm_generic_paste();
	canvas.popdialog(nfm_rightclick);
}

function nfm_rightclick_pressrename()
{
	canvas.popdialog(nfm_rightclick);
	canvas.pushdialog(nfm_renamedialog);
	promptUserText(fileName(NFMGUI.selectedfile), "nfm_renamefile");
}

function nfm_rightclick_presscut()
{
	NFMGUI.copyfilepath = NFMGUI.selectedfile;
	NFMGUI.cutting = true;
	canvas.popdialog(nfm_rightclick);
}

function nfm_rightclick_pressdelete()
{
	canvas.popdialog(nfm_rightclick);
	messageboxyesno("Delete file?", "Are you sure you want to delete this file?\n\nFile: " @ NFMGUI.selectedfile, "nfm_rightclick_confirmdelete(\"" @ NFMGUI.selectedfile @ "\");");
}

function nfm_rightclick_confirmdelete(%filepath)
{
	nfm_debug("got delete confirmation for: " @ %filepath);

	filedelete(%filepath);
	NFMGUI.refresh_list();
}

//pulldown menu options
function nfm_morelist_pressnewfolder()
{
	canvas.popdialog(nfm_morelist);
	promptUserText("New Folder", "nfm_newfoldername");
}

function nfm_morelist_presshome()
{
	canvas.popdialog(nfm_morelist);
	NFMGUI.loadpath($NFM::Home);
}

function nfm_morelist_presspaste()
{
	nfm_generic_paste();
	canvas.popdialog(nfm_morelist);
}

function nfm_morelist_pressrefresh()
{
	canvas.popdialog(nfm_morelist);
	NFMGUI.refresh_list();
}

