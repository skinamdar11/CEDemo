<?php
// Even given the expiry check, there's a small window of opportunity for a replay attack. It's best to hold a cache of
// received notification IDs so, if a notification is replayed before it's considered expired, we can detect
// it. On a load-balanced, multi-node application, it's best to use a caching service such as memcached or velocity
// to manage this. For the purposes of this application, however, we'll just use a text file.

class CacheHelper {
	
	static private function getCache() {
		$f = 'sessions/Cache.txt';
		
		if (file_exists($f)){
			$data = file_get_contents($f);		
			$result = unserialize($data);
			return $result;
		}
		else
			return array();
	}
	
	static public function add($key, $value) {
		$cache = self::getCache();
		
		$gmtNow = new DateTime('now', new DateTimeZone('UTC'));
		$cache[$key] = array('value' => $value, 'date' => $gmtNow->format('Y-m-d H:i:s'));
		
		$f = file_put_contents('sessions/Cache.txt', serialize($cache));
	}
	
	static public function get($key) {
		$cache = self::getCache();
		
		return $cache[$key];
	}
	
	static public function delete($key) {
		$cache = self::getCache();
		
		unset($cache[$key]);
		
		$f = file_put_contents('sessions/Cache.txt', serialize($cache));
	}		
	
}

?>