FIND /C /I "app.tsdev.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1				app.tsdev.com>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "portal.tsdev.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1				portal.tsdev.com>>%WINDIR%\system32\drivers\etc\hosts
