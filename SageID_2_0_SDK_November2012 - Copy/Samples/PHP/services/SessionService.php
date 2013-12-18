<?php

include_once('/services/ServiceBase.php');
require_once('/helpers/SessionHelper.php');
require_once('/helpers/WebAppSession.php');
require_once('/services/FailureReason.php');

/**
 *
 * A class to handle all "Session" functionality like sign in, sign out, extending session etc
 * @author ben.crinion
 *
 */
class SessionService extends ServiceBase{

	function __construct(){
		parent::__construct();
	}

	/**
	 *
	 * Starts a sign on attempt, should return a redirect URI to the SSO site
	 */
	function StartSignOn(){
		// get the parameter object to send to StartSignOnAttempt
		$requestObj = new StartSignOnAttemptRequest();

		try {
			// make the call to obsidion
			$result = $this->soapClient->StartSignOnAttempt(array('request' => $requestObj));
		} catch (Exception $e) {
			// if the obsidion call fails, wrap up the error as a JSON object and return
			return $this->GetJsonException($e);
		}

		// if for some reason the service call doesn't return a StartSignOnAttemptResult...
		if(empty($result->StartSignOnAttemptResult)){
			// ...return a JSON error
			return $this->GetJsonException(new Exception('Empty result returned from StartSignOnAttempt'));
		}

		// cache the sign on attempt id in session so we can check it later
		$attemptId = $result->StartSignOnAttemptResult->SignOnAttemptId;
		$s = new WebAppSession();
		$s->SignOnAttemptId = $attemptId;
		SessionHelper::updateSession($s);

		return json_encode($result);
	}

	/**
	 *
	 * End the sign on attempt
	 */
	function EndSignOn(){
		// ensure a sign on attempt is in progress
		$session = SessionHelper::getCurrentSession();
		if(empty($session->SignOnAttemptId)){
			return $this->GetJsonException(new Exception('SignOn not in-progress'));
		}

		$id = $_REQUEST["id"];

		$requestObj = new EndSignOnAttemptRequest();
		$requestObj->ResultId = $id;

		try {
			// make the call to obsidion
			$result = $this->soapClient->EndSignOnAttempt(array('request' => $requestObj));
		} catch (Exception $e) {
			// if the obsidion call fails, wrap up the error as a JSON object and return
			return $this->GetJsonException($e);
		}

		if(!empty($result->EndSignOnAttemptResult->SuccessResult)){
			$this->HandleSuccessfullSignOn($result);
			return;
		}
		else{
			$this->HandleFailedSignOn($result);
		}
	}

	/**
	 *
	 * Handle a failed sign on attempt
	 * @param $result
	 */
	private function HandleFailedSignOn($result){
		// get the failure reason from the result
		$failureReason = $result->EndSignOnAttemptResult->FailedResult->Reason;
		$errorMessage = sprintf(ErrorMessages::GeneralErrorMessageFormat, $failureReason);

		// craft an error message based on the reason
		// see /services/FaulureReason.php for the constants used below 
		switch($failureReason){
			case FailureReason::SignOnCancelled:
				// no error required for user cancelled
				$errorMessage = null;
				break;
			case FailureReason::AccountPasswordReset:
				// Note that we DON'T recommend that the activation link is
				// displayed to the user. It is displayed here to facilitate testing.
				$errorMessage = sprintf(ErrorMessages::AccountPasswordResetMessageFormat, $result->EndSignOnAttemptResult->FailedResult->ActivationLinkUri, $failureReason);
				break;
			case FailureReason::ValidationFailed:
				$errorMessage = sprintf(ErrorMessages::ValidationFailedMessageFormat, $failureReason);
				break;
			case FailureReason::AccountBlocked:
				$errorMessage = sprintf(ErrorMessages::AccountBlockedMessageFormat, $failureReason);
				break;
			case FailureReason::AccountExpired:
				$errorMessage = sprintf(ErrorMessages::AccountExpiredMessageFormat, $failureReason);
				break;
			case FailureReason::AccountHardLocked:
				$errorMessage = sprintf(ErrorMessages::AccountHardLockedMessageFormat, $failureReason);
				break;
			case FailureReason::AccountSoftLocked:
				$errorMessage = sprintf(ErrorMessages::AccountSoftLockedMessageFormat, $failureReason);
				break;
			case FailureReason::AccountNotActivated:
				$errorMessage = sprintf(ErrorMessages::AccountNotActivatedMessageFormat, $failureReason);
				break;
			case FailureReason::AccountNotRegistered:
				$errorMessage = sprintf(ErrorMessages::AccountNotRegisteredMessageFormat, $failureReason);
				break;
			case FailureReason::SessionExpired:
				$errorMessage = sprintf(ErrorMessages::SessionExpiredMessageFormat, $failureReason);
				break;
			case FailureReason::UnknownAccount:
			case FailureReason::ProtocolViolation:
			case FailureReason::SignOnAttemptNotFound:
				// general error message already set above is fine
				break;
		}

		// if signon failed, it's unlikely there will be a valid session
		if(SessionHelper::isCurrentSessionValid()){
			$session = SessionHelper::getCurrentSession();
			$session->ErrorMessage = $errorMessage;
			SessionHelper::updateSession($session);
			header("Location: /");
		}
		else{
			// set error message in session...
			$session = WebAppSession::CreateErrorSession($errorMessage);
			SessionHelper::updateSession($session);
			// ...and redirect to sign on.
			header("Location: /SignIn.php");
		}
	}

