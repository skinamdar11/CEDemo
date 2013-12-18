<?php
/**
 *  This is a structure for storing the local session
 */
class WebAppSession {
	/**
	 * The web server session ID - GUID
	 */
	public $SessionId;
	
	/**
	 * The sign on attempt id - GUID
	 */
	public $SignOnAttemptId;
	public $NewUserRegistrationAttemptID;
	public $ExistingUserRegistrationAttemptID;
	public $PasswordChangeAttemptId;
	
	public $Username;
	public $SSOSessionId;
	public $SSOSessionExpires;
	public $Culture;
	public $LastActivity;
	public $ExtendOnUserActivity;
	public $AuthToken;
	public $ErrorMessage;
	
	
	function __construct(){
		$this->SessionId = session_id();
	}
	
	/**
	 * 
	 * Create a WebAppSession with only the error message set.
	 * @param unknown_type $message
	 */
	static function CreateErrorSession($message){
		$sess = new WebAppSession();
		$sess->ErrorMessage = $message;
		return $sess;
	}
	
	/**
	 * 
	 * Create a WebAppSession for an authenticated session
	 * @param unknown_type $username
	 * @param unknown_type $ssoSessionId
	 * @param unknown_type $ssoSessionExpires
	 * @param unknown_type $culture
	 * @param unknown_type $authToken
	 */
	static function CreateWebAppSession($username, $ssoSessionId, $ssoSessionExpires, $culture, $authToken) {
		$sess = new WebAppSession();
		$sess->Username = $username;
		$sess->SSOSessionId = $ssoSessionId;
		$sess->SSOSessionExpires = $ssoSessionExpires;
		$sess->Culture = $culture;
		
		$sess->LastActivity = new DateTime('now', new DateTimeZone('UTC'));
		$sess->ExtendOnUserActivity = false;
		$sess->AuthToken = $authToken;
		return $sess;
	}
}

?>