<?php
class ExceptionHelper {
	public static function GetJsonException($exception){
		if(!($exception instanceof Exception)){
			throw InvalidArgumentException("GetJsonException expects an Exception. Input:".$exception);
		}
		$errorObj = array( 'Error' => array('Message'=>$exception->getMessage(), 'Trace'=>$exception->getTraceAsString()));

		return json_encode($errorObj);
	}
}