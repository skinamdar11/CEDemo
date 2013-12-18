<?php
require_once("/services/Definitions.php");
require_once('/services/SessionService.php');
require_once('/libs/http_request.php');
require_once('/libs/SimpleSAML_XML_Validator.php');
require_once('/helpers/CacheHelper.php');

/**
 *
 * A service for handling notifications from the SSO service
 */
class NotificationService extends SessionService{

	function __construct(){
		parent::__construct();
	}

	/**
	 *
	 * Handles the notification request
	 */
	function HandleNotification(){
		$http_request = new http_request();

		// Notification data is supplied in Base64 format, to avoid digital signature encoding issues
		$data = base64_decode($http_request->body);
			
		// load the notification body into an xml document which can be traversed
		// with xpath
		$notificationDOM = new DOMDocument();
		$notificationDOM->loadXML($data);
		$xPath = new DomXPath($notificationDOM);
		$xPath->registerNamespace('sso', 'http://sso.sage.com');

		try{
			// ensure the notification is signed, not expired and not replayed
			$this->CheckNotificationValidSignature($data);

			$this->CheckNotificationNotExpired($xPath);

			$this->CheckNotificationNotReplayed($xPath);
		}
		catch(Exception $e){
			return $this->GetJsonException($e);
		}

		// get the notification type from the xml document
		$notificationType = $xPath->query('/sso:Notification/sso:Type')->item(0)->nodeValue;

		switch($notificationType){
			case "Session.Ended":
				// if this is a Session.Ended notification...
				$sessionId = $xPath->query('/sso:Notification[sso:Type=\'Session.Ended\']/sso:Parameters/sso:Parameter[sso:Name=\'SessionId\']/sso:Value')->item(0)->nodeValue;
				// ... kill the session associated with the SSO Session Id $sessionId
				SessionHelper::deleteSession($sessionId);
				break;
			case "Session.ExpiryDue":
				// if this is an Session.ExpiryDue notification, extract the SSO Session ID and expiry due date...
				$sessionId = $xPath->query('/sso:Notification[sso:Type=\'Session.ExpiryDue\']/sso:Parameters/sso:Parameter[sso:Name=\'SessionId\']/sso:Value')->item(0)->nodeValue;
				$ssoSessionExpiryDueNodeDom = $xPath->query('/sso:Notification/sso:Parameters/sso:Parameter[sso:Name=\'Timestamp\']/sso:Value');
				// The timestamp, like all Sage SSO timestamps, is in XSD schema format and is UTC.
				$ssoSessionExpiryDue = new DateTime($ssoSessionExpiryDueNodeDom->item(0)->nodeValue);

				// ... and extend the session if required
				$this->ExtendOtherSession($sessionId, $ssoSessionExpiryDue);
				break;
			default:
				throw new Exception("Unexpected notification type.");
				break;
		}
			
	}

	/**
	 *
	 * Check the signature of the notification using the SSO Root certificate (which should be installed
	 * into the LocalMachine / Root certificate store). If the notification wasn't signed by the SSO Root
	 * certificate, it didn't come from Sage SSO so in that case we'll ignore it.
	 * @param unknown_type $strXmlSig
	 */
	private function CheckNotificationValidSignature($strXmlSig) {
		$fh = null;

		$fh = fopen(ROOT_CERT_PUB_KEY, 'r');
		$strPublicKey = fread($fh, filesize(ROOT_CERT_PUB_KEY));
		fclose($fh);

		$xmlToBeValidatedDOM = new DOMDocument();
		$xmlToBeValidatedDOM->loadXML($strXmlSig);
			
		// Validate XML Signature
		$xmlSigValidator = new SimpleSAML_XML_Validator($xmlToBeValidatedDOM, null, $strPublicKey);
	}

	/**
	 *
	 * We can now check that the Notification has an Issued timestamp, that it is in a valid
	 * format and that it is recent.
	 * @param unknown_type $xPath
	 * @throws Exception
	 */
	private function CheckNotificationNotExpired($xPath) {
		$issuedNodeDom = $xPath->query('/sso:Notification/sso:Issued');
		$issuedNode = $issuedNodeDom->item(0)->nodeValue;

		if (empty($issuedNode)) throw new Exception('Notification contains no Issued element.');

		try {
			$issuedDT = new DateTime($issuedNode);
		}
		catch (Exception $e) {
			throw new Exception('Notification issue time is invalid.');
		}

		// Notifications should be delivered within a few seconds of issue, depending on the overall load of
		// your application and Sage SSO. If the notification is more than ten seconds old, discard it.
		$gmtNow = new DateTime('now', new DateTimeZone('UTC'));
		$gmtNow->sub(new DateInterval('PT10S'));

		if ($gmtNow > $issuedDT) throw new Exception('Notification has expired.');

		return $issuedNode;
	}

	/**
	 * 
	 * Even given the expiry check, there's a small window of opportunity for a replay attack. It's best to hold a cache of
	 * received notification IDs so, if a notification is replayed before it's considered expired, we can detect
	 * it. On a load-balanced, multi-node application, it's best to use a caching service such as memcached or velocity
	 * to manage this. For the purposes of this application, however, we'll just use a text file.
	 * 
	 * @param unknown_type $xPath
	 * @throws Exception
	 */
	private function CheckNotificationNotReplayed($xPath) { 
		$notificationIdDom = $xPath->query('/sso:Notification/sso:NotificationId');
		$notificationId = $notificationIdDom->item(0)->nodeValue;

		if (empty($notificationId)) throw new Exception('Notification contains no NotificationId element.');

		if ($this->IsReplayed($notificationId)) throw new Exception('Notification has been replayed.');
	}

	/**
	 * 
	 * Check the notification id isn't in the cache
	 * @param unknown_type $id
	 */
	private function IsReplayed($id) {

		$replayed = false;

		$idKey = 'NotificationId_' . $id;

		$cacheNotification = CacheHelper::get($idKey);

		if (empty($cacheNotification)) {
			CacheHelper::add($idKey, '<not null value>');
		}
		else {
			$replayed = true;
		}

		return $replayed;
	}

}