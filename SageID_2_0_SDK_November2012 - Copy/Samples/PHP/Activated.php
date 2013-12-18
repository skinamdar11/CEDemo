<?php

/**
 *	This is the file that SSO redirects to when a user account or password has been activated 
 */

require_once('/services/ActivationService.php');

// ensure page is being POSTed
if($_SERVER['REQUEST_METHOD'] != 'GET'){
	echo "Incorrect HTTP method. Activated only supports GET";
	throw new Exception();
	exit();
}

$service = new ActivationService();
echo $service->HandleActivation();
