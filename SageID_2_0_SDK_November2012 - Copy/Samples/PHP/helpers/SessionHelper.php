<?php
/*
 * This is a helper class, which is responsible for managing 
 * the web application sessions objects. For the purpose 
 * of this example the session objects together with the 
 * sessions mapping array are stored in the file system under
 * ./sessions. However this solution is not optimal and is 
 * supposed to be used only for the purpose of understanding 
 * how a web application can use Obsidian authentication provider.
 * A much better solution in a load-balanced environment is 
 * to use a database or a dedicated state server as a session store.
 * */

require_once 'WebAppSession.php';

session_start();

class SessionHelper {
	
	/**
	 * 
	 * Function to get local SessionID from SSOSessionID
	 */
	static private function getSSOSessionMapping() {
		$f = 'sessions/SessionMapping.txt';
		
		if (file_exists($f)){
			$data = file_get_contents($f);		
			$result = unserialize($data);
			return $result;
		}
		else
			return array();
	}
	
	/**
	 * 
	 * Function to set mapping between local SessionId and SSOSessionID
	 * @param GUID $ssoSessionId
	 * @param GUID $appSessionId
	 */
	static private function setSSOSessionMapping($ssoSessionId, $appSessionId) {
		$sessionMapping = self::getSSOSessionMapping();
		
		$sessionMapping[$ssoSessionId] = $appSessionId;
		
		$f = file_put_contents('sessions/SessionMapping.txt', serialize($sessionMapping));
	}
	
	/**
	 * 
	 * Function to remove a SSOSessionID mapping.
	 * Note: This doesn't end the local session 
	 * @param unknown_type $ssoSessionId
	 */
	static private function removeSSOSessionMapping($ssoSessionId) {
		$sessionMapping = self::getSSOSessionMapping();
		
		unset($sessionMapping[$ssoSessionId]);
		
		$f = file_put_contents('sessions/SessionMapping.txt', serialize($sessionMapping));
	}
	
	/**
	 * 
	 * Gets the local session
	 * Returns a WebAppSession 
	 */
	static public function getCurrentSession() {
		$result = '';
		
		$f = 'sessions/' . session_id() . '.txt';	
	
		if (file_exists($f)) {
			$data = file_get_contents($f);		
			$result = unserialize($data);
		}
		
		if ($result instanceof WebAppSession){
			return $result;
		}
		else{
			return null;
		}
	}
	
	/**
	 * 
	 * Deletes the current session, effectively ending the session as protected pages check this exists
	 */
	static public function clearCurrentSession() {
		
		$session = self::getCurrentSession();
		
		if (!empty($session)) {
			
			self::deleteSession($session->SSOSessionId);
		}
	}
	
	/**
	 * 
	 * Returns true if a session exists and is still valid (i.e. not expired)
	 */
	static public function isCurrentSessionValid() {
		$session = self::getCurrentSession();
		
		if (!empty($session)) {
			
			$now = new DateTime('now', new DateTimeZone('UTC'));
			$ssoSessionExpires = new DateTime($session->SSOSessionExpires);
			
			if ($ssoSessionExpires > $now) 
				return true;
			else {
				return false;
				self::clearCurrentSession();
			}
		} 
		
		return false;
	}
	
	/**
	 * 
	 * Creates a local session and adds a session mapping
	 * @param WebAppSession $webAppSession
	 */
	static public function addSession($webAppSession) {
		
		$f = file_put_contents('sessions/' . $webAppSession->SessionId . '.txt', serialize($webAppSession));
		
		self::setSSOSessionMapping($webAppSession->SSOSessionId, $webAppSession->SessionId);
	}
	
	/**
	 * 
	 * Updates the session
	 * @param WebAppSession $webAppSession
	 */
	static public function updateSession($webAppSession) {
		//try delete, it's okay if it fails cos session might not exist yet.
		try{
			self::deleteSession($webAppSession->SSOSessionId);
		}
		catch (Exception $e){
		}
		self::addSession($webAppSession);
	}
	
