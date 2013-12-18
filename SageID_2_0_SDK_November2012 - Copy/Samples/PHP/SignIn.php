<?php
// include public page which redirects to the default page if an authenticated session exists
include_once('PublicPage.php');

// include session helper for any error messages
require_once('/helpers/SessionHelper.php');

$session = SessionHelper::getCurrentSession()

?>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>Web Application P - Sign In</title>
<link href="styles.css" type="text/css" rel="stylesheet" />
<script type="text/javascript" src="jquery-1.6.1.js"></script>
</head>
<body>
<div id="content" class="content">
<div id="header" class="header">
<table cellpadding="0" cellspacing="0" width="100%">
	<tr>
		<td width="40%" align="left">&nbsp;</td>
		<td width="20%" align="center"><span
			style="font-weight: bold; font-size: small;">Web App P</span></td>
		<td width="40%" align="right"><span id="LabelUserInfo">Not Signed In</span>
		</td>
	</tr>
</table>
</div>
<table cellspacing="8px">
	<tr>
		<td width="30%"><a href="javascript:StartSignOn();">Sign In</a></td>
		<td>Sign into this application using Sage SSO.</td>
	</tr>
	<tr>
		<td width="20%"><a href="/SignUp.php">Sign Up</a></td>
		<td>Register a new or existing Sage SSO user for this application ay.</td>
	</tr>
</table>
<div id="push" class="push"></div>
</div>
<div id="footer" class="footer">
<div id="footerMessage" class="footerMessage">
<p id="result"><?php echo empty($session->ErrorMessage) ? '' : $session->ErrorMessage ; ?></p>
</div>
</div>
<script type="text/javascript">
    	function StartSignOn(){
        	$.post('_StartSignIn.php',
    	    		function(data) {
	    				var jsonResult = jQuery.parseJSON(data);
	    				if(jsonResult.Error){
	    					$("#result").html(jsonResult.Error.Message);
	    					return;		    				
	    				}
    					window.location = jsonResult.StartSignOnAttemptResult.RedirectUri;
	    			});
    	}
    </script>
</body>
</html>
<?php
// clear any displayed error messages from session
SessionHelper::clearCurrentSession();
?>
