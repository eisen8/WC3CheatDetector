#include "CTErrorCodes.h"
#include <windows.h>
#include <map>
#include <string>
#include <codecvt>
#include <locale>
namespace CT
{
	static CTSTATUS LastErrorStatus = 0;
	static std::wstring LastErrorMessage = L"";

	static std::map <CTSTATUS, const std::wstring> ErrorCodeMap = {
		{0,  L""}, // Success
		{1,  L"Generic Error."},
		{2,  L"Invalid Handle."},
		{3,  L"Unable to get Processes."},
		{20, L"Could not load ntdll.dll."},
		{21, L"Could not duplicate process."},
		{22, L"Ran out of memory."},
		{23, L"Error calling NtQuerySystemInformation."}
	};

	void SetLastError(CTSTATUS errorStatusCode)
	{
		LastErrorStatus = errorStatusCode;

		if (ErrorCodeMap.count(errorStatusCode))
		{
			// Found error code
			LastErrorMessage = ErrorCodeMap[errorStatusCode];
		}
		else
		{
			// No matching error message
			LastErrorMessage = L"";
		}
	}

	void SetLastError(CTSTATUS errorStatusCode, const CHAR* errorMessage)
	{

		if (ErrorCodeMap.count(errorStatusCode))
		{
			// Found error code. Combine map value and provided message.
			LastErrorStatus = errorStatusCode;
			std::wstring errorMessageInput = std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>>().from_bytes(errorMessage);
			LastErrorMessage = ErrorCodeMap[errorStatusCode] + L" " + errorMessageInput;
		}
		else
		{
			// No matching error message
			LastErrorStatus = errorStatusCode;
			LastErrorMessage = std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>>().from_bytes(errorMessage);
		}
	}


	CTSTATUS GetLastCTError(WCHAR* messageBuffer, rsize_t bufferLength)
	{
		DWORD lastError = LastErrorStatus;

		PCWSTR message = LastErrorMessage.c_str();
		size_t messageLength = std::char_traits<wchar_t>::length(message);
		wcsncpy_s(messageBuffer, bufferLength, message, messageLength);

		LastErrorStatus = 0; // reset last error
		LastErrorMessage = L"";
		return lastError;
	}
}