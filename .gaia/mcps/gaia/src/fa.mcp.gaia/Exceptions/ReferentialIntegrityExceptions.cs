namespace FrostAura.MCP.Gaia.Exceptions;

/// <summary>
/// Exception thrown when a referenced entity is not found
/// </summary>
public class ReferentialIntegrityException : Exception
{
    public ReferentialIntegrityException() : base()
    {
    }

    public ReferentialIntegrityException(string message) : base(message)
    {
    }

    public ReferentialIntegrityException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when a session is not found
/// </summary>
public class SessionNotFoundException : ReferentialIntegrityException
{
    public string SessionId { get; }

    public SessionNotFoundException(string sessionId) : base($"Session with ID '{sessionId}' not found")
    {
        SessionId = sessionId;
    }

    public SessionNotFoundException(string sessionId, string message) : base(message)
    {
        SessionId = sessionId;
    }
}

/// <summary>
/// Exception thrown when a parent TODO is not found
/// </summary>
public class ParentTodoNotFoundException : ReferentialIntegrityException
{
    public string ParentTodoId { get; }
    public string SessionId { get; }

    public ParentTodoNotFoundException(string parentTodoId, string sessionId) 
        : base($"Parent TODO with ID '{parentTodoId}' not found in session '{sessionId}'")
    {
        ParentTodoId = parentTodoId;
        SessionId = sessionId;
    }

    public ParentTodoNotFoundException(string parentTodoId, string sessionId, string message) 
        : base(message)
    {
        ParentTodoId = parentTodoId;
        SessionId = sessionId;
    }
}
