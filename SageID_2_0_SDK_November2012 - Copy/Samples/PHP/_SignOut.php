<?php

/**
 *	This is the file that the web application posts to when a user wants to sign out 
 */


include_once('/services/SessionService.php');

// ensure page is being POSTed
if($_SERVER['REQUEST_METHOD'] != 'POST'){
	echo "Incorrect HTTP method. SignOut only supports POST";
	throw new Exception();
	exit();
}

$service = new SessionService();
echo $service->SignOut();
?>