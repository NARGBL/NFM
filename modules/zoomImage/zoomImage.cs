//Image zoom
if(isobject(nfm_imagezoom))
	nfm_imagezoom.delete();
exec("./imagezoom.gui");

%zoomimage_version = 1.0;

if($zoomimage_version > %zoomimage_version)
{
	echo("Newer version of zoomImage module already in place");
	return;
}
$zoomimage_version = %zoomimage_version;

//This module shows the user an image as a large overlay on their screen (easily closed)
//Made by Nexus 4833
//call promptUserImage(%bitmap)

//The bitmap should be the full path to the image you want to display
//note that stuff like "./picture.png" may not work since this is might be in a different folder

function promptUserImage(%bitmap)
{
	nfm_imagezoom_display.setbitmap(%bitmap);

	if(nfm_imagezoom.isawake())
		canvas.popdialog(nfm_imagezoom);
	canvas.pushdialog(nfm_imagezoom);
}

function nfm_imgclickout::onmouseup(%this, %key, %pos, %count)
{
	canvas.popdialog(nfm_imagezoom);
}

function nfm_imgclickout::onrightmouseup(%this, %key, %pos, %count)
{
	canvas.popdialog(nfm_imagezoom);
}

