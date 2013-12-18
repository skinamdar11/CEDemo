<?php

include_once('/services/ServiceBase.php');

class RegistrationService extends ServiceBase{

	function __construct(){
		parent::__construct();
	}

	/**
	 *
	 * Starts a register new user attempt with the SSO service
	 */
	function StartRegisterNewUser(){
		// get the parameter object to send to StartNewUserRegistrationAttempt
		$requestObj = new StartNewUserRegistrationAttemptRequest();

		try {
			// make the call to obsidion
			$result = $this->soapClient->StartNewUserRegistrationAttempt(array('request' => $requestObj));
		} catch (Exception $e) {
			// if the obsidion call fails, wrap up the error as a JSON object and return
			return $this->GetJsonException($e);
		}

		// if for some reason the service call doesn't return a StartSignOnAttemptResult...
		if(empty($result->StartNewUserRegistrationAttemptResult)){
			// ...return a JSON error
			return $this->GetJsonException(new Exception('Empty result returned from StartNewUserRegistrationAttempt'));
		}

		// cache the sign on attempt id in session so we can check it later
		$attemptId = $result->StartNewUserRegistrationAttemptResult->RegistrationAttemptId;
		$s = new WebAppSession();
		$s->NewUserRegistrationAttemptID = $attemptId;
		SessionHelper::updateSession($s);

		return json_encode($result);
	}

	/**
	 *
	 * End the register new user attempt and handle the result
	 */
	function EndRegisterNewUser(){
		// ensure a register new user attempt was actually in progress
		$session = SessionHelper::getCurrentSession();
		if(empty($session->NewUserRegistrationAttemptID)){
			return $this->GetJsonException(new Exception('Registration not in-progress'));
		}

		// get the attempt ID from the query string
		$id = $_REQUEST["id"];
		$requestObj = new EndRegistrationAttemptRequest();
		$requestObj->ResultId = $id;

		try {
			// make the call to obsidion
			$result = $this->soapClient->EndNewUserRegistrationAttempt(array('request' => $requestObj));
		} catch (Exception $e) {
			$output = array("id"=> $id, "error"=> $e);

			// if the obsidion call fails, wrap up the error as a JSON object and return
			return json_encode($output);

			return $this->GetJsonException($e);
		}

		// handle failed registration
		if(!empty($result->EndNewUserRegistrationAttemptResult->RegistrationFailedResult)){
			var_dump($result);
			$this->HandleFailedRegistrationResult($result->EndNewUserRegistrationAttemptResult->RegistrationFailedResult);
			exit();
		}

		// handle successfull registration
		return $this->HandleSuccessfullRegistration($result->EndNewUserRegistrationAttemptResult);
	}

	/**
	 *
	 * Start a register existing user attempt with the SSO service
	 */
	function StartRegisterExistingUser(){
		// get the parameter object to send to StartExistingUserRegistrationAttempt
		$requestObj = new StartExistingUserRegistrationAttemptRequest();

		try {
			// make the call to obsidion
			$result = $this->soapClient->StartExistingUserRegistrationAttempt(array('request' => $requestObj));
		} catch (Exception $e) {
			// if the obsidion call fails, wrap up the error as a JSON object and return
			return $this->GetJsonException($e);
		}

		// if for some reason the service call doesn't return a StartSignOnAttemptResult...
		if(empty($result->StartExistingUserRegistrationAttemptResult)){
			// ...return a JSON error
			return $this->GetJsonException(new Exception('Empty result returned from StartExistingUserRegistrationAttempt'));
		}

		// cache the sign on attempt id in session so we can check it later
		$attemptId = $result->StartExistingUserRegistrationAttemptResult->RegistrationAttemptId;
		$s = new WebAppSession();
		$s->ExistingUserRegistrationAttemptID = $attemptId;
		SessionHelper::updateSession($s);

		return json_encode($result);
	}

	/**
	 *
	 * End the register existing user attempt and handle the result
	 */
	function EndRegisterExistingUser(){
		// ensure a register user attempt was in progress
		$session = SessionHelper::getCurrentSession();
		if(empty($session->ExistingUserRegistrationAttemptID)){
			return $this->GetJsonException(new Exception('Registration not in-progress'));
		}

		$id = $_REQUEST["id"];
		$requestObj = new EndRegistrationAttemptRequest();
		$requestObj->ResultId = $id;

		try {
			// make the call to obsidion
			$result = $this->soapClient->EndExistingUserRegistrationAttempt(array('request' => $requestObj));
		} catch (Exception $e) {
			// if the obsidion call fails, wrap up the error as a JSON object and return
			return $this->GetJsonException($e);
		}

		// handle failed registration
		if(!empty($result->EndExistingUserRegistrationAttemptResult->RegistrationFailedResult)){
			var_dump($result);
			$this->HandleFailedRegistrationResult($result->EndExistingUserRegistrationAttemptResult->RegistrationFailedResult);
			exit();
		}

		// handle successfull registration attempt
		return $this->HandleSuccessfullRegistration($result->EndExistingUserRegistrationAttemptResult);
	}

