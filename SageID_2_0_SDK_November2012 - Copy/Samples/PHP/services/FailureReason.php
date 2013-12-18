<?php

/**
 * 
 * Some constants for failure reasons and error messages
 * @author ben.crinion
 *
 */

class FailureReason{
	const SignOnCancelled = "SignOnCancelled";
	const AccountPasswordReset = "AccountPasswordReset";
	const ValidationFailed = "ValidationFailed";
	const AccountBlocked = "AccountBlocked";
	const AccountExpired = "AccountExpired";
	const AccountHardLocked = "AccountHardLocked";
	const AccountSoftLocked = "AccountSoftLocked";
	const AccountNotActivated = "AccountNotActivated";
	const AccountNotRegistered = "AccountNotRegistered";
	const SessionExpired = "SessionExpired";
	const UnknownAccount = "UnknownAccount";
	const ProtocolViolation = "ProtocolViolation";
	const SignOnAttemptNotFound = "SignOnAttemptNotFound";
}

class ErrorMessages{
	const AccountPasswordResetMessageFormat ="Password recovery successful. An activation email was sent which includes the following activation link: <a href=\"%s\">Activation Link</a>. (%s)";
	const ValidationFailedMessageFormat = "Password recovery unsuccessful. (%s)";
	const AccountBlockedMessageFormat = "Account has been blocked for this application. (%s)";
	const AccountExpiredMessageFormat = "Account has expired. (%s)";
	const AccountHardLockedMessageFormat = "Account has been hard locked and must be unlocked by an administrator. (%s)";
	const AccountSoftLockedMessageFormat = "Account has been temporarily locked. (%s)";
	const AccountNotActivatedMessageFormat = "Account is not activated. (%s)";
	const AccountNotRegisteredMessageFormat = "Account exists but is not registered for this application. (%s)";
	const SessionExpiredMessageFormat = "The sign-in page expired. (%s)";
	const GeneralErrorMessageFormat = "An error occured during sign-in. (%s)";
}

?>