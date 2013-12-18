<?php
require_once('/services/SessionService.php');

class ActivationService extends SessionService{

	function __construct(){
		parent::__construct();
	}

	/**
	 * 
	 * Handle activation callbacks from SSO service
	 */
	function HandleActivation(){
		$id = $_REQUEST["id"];

		$requestObj = new ActivationLinkContextRequest();
		$requestObj->ActivationId = $id;

		try {
			// make the call to obsidion
			$result = $this->soapClient->ActivationLinkContext(array('request' => $requestObj));
		} catch (Exception $e) {
			// if the obsidion call fails, wrap up the error as a JSON object and return
			return $this->GetJsonException($e);
		}
		
		$resultDictionary = $this->GetResultDictionary($result);
		
		$handler = $resultDictionary["Handler"];
		$activationResult = $resultDictionary["Result"];
		$identityId = $resultDictionary["IdentityId"];

		$errorMessage = "The activation link was expired.";

		if($handler == "WebSSO/Registration" && $activationResult == "Activated"){
			$errorMessage = sprintf("Account activated for user identity {%s}", $identityId);
		}
		else if ($handler == "WebSSO/PasswordActivation" && $activationResult == "PasswordChanged"){
			$errorMessage = sprintf("Password changed for user identity {%s}", $identityId);
		}
		else{
			// do nothing the default error message is fine
		}
		
		// set the error message i the session and redirect
		if(SessionHelper::isCurrentSessionValid()){
			// redirect to the default page if an autheticated session exists
			$session = SessionHelper::getCurrentSession();
			$session->ErrorMessage = $errorMessage;
			SessionHelper::updateSession($session);
			header("Location: /");
		}
		else{
			// else redirect to sign in
			$session = WebAppSession::CreateErrorSession($errorMessage);
			SessionHelper::updateSession($session);

			header("Location: /SignIn.php");
		}
	}
	
	/**
	 * 
	 * Take the ActivationLinkContext result and return an associative array of the names->values
	 * @param unknown_type $result
	 */
	private function GetResultDictionary($result){
		$itemArray = $result->ActivationLinkContextResult->Items->Item;
		
		foreach($itemArray as $i => $value){
			$resultArray[$value->Name] = $value->Value;
		}
		
		return $resultArray;
	}
}