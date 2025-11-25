namespace TaskWeb.Services;

using TaskWeb.Models;
using System.Collections.Generic;

public class GradeGenerationResult
{
    public List<string> Errors { get; } = new();
    public List<GradeHorario> HorariosGerados { get; } = new();

    public bool Success => Errors.Count == 0 && HorariosGerados.Count > 0;

    public void AddError(string message)
    {
        if (!string.IsNullOrWhiteSpace(message) && !Errors.Contains(message))
        {
            Errors.Add(message);
        }
    }
}