	function StartResetPassword(){
		// get the parameter object to send to StartNewUserRegistrationAttempt
		$requestObj = new StartPasswordChangeAttemptRequest();

		try {
			// make the call to obsidion
			$result = $this->soapClient->StartPasswordChangeAttempt(array('request' => $requestObj));
		}
		catch (Exception $e) {
			// if the obsidion call fails, wrap up the error as a JSON object and return
			return $this->GetJsonException($e);
		}

		if(empty($result->StartPasswordChangeAttemptResult)){
			return $this->GetJsonException(new Exception('Empty result returned from StartPasswordChangeAttempt'));
		}

		// cache the sign on attempt id in session so we can check it later
		$attemptId = $result->StartPasswordChangeAttemptResult->PasswordChangeAttemptId;
		
		$s = SessionHelper::getCurrentSession();
		$s->PasswordChangeAttemptId = $attemptId;
		SessionHelper::updateSession($s);

		return json_encode($result);
	}

	function EndResetPassword(){
		// ensure a register user attempt was in progress
		$session = SessionHelper::getCurrentSession();
		if(empty($session->PasswordChangeAttemptId)){
			return $this->GetJsonException(new Exception('Change password not in-progress'));
		}
		unset($session->PasswordChangeAttemptId);
		SessionHelper::updateSession($session);
		
		$id = $_REQUEST["id"];
		$requestObj = new EndPasswordChangeAttemptRequest();
		$requestObj->ResultId = $id;

		try {
			// make the call to obsidion
			$result = $this->soapClient->EndPasswordChangeAttempt(array('request' => $requestObj));
		} catch (Exception $e) {
			// if the obsidion call fails, wrap up the error as a JSON object and return
			return $this->GetJsonException($e);
		}

		// handle failed registration
		if(!empty($result->EndPasswordChangeAttemptResult->PasswordChangeFailedResult)){
			var_dump($result);
			$reason = sprintf("Password change unsuccessful. Reason: %s", $result->EndPasswordChangeAttemptResult->PasswordChangeFailedResult->Reason);
			$this->HandlePasswordChangeResult($reason);
			exit();
		}

		$this->HandlePasswordChangeResult("Password change successful");
	}

	/**
	 *
	 * Handle successfull registration
	 * Process is the same for new and existing users
	 * @param unknown_type $result
	 */
	private function HandleSuccessfullRegistration($result){
		// ensure this is really a successfull result that we're processing
		if(empty($result->RegistrationSuccessResult) || empty($result->RegistrationSuccessResult->SuccessResult)){
			// ...return a JSON error
			return $this->GetJsonException(new Exception('Unexpected result from EndSignOnAttempt. Result: '.$result));
		}

		// create an authenticated session
		$successResult = $result->RegistrationSuccessResult->SuccessResult;
		$culture = $result->Culture;
		$ssoSessionId = $successResult->SessionId;
		$sessionExpiry = $successResult->SessionExpiry;
		$email = $successResult->EmailAddress;
		$authToken = $successResult->UserAuthenticationToken;

		$s = WebAppSession::CreateWebAppSession( $email, $ssoSessionId, $sessionExpiry, $culture, $authToken);
		SessionHelper::updateSession($s);

		// Redirect to the default page
		header("Location: /Default.php");
	}

	/**
	 *
	 * Handle an unsuccessfull registration
	 * Process is the same for new and existing users
	 * @param unknown_type $result
	 */
	private function HandleFailedRegistrationResult($result){
		// create an error message session object
		$s = WebAppSession::CreateErrorSession($result->Reason);
		SessionHelper::updateSession($s);
		// redirect back to sign in
		header("Location: /SignIn.php");
	}
	
	private function HandlePasswordChangeResult($result){
		$s = SessionHelper::getCurrentSession();
		$s->ErrorMessage = $result;
		SessionHelper::updateSession($s);

		header("Location: /");
	}
}