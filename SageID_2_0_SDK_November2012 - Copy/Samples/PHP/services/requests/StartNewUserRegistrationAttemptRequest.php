<?php
include_once '/services/Definitions.php';

class StartNewUserRegistrationAttemptRequest {
		
	public $SuccessUri = 'http://webappp.dev.sage.com/_SignUpNewResult.php?id={0}';
	public $FailureUri = 'http://webappp.dev.sage.com/_SignUpNewResult.php?id={0}';
	public $CancelAllowed = true;
	public $State = 'this is arbitary state';

	public $DisplayName = null;
	public $EmailAddress = '';
	public $SignOnAfterSuccess = true;
	public $ActivateAfterSuccess = true;

	public $Culture = CULTURE;

	public $TemplateName = TEMPLATE_NAME;
	public $CaptchaTheme = CAPTCHA_THEME;
}

?>