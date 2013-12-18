<?php

// Include ProtectedPage.php to ensure only users with a valid session can access this page
include_once('ProtectedPage.php');

// requires the session helper as it displays the session id and exipry dates
require_once('/helpers/SessionHelper.php');

$session = SessionHelper::getCurrentSession();

date_default_timezone_set('Europe/London');

?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>Web App P - Home</title>
<link href="styles.css" type="text/css" rel="stylesheet" />

<script type="text/javascript" src="jquery-1.6.1.js"></script>
</head>
<body>
<div id="content" class="content">
<div id="header" class="header">
<table cellpadding="0" cellspacing="0" width="100%">
	<tr>
		<td width="40%" align="left"><span id="LabelSessionId">Session ID: <?php echo $session->SSOSessionId; ?>
		</span><br />
		<span id="LabelSessionExpiry">Expires: <?php echo date('d/m/Y  H:i:s', strtotime($session->SSOSessionExpires)); ?>
		</span></td>
		<td width="20%" align="center"><span
			style="font-weight: bold; font-size: small;">Web App P</span></td>
		<td width="40%" align="right"><span id="LabelUserInfo"><?php echo $session->Username; ?>
		</span></td>
	</tr>
</table>
</div>
<form id="form1">
<table cellspacing="8px">
	<tr>
		<td width="20%"><a href="default.php">Refresh Page</a></td>
		<td>Simulate activity in the web application, causing it to update the
		recorded last activity time and to extend the session if expiry is
		due.</td>
	</tr>
	<tr>
		<td width="20%"><a href="javascript:ChangePassword();">Change Password</a>
		</td>
		<td>Change the signed-on user's password and security questions.</td>
	</tr>
	<tr>
		<td width="20%"><a href="javascript:SignOut();">Sign Out</a></td>
		<td>Sign out of this application only. This application will leave the
		SSO session but the SSO session will continue if there is more than
		one participant application.</td>
	</tr>
	<tr>
		<td width="20%"><a href="javascript:SignOutAll();">Sign Out All</a></td>
		<td>End the SSO session. A Session.Ended notification will be sent to
		all participant applications.</td>
	</tr>
</table>
</form>
<div id="push" class="push"></div>
</div>
<div id="footer" class="footer">
<div id="footerMessage" class="footerMessage">
<p id="result"><?php echo empty($session->ErrorMessage) ? '' : $session->ErrorMessage ; ?></p>
</div>
</div>
<script type="text/javascript">
		function ChangePassword(){
			$.post('_StartPasswordChangeAttempt.php',
					function(data){
						var jsonResult = jQuery.parseJSON(data);
						if(jsonResult.Error){
							$("#result").html(jsonResult.Error.Message);
							return;		    				
						}
				
						window.location = jsonResult.StartPasswordChangeAttemptResult.RedirectUri;
					});
		}
	
		function SignOut(){
        	$.post('_SignOut.php',
        			function(data){ 
						SignOutCompleted(data); 
					});
    	}

		function SignOutAll(){
        	$.post('_SignOutAll.php',
        			function(data){ 
    					SignOutCompleted(data); 
    				});
    	}

    	function SignOutCompleted(data){
    		if(!data){
				// refresh page to force log-out
				window.location = window.location;
			}
			else{
				var jsonResult = jQuery.parseJSON(data);
				if(jsonResult.Error){
					$("#result").html(jsonResult.Error.Message);
					return;    				
				}
			}
    	}
	</script>
</body>
</html>
