namespace TaskWeb.Services;

public class ValidationResult
{
    public List<string> Errors { get; } = new();

    public bool Success => Errors.Count == 0;

    public void AddError(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            Errors.Add(message);
        }
    }
}
