<?php
class StartPasswordChangeAttemptRequest {
		
	public $SuccessUri = 'http://webappp.dev.sage.com/_ChangePasswordResult.php?id={0}';
	public $FailureUri = 'http://webappp.dev.sage.com/_ChangePasswordResult.php?id={0}';
	public $CancelAllowed = true;
	public $State = 'this is arbitary state';

	public $TemplateName = TEMPLATE_NAME;
	public $CaptchaTheme = CAPTCHA_THEME;
}
?>