	/**
	 * 
	 * Gets a local session based on an SSO Session Id
	 * @param GUID $ssoSessionId
	 */
	static public function getSession($ssoSessionId) {
		
		$result = '';
		$sessionMapping = self::getSSOSessionMapping();
		$appSessionId = $sessionMapping[$ssoSessionId];
		
		if (!empty($appSessionId)){

			$f = 'sessions/' . $appSessionId . '.txt';	
		
			if (file_exists($f)) {
				$data = file_get_contents($f);		
				$result = unserialize($data);
			}
			
			if ($result instanceof WebAppSession)
				return $result;
			else
				return null;
		}
		else
			return null;
	}
	
	/**
	 * 
	 * Deletes a local session based on an Sso SessionId and deletese the mapping
	 * @param GUID $ssoSessionId
	 */
	static public function deleteSession($ssoSessionId) {
		
		$sessionMapping = self::getSSOSessionMapping();
		
		if(empty($sessionMapping[$ssoSessionId])){
			return;
		}
		
		$appSessionId = $sessionMapping[$ssoSessionId];
		
		self::removeSSOSessionMapping($ssoSessionId);
		
		if (!empty($appSessionId)){
		
			$f = 'sessions/' . $appSessionId . '.txt';
		
			if (file_exists($f))
				return unlink($f);	
		}
	}
	
	/**
	 * 
	 * Refreshes a local session (sets the LastActivity property to "now") based on an SSO Session Id
	 * @param GUID $ssoSessionId
	 * @throws Exception
	 */
	static public function refreshSession() {
		
		$session = self::getCurrentSession();
		
		if (!empty($session)) {
			
			$session->LastActivity = new DateTime('now', new DateTimeZone('UTC'));
			
			self::updateSession($session);
			
		} else {
			throw new Exception('No SSO session with specified ID in the cache.');
		}
	}
	
	/**
	 * 
	 * Cheks to see if a local session exists for the SSO Session Id
	 * @param unknown_type $ssoSessionId
	 */
	static public function hasSession($ssoSessionId) {
		
		$session = self::getSession($ssoSessionId);
		
		if (!empty($session))
			return true;
		else
			return false; 
	}
	
	/**
	 * 
	 * Checks to see if the application should call SSO to extend the session.
	 * @param GUID $ssoSessionId
	 * @param DateTime $sessionExpiryDue
	 * @param DateTime $newSessionExpiry This is an output parameter which tells us what to extend the session expiry to.
	 * @return true if SSO should be extended
	 */
	static public function shouldExtendSSOSession($ssoSessionId, $sessionExpiryDue, &$newSessionExpiry) {
		
		$session = self::getSession($ssoSessionId);
		
		if (!empty($session)) {
			
			// Calculate the new SSO session expiry time to request by adding our default application
            // session timeout to the last time we had any activity
			$newSessionExpiry = $session->LastActivity; 			
			$newSessionExpiry->add(new DateInterval('PT' . DEFAULT_SESSION_TIMEOUT . 'M'));						
			
			return $newSessionExpiry > $sessionExpiryDue;
		}
		else
			return false;
	}
	
	/**
	 * 
	 * Return true if the current session has been flaged as expring soon and requires extending 
	 */
	static public function shouldExtendSSOSessionOnUserActivity() {
		
		$session = self::getCurrentSession();
				
		if (!empty($session)) {
			return $session->ExtendOnUserActivity; 
		} else {
			return false;
		}
	}

	/**
	 * 
	 * Set the flag so the the session gets extended upon the next user activity.
	 * @param guid $ssoSessionId
	 * @param boolean $expiryDue
	 */
	static public function setShouldExtendSSOSessionOnUserActivity($ssoSessionId, $expiryDue) {
		
		$session = self::getSession($ssoSessionId);
		
		if (!empty($session)) {
			
			$session->ExtendOnUserActivity = $expiryDue;
			
			self::updateSession($session);
			
		} else {
			throw new Exception('No SSO session with specified ID in the cache.');
		}
	}
	
	/**
	 * 
	 * Extends the local session associated with the SSO Session id $ssoSessionId.
	 * @param GUID $ssoSessionId
	 * @param DateTime $ssoSessionExpiry
	 * @return boolean
	 */
	static public function extendSession($ssoSessionId, $ssoSessionExpiry) {
		
		$session = self::getSession($ssoSessionId);
		
		if (!empty($session)) {
			$session->SSOSessionExpires = $ssoSessionExpiry;
		
			self::updateSession($session);
			
			return true;
		} else {
			return false;
		}
	}
}
?>