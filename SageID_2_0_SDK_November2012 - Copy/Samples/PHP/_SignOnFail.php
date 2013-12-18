<?php

/**
 *	This is the file that SSO redirects to when a user sign on fails
 */

include_once('/services/SessionService.php');
include_once('/helpers/ExceptionHelper.php');

// ensure page is being GETed
if($_SERVER['REQUEST_METHOD'] != 'GET'){
	echo "Incorrect HTTP method. SignOnSuccess only supports GET";
	throw new Exception();
	exit();
}
try{
	$service = new SessionService();
	echo $service->EndSignOn();
}
catch(Exception $e){
	echo ExceptionHelper::GetJsonException($e);
}
?>