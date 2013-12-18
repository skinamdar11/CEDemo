<?php

/**
 *	This is the file that the web application posts to when a user wants to start a registration attempt
 */

include_once('./services/RegistrationService.php');
include_once('/helpers/ExceptionHelper.php');

// ensure page is being POSTed
if($_SERVER['REQUEST_METHOD'] != 'POST'){
	echo "Incorrect HTTP method. StartRegisterNewUser only supports POST";
	throw new Exception();
	exit();
}

try{
	$ssoService = new RegistrationService();
	echo $ssoService->StartRegisterNewUser();
}
catch(Exception $e){
	echo ExceptionHelper::GetJsonException($e);
}


?>