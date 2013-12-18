<?php

/**
 *	This is the file that the SSO redirects to when an new user has registered 
 *
 * 	This page hands off to the registration service and handles both successfull and failed registrations
 */

include_once('./services/RegistrationService.php');

// ensure page is being GETed

if($_SERVER['REQUEST_METHOD'] != 'GET'){
	echo "Incorrect HTTP method. SignUpNewResult only supports GET";
	throw new Exception();
	exit();
}

$ssoService = new RegistrationService();
echo $ssoService->EndRegisterNewUser();
?>