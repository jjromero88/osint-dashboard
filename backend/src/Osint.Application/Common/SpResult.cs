namespace Osint.Application.Common;

public class SpResult
{
    public bool Error { get; set; }
    public string Msg { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }
}

public class SpResult<T> : SpResult
{
    public T? Data { get; set; }
}
