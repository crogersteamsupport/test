FIND /C /I "app.tsdev.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1				app.tsdev.com>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "portal.tsdev.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1				portal.tsdev.com>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "beta.tsdev.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1				beta.tsdev.com>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "alpha.tsdev.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1				alpha.tsdev.com>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "updates.tsdev.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1				updates.tsdev.com>>%WINDIR%\system32\drivers\etc\hosts
