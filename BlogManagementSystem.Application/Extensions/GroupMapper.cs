using BlogManagementSystem.Application.DTOs;
using BlogManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BlogManagementSystem.Application.Extensions
{
    public static class GroupMapper
    {
        public static GroupDto ToDto(this LocalGroup group)
        {
            return new GroupDto
            {
                Id = group.Id.ToString(),
                Name = group.Name,
                Path = group.Path,
                ParentGroupId = group.ParentGroupId?.ToString(),
                SubGroups = []
            };
        }
        
        public static IEnumerable<GroupDto> ToHierarchicalDto(this IEnumerable<LocalGroup> groups)
        {
            if (groups == null) return [];
            
            // Convert flat list of groups to a hierarchical structure
            var result = new List<GroupDto>();
            var groupMap = groups.ToDictionary(g => g.Id, g => g.ToDto());
            
            // Set parent-child relationships
            foreach (var group in groups)
            {
                if (group.ParentGroupId.HasValue && groupMap.TryGetValue(group.ParentGroupId.Value, out var parentDto))
                {
                    parentDto.SubGroups ??= [];
                    if (groupMap.TryGetValue(group.Id, out var childDto))
                    {
                        parentDto.SubGroups.Add(childDto);
                    }
                }
                else
                {
                    // Root level group
                    if (groupMap.TryGetValue(group.Id, out var rootGroupDto))
                    {
                        result.Add(rootGroupDto);
                    }
                }
            }
            
            return result;
        }
    }
} 