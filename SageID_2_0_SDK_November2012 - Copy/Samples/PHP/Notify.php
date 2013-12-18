<?php

/**
 * The notify page which SSO posts any notifications to.
 * 
 * The notifications are all handled by the NotificationService
 */

require_once('/services/NotificationService.php');

// ensure page is being POSTed
if($_SERVER['REQUEST_METHOD'] != 'POST'){
	echo "Incorrect HTTP method. Notify only supports POST";
	throw new Exception();
	exit();
}

$service = new NotificationService();
echo $service->HandleNotification();

?>