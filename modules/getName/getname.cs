if(isobject(nfm_renamedialog))
	nfm_renamedialog.delete();
exec("./renamedialog.gui");

%getname_version = 1.1;

if($getname_version >= %getname_version)
{
	echo("Newer version of getName module already in place");
	return;
}
$getname_version = %getname_version;

//This module prompts the user for text input
//Made by Nexus 4833
//call promptUserText(%default, %callback)

//%default is the default text in the text box
//%callback will be called with the user text as the only argument.
//no callback is provided if the user fails to provide input
//%prompt is an optional argument that will set the title of the window.
//	defaults to "Choose a name"

function promptUserText(%default, %callback, %prompt)
{
	nfm_renamedialog.callback = %callback;
	nfm_renametext.settext(%default);

	if(%prompt !$= "")
		nfm_renamedialog.getobject(0).settext(%prompt);
	else
		nfm_renamedialog.getobject(0).settext("Choose a name");

	//in case this gets stuck behind another gui
	if(nfm_renamedialog.isawake())
		canvas.popdialog(nfm_renamedialog);
	canvas.pushdialog(nfm_renamedialog);
}

function nfm_renameok()
{
	canvas.popdialog(nfm_renamedialog);
	%text = nfm_renametext.getvalue();
	%callback = nfm_renamedialog.callback;
	nfm_renamedialog.callback = "";
	call(%callback, %text);
}

