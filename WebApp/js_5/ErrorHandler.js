/// <reference name="MicrosoftAjax.js"/>

_ErrorHandler = function _ErrorHandler() {
    _ErrorHandler.initializeBase(this);
};



_ErrorHandler.prototype = {
    initialize: function _ErrorHandler$initialize() {
        _ErrorHandler.callBaseMethod(this, 'initialize');
        window.onerror = Function.createDelegate(this, this._unhandledError);
    },
    dispose: function _ErrorHandler$dispose() {
        window.onerror = null;
        _ErrorHandler.callBaseMethod(this, 'dispose');
    },
    
    _unhandledError: function _ErrorHandler$_unhandledError(msg, url, lineNumber) {
        try
        {
            var stackTrace = 'Error Retrieving StackTrace';
            try
            {
              stackTrace = StackTrace.createStackTrace(arguments.callee);
            }
            catch (e) { }
            TeamSupport.Services.PublicServices.AddExceptionToLog(url, lineNumber, 'JavaScript Exception', msg, 'stackTrace');
            if (msg = 'h is not a constructor') 
            {
              //window.location.reload();
            }
            else
            {
            }
            //window.location.reload();
        }
        catch (e) { }
    },
    publishError: function _ErrorHandler$handledError(error) {
        var x = error.popStackFrame();
        var stackTrace = StackTrace.createStackTrace(error.callee);
        TeamSupport.Services.PublicServices.AddExceptionToLog(url, lineNumber, 'JavaScript Exception', msg, stackTrace);
            
    }
}

_ErrorHandler.registerClass('_ErrorHandler', Sys.Component);

Sys.Application.add_init(
    function() { 
      $create(_ErrorHandler);
    }
);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();