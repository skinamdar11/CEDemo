<?php
include_once('/services/AllRequestClasses.php');
include_once('/helpers/SessionHelper.php');
include_once('/helpers/WebAppSession.php');
include_once('/helpers/ExceptionHelper.php');
/**
 *
 * Base class for services which will call SSO service
 * @author ben.crinion
 *
 */
class ServiceBase
{
	protected $soapClient = null;

	/**
	 *
	 * Construct the SsoService i.e. initialise the SoapClient
	 */
	function __construct(){
		$opts = array(
	        'ssl' => array(
	            'local_cert' => './resources/SSOTestIdentity_WebAppP.pem',
	            'allow_self_signed' => true
		)
		);

		$ctx = stream_context_create($opts);
		$this->soapClient = @new SoapClient('./resources/wsdl/sso.sage.com.wsdl', array(
						'exceptions'		=> 1,
	        			'stream_context'    => $ctx,
	        			'local_cert'        => './resources/SSOTestIdentity_WebAppP.pem',
						'trace' => false // TRUE only for debug purposes to inspect the soap request $soapClient->__getLastRequest()
		));
	}

	/**
	 *
	 * A method which wraps an exception as JSON so we've got a standard type to handle in the JS
	 * @param Exception $exception
	 */
	protected function GetJsonException($exception){
		if(!($exception instanceof Exception)){
			throw InvalidArgumentException("GetJsonException expects an Exception. Input:".$exception);
		}
		
		return ExceptionHelper::GetJsonException($exception);
	}
}
?>