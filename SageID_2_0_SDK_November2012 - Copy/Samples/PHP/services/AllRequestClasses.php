<?php

/**
 * This file aggregates all the Request classes to make include() simpler for the service classes
 */

include_once('/services/requests/StartSignOnAttemptRequest.php');
include_once('/services/requests/EndSignOnAttemptRequest.php');
include_once('/services/requests/StartNewUserRegistrationAttemptRequest.php');
include_once('/services/requests/StartExistingUserRegistrationAttemptRequest.php');
include_once('/services/requests/EndRegistrationAttemptRequest.php');
include_once('/services/requests/SessionRemoveParticipantRequest.php');
include_once('/services/requests/SessionSignOffRequest.php');
include_once('/services/requests/SessionExtendRequest.php');
include_once('/services/requests/ActivationLinkContextRequest.php');
include_once('/services/requests/StartPasswordChangeAttemptRequest.php');
include_once('/services/requests/EndPasswordChangeAttemptRequest.php');

?>
