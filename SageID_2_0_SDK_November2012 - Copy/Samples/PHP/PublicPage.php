<?php

require_once('/helpers/SessionHelper.php');

if (SessionHelper::isCurrentSessionValid()) {
	header("Location: /Home.php");	
	die();
}

?>