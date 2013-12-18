<?php

require_once('/services/SessionService.php');
include_once('/helpers/WebAppSession.php');
include_once('/helpers/SessionHelper.php');

// This will check the current session is valid and redirct to login if it's not
// it also extends the session if it's been flaged as requiring extension
try{
	$service = new SessionService();
	$service->CheckCurrentSessionIsValidAndExtendIfRequired();
}
catch (Exception $e){
	$webAppSession = WebAppSession::CreateErrorSession($e->getMessage());
	SessionHelper::updateSession($webAppSession);
	
	header("Location: /SignIn.php");
}
?>