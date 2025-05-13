using System.Text.RegularExpressions;
using BlogManagementSystem.Application.DTOs;

namespace BlogManagementSystem.Application.Extensions;

public static partial class RoleDtoExtensions
{
	public static string GetFormattedDescription(this RoleDto role)
	{
		if (string.IsNullOrEmpty(role.Description))
			return string.Empty;
                
		var match = RegexPattern().Match(role.Description);
		return match.Success ? match.Groups[1].Value.Trim() : role.Description.Trim();
	}

    [GeneratedRegex(@"\$\{(.*?)\}")]
    private static partial Regex RegexPattern();
}