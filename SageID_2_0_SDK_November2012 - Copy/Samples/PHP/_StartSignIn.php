<?php

/**
 *	This is the file that the web application posts to when a user wants to start a sign on attempt 
 */

include_once('/services/SessionService.php');
include_once('/helpers/ExceptionHelper.php');

// ensure page is being POSTed
if($_SERVER['REQUEST_METHOD'] != 'POST'){
	echo "Incorrect HTTP method. StartSignIn only supports POST";
	throw new Exception();
	exit();
}

try{
	$service = new SessionService();
	echo $service->StartSignOn();
}
catch(Exception $e){
	echo ExceptionHelper::GetJsonException($e);
}

?>