//version check
//----------

if($narg_versioncheck_version >= 2)
{
	echo("NARG VersionCheck Module is already in place: [v" @ $narg_versioncheck_version @ "]");
	return;
}
$narg_versioncheck_version = 2;

//%version must be float followed by a word.
function narg_versioncheck(%x, %str, %version) //%x is true if version check is manual
{
	%obj = new tcpobject(NARGDownloader)
	{
		manualconnect = %x;
		downloadphase = 0;
		mod = %str;
		oldver = %version;
	};
	%obj.connect("forum.blockland.us:80");
}

function NARGDownloader::onConnected(%this)
{
	if(%this.downloadphase == 0)
	{
		//forum.blockland.us/index.php?topic=161834.720
		%req = "GET /index.php?topic=161834.720 HTTP/1.0\nHost: forum.blockland.us\n\n";
		%this.send(%req);
	}
	else
	{
		%req = "GET /index.php?action=dlattach;topic=161834.0;attach="@ %this.versionfile SPC"HTTP/1.0\nHost: forum.blockland.us\n\n";
		%this.send(%req);
	}
}

function NARGDownloader::onConnectFailed(%this)
{
	if(%this.manualconnect)
		messageboxok("Attention!", "Unable to connect to the online version information.\n\nYou should check online to make sure this File Manager is up to date.");
	%this.delete();
}

function NARGDownloader::onDNSFailed(%this)
{
	if(%this.manualconnect)
		messageboxok("Attention!", "Unable to connect to the online version information.\n\nYou should check online to make sure this File Manager is up to date.");
	%this.delete();
}

function NARGDownloader::onDisconnect(%this)
{
	if(!%this.connected)
	{
		if(%this.manualconnect)
			messageboxok("Attention!", "Unable to connect to the online version information.");
		%this.delete();
	}
}

function NARGDownloader::onLine(%this, %line)
{
	if(%this.downloadphase == 0)
	{
		if(strpos(%line, "Versions.txt") > -1)
		{
			%subline = "http://forum.blockland.us/index.php?action=dlattach;topic=161834.0;attach=";
			%this.versionfile = getsubstr(%line, strpos(%line, %subline)+strlen(%subline), strpos(%line, "\">")-(strpos(%line, %subline)+strlen(%subline)));

			if(mfloor(%this.versionfile) $= %this.versionfile)
			{
				%this.connected = 1;
				%this.disconnect();
				%this.downloadphase = 1;
				%this.connect("forum.blockland.us:80");
			}
			else
			{
				%this.disconnect();

				if(isobject(%this))
					%this.delete();
			}
		}
	}
	else
	{
		%tag = "NARG MOD VERSION " @ %this.mod @ ": ";

		if(getsubstr(%line, 0, strlen(%tag)) $= %tag)
		{
			%this.connected = 1;
			%this.availableversion = getsubstr(%line, 22, strlen(%line));
			%av0 = getword(%this.availableversion, 0);
			%av1 = getword(%this.availableversion, 1);
			%v0 = getword(%this.oldver, 0);
			%v1 = getword(%this.oldver, 1);

			if(%av0 > %v0 || (%av0 $= %v0 && stricmp(%av1, %v1) == 1))
			{
				%this.versionresult(0);
			}
			else
			{
				%this.versionresult(1);
			}
			%this.disconnect();

			if(isobject(%this))
				%this.delete();
		}
	}
}

function NARGDownloader::versionresult(%this, %x)
{
	if(%x)
	{
		if(%this.manualconnect)
			messageboxok("Good News!", "This Add-On (" @ %this.mod @ ") is up to date.");
	}
	else
		messageboxok("Attention!", "There is a more current version of this  Add-On! (" @ %this.mod @ ")\n\nYour Version: "@ %this.oldver @"\n\nAvailable: "@%this.availableversion @"\n\nDownload link: <a:http://forum.blockland.us/index.php?topic=161834.0>Forum Thread</a>");
}

