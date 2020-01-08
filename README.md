# Extension and Helpers
This library provides various extension functions and helpers for day to day development activities
  
## String - Extensions
#
#### (string).IsNullOrEmpty()
    Check for null or empty
#### (string).IsNotNullAndWhiteSpace(int minLength = 1)
    Check for null or empty
    minLength parameter is used to validate the minimum number of chars in the string.
#### (string).IsNotNull(minLength = 1)
    Check for null or empty
#### (string[]).Join(string separator)
	Joins a string array with a separator
#### (string).GetBytes()
	Get Byte Array of a string
#### (string).GetBytesFromBase64()
	Get Byte Array of Base64 of a string
#### (string).GetBase64()
	Get Base64 of a string
#### (string).ToDateTime(format)
	Convert a string to a datetime with a specified format. Default will be ("MM/dd/yyyy")
#### (string).ToEpoch(format)
	Get Epoch seconds from a string represented date
#### (string).Compress()
	Compress a string
#### (string).Decompress()
	Decompress a string
#### (string).ToObject<T>()
	Deserialize a string to a specified Type by "T"
#### (string).AddVariableValue(variable, value)
	Substitute values of "{  }" with value specified, for e.g {URL} will be replaced to "google.com"
#### (string).ClipSegments(start, end)
	Remove the part of string which starts with "start" and ends with "end". 
    Remove Script tag from a serialized html string
#### (string).ToStream()
	Convert a string to a memory stream
#### (string).Deserialize<T>()
    Deserialize a string to a specified Type by "T"
	same as that of ToObject<T>
#### (string).DeserializeXml<T>()
    Deserialze a XML string to a Typed object
#### (string).DeserializeUrlEncoded<T>()
    Convert a encoded Url with query strings value to a Typed Object
    ?a=1&b=3&c=4  => class obj { int a = 1, int b = 3, int c = 4 }
#### (string).ToInt()
    Convert string to Int
#### (string).ToDouble()
    Convert string to Double
#### (string).Encrypt(passphrase)
    Encrypt a string with given passphrase
#### (string).Decrypt(passphrase)
    Decrypt a string with given passphrase

