using System.Collections.Concurrent;
using Scriban;

namespace Shared.Mailing;

/// <summary>
/// Renders Scriban templates and caches them for speed.
/// Usage:
///   var html = TemplateRenderer.Render("Hello {{ name }}", new { name = "Dr. Siti" });
/// </summary>
public static class TemplateRenderer
{
    private static readonly ConcurrentDictionary<string, Template> _cache = new();

    public static string Render(string templateText, object model)
    {
        var tpl = _cache.GetOrAdd(templateText, t => Template.Parse(t));
        return tpl.Render(model, member => member.Name);
    }
}
