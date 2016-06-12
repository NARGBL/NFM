//warning.cs

exec("./warning.gui");

function nfm_warning::presscancel(%this)
{
	canvas.popdialog(%this);
	canvas.pushdialog(nfmgui);
}

function nfm_warning::pressok(%this)
{
	canvas.popdialog(%this);
	nfm_savebricks(%this.savepath, %this.savedesc, true);
}

function nfm_warning::pressignore(%this)
{
	canvas.popdialog(%this);
	$NFM::Warning = false;
	nfm_options.loadvalues();
	nfm_options.clickapply();
	nfm_savebricks(%this.savepath, %this.savedesc, true);
}