	/**
	 * 
	 * Handle a successfull sign on
	 * @param unknown_type $result
	 */
	private function HandleSuccessfullSignOn($result){
		// create an "authenticated" session object
		$culture = $result->EndSignOnAttemptResult->Culture;
		$successResult = $result->EndSignOnAttemptResult->SuccessResult;
		$ssoSessionId = $successResult->SessionId;
		$sessionExpiry = $successResult->SessionExpiry;
		$authToken = $successResult->UserAuthenticationToken;
		$email = $successResult->EmailAddress;

		$s = WebAppSession::CreateWebAppSession($email, $ssoSessionId, $sessionExpiry, $culture, $authToken);
		SessionHelper::updateSession($s);

		// Redirect to Home.php
		header("Location: /Default.php");
	}

	/**
	 * 
	 * Sign out of the current "application"
	 */
	function SignOut(){
		// ensure you're actually signed into the current application
		$session = SessionHelper::getCurrentSession();
		if(empty($session->SSOSessionId)){
			return $this->GetJsonException(new Exception('SSO Session not in-progress'));
		}

		$requestObj = new SessionRemoveParticipantRequest();
		$requestObj->SessionId = $session->SSOSessionId;
		try {
			// make the call to obsidion
			$result = $this->soapClient->SessionRemoveParticipant(array('request' => $requestObj));
		} catch (Exception $e) {
			// if the obsidion call fails, wrap up the error as a JSON object and return
			return $this->GetJsonException($e);
		}

		// check sign out succeded
		if($result->SessionRemoveParticipantResult->Success == false)
		{
			return $this->GetJsonException(new Exception("Sign out failed"));
		}

		// distroy the current session
		SessionHelper::clearCurrentSession();
	}

	/**
	 * 
	 * Sign out of all SSO applications
	 */
	function SignOutAll(){
		// ensure you're actually signed into the current application
		$session = SessionHelper::getCurrentSession();
		if(empty($session->SSOSessionId)){
			return $this->GetJsonException(new Exception('SSO Session not in-progress'));
		}

		$requestObj = new SessionSignOffRequest();
		$requestObj->SessionId = $session->SSOSessionId;

		try {
			// make the call to obsidion
			$result = $this->soapClient->SessionSignOff(array('request' => $requestObj));
		} catch (Exception $e) {
			// if the obsidion call fails, wrap up the error as a JSON object and return
			return $this->GetJsonException($e);
		}

		// check sign off succeded
		if($result->SessionSignOffResult->Success == false)
		{
			return $this->GetJsonException(new Exception("Sign out failed"));
		}

		// distroy the current session
		SessionHelper::clearCurrentSession();
	}

