using BlogManagementSystem.Application.DTOs;
using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Extensions;

public static class UserMapper
{
	public static UserDto ToDto(this LocalUserIdentity user)
	{
		return new UserDto
		{
			Id = user.UserId,
			Username = user.Username,
			Email = user.Email,
			FirstName = user.FirstName,
			LastName = user.LastName,
			Enabled = user.IsEnabled,
			Attributes = []
		};
	}
}