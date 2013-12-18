<?php
include_once '/services/Definitions.php';

class StartSignOnAttemptRequest {
		
	public $SuccessUri = 'http://webappp.dev.sage.com/_SignOnSuccess.php?id={0}';
	//public $SuccessUri = '';
	public $FailureUri = 'http://webappp.dev.sage.com/_SignOnFail.php?id={0}';
	
	public $CancelAllowed = true;
	public $State = 'this is arbitary state';

	public $Culture = CULTURE;

	public $TemplateName = TEMPLATE_NAME;
	public $CaptchaTheme = CAPTCHA_THEME;

	// Set SessionLengthMinutes to the length of your application session.
	public $SessionLengthMinutes = DEFAULT_SESSION_TIMEOUT;
}
?>