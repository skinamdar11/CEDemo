<?php

/**
 * constant definitions for use throughout the system
 */

// Session TimeOut in minutes
define('DEFAULT_SESSION_TIMEOUT', '10');

// Root certificate's Public Key, which is used to validate the XML Signature
define('ROOT_CERT_PUB_KEY', './resources/SSOIdentityRootPubKey.pem');

// Optionally set culture
define('CULTURE', 'en-GB');

// Optionally set TemplateName and CaptchaTheme. Both must be provided.
define('TEMPLATE_NAME', 'Default');
define('CAPTCHA_THEME', 'Clean');

?>