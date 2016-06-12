if(isobject(nfm_tutorial))
	nfm_tutorial.delete();
exec("./guis/tutorial.gui");

//tutorial
function nfm_tutorial_mousectrl::onmouseup(%this, %key, %pos, %count)
{
	promptUserImage(nfm_getuipath() @ "default.png");
}

function nfm_tutorial::next(%this)
{
	if(nfm_tutorial_page1.isvisible())
	{
		nfm_tutorial_page1.setvisible(0);
		nfm_tutorial_page2.setvisible(1);
	}
	else if(nfm_tutorial_page2.isvisible())
	{
		nfm_tutorial_page2.setvisible(0);
		nfm_tutorial_page3.setvisible(1);
	}
	else if(nfm_tutorial_page3.isvisible())
	{
		nfm_tutorial_page3.setvisible(0);
		nfm_tutorial_page4.setvisible(1);
	}
}

function nfm_tutorial::prev(%this)
{
	if(nfm_tutorial_page2.isvisible())
	{
		nfm_tutorial_page1.setvisible(1);
		nfm_tutorial_page2.setvisible(0);
	}
	else if(nfm_tutorial_page3.isvisible())
	{
		nfm_tutorial_page2.setvisible(1);
		nfm_tutorial_page3.setvisible(0);
	}
	else if(nfm_tutorial_page4.isvisible())
	{
		nfm_tutorial_page3.setvisible(1);
		nfm_tutorial_page4.setvisible(0);
	}
}

