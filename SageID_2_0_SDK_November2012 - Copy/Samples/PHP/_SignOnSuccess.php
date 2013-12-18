<?php

/**
 *	This is the file that SSO redirects to when a user sign on succeeds 
 */


include_once('/services/SessionService.php');

// ensure page is being GETed
if($_SERVER['REQUEST_METHOD'] != 'GET'){
	echo "Incorrect HTTP method. SignOnSuccess only supports GET";
	throw new Exception();
	exit();
}

$service = new SessionService();
echo $service->EndSignOn();
?>