	/**
	 * 
	 * Check the current session is still valid and call SSO to extend it if necessary.
	 * Called every time a "protected" page is loaded
	 */
	function CheckCurrentSessionIsValidAndExtendIfRequired(){
		// if the current session is nolonger valid...
		if(!SessionHelper::isCurrentSessionValid()){
			// redirect to sign in. don't need to destroy the current session as something already did
			header("Location: /SignIn.php");
			exit();
		}

		// if the session hasn't been flagged by a notification...
		if(!SessionHelper::shouldExtendSSOSessionOnUserActivity()){
			// ...don't go to the SSO service to extend the SSO session
			// just update the latest user activity on the application session
			SessionHelper::refreshSession();
			return;
		}

		// the session has been flagged so goto SSO service and extend it
		$session = SessionHelper::getCurrentSession();
		$ssoSessionId = $session->SSOSessionId;

		// calculate the requtested date
		$sessionExpiry = new DateTime('now', new DateTimeZone('UTC'));
		$sessionExpiry->add(new DateInterval('PT' . DEFAULT_SESSION_TIMEOUT . 'M'));

		// call the SSI Service
		$result = $this->ExtendSSOSession($ssoSessionId, $sessionExpiry);

		// extend the local session according to the returned SSO response
		$ssoSessionExpiry = $result->SessionExtendResult->SessionExpiry;
		SessionHelper::extendSession($ssoSessionId, $ssoSessionExpiry);

		// set the local session to not flaged for next load
		SessionHelper::setShouldExtendSSOSessionOnUserActivity($ssoSessionId, false);
	}

	/**
	 *
	 * A reusable method that will try to extend the SSO session 
	 * @param GUID $sessionId
	 * @param DateTime $newSSOSessionExpiry
	 */
	private function ExtendSSOSession($sessionId, $newSSOSessionExpiry){
		$requestObj = new SessionExtendRequest();
		$requestObj->SessionId = $sessionId;
		$requestObj->SessionExpiry = $newSSOSessionExpiry->format('Y-m-d\TH:i:s\Z');

		// Call Sage SSO to extend the SSO session to match our application session
		$result = $this->soapClient->SessionExtend(array('request' => $requestObj));

		return $result;
	}

	/**
	 * 
	 * Called by the notification service to extend the local session associated with the $ssoSessionId
	 * @param $ssoSessionId
	 * @param $ssoSessionExpiryDue
	 */
	function ExtendOtherSession($ssoSessionId, $ssoSessionExpiryDue){
		// ensure the "other" session exists
		$session = SessionHelper::getSession($ssoSessionId);
		if(empty($session->SSOSessionId)){
			return $this->GetJsonException(new Exception('SSO Session not in-progress'));
		}

		// check to see if it needs extending or just flagging
		if(!SessionHelper::shouldExtendSSOSession($ssoSessionId, $ssoSessionExpiryDue, $newSessionExpiry)){
			// it's okay to simply flag it as there wasn't any recent user activity
			SessionHelper::setShouldExtendSSOSessionOnUserActivity($sessionId, true);

			exit();
		}

		// there was recent user activity so we can automatically extend the local session
		try {
			// user the local helper which calls SSO service to extend the remote session
			$result = $this->ExtendSSOSession($ssoSessionId, $newSessionExpiry);
		}
		catch (Exception $e){
			// There was a problem extending the session on Sage SSO, or some other local problem
			// with the SSO session map or application session state. The safest thing to do
			// at this point is to end the application session.
			SessionHelper::deleteSession($ssoSessionId);
		}

		$sessionExpiry = $result->SessionExtendResult->SessionExpiry;
		$expiryDue = $result->SessionExtendResult->ExpiryDue;

		// Extend the SSO session in the local session to synchronise with what Sage SSO
		// tells us. It's important to observe the session expiry that's returned
		// because the Sage SSO session may be nearing its hard timeout and we
		// don't want to extend the application session beyond that.
		SessionHelper::extendSession($ssoSessionId, $sessionExpiry);

		// The ExpiryDue flag tells us whether this session is in its expiry period. If
		// ExpiryDue returns true at this point either we haven't extended the session far enough
		// past the expiry due threshold to cause Sage SSO to unmark the session (perhaps
		// because there was some activity right at the start of the application session but
		// nothing since) or the SSO session is approaching its hard timeout.
		//
		// If the flag is false then we'll receive another notification before the session expires,
		// if it's true then we WON'T receive another notification before the session expires and
		// we must call Sage SSO to extend the session if there's any activity.
		SessionHelper::setShouldExtendSSOSessionOnUserActivity($ssoSessionId, $expiryDue);
	}
}