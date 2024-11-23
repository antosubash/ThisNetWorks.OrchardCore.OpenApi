using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ThisNetWorks.OrchardCore.OpenApi.Models;
using ThisNetWorks.OrchardCore.OpenApi.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Settings;
using JsonMergeSettings = System.Text.Json.Settings.JsonMergeSettings;

namespace OrchardCore.ContentManagement
{
    public static class ContentDtoExtensions
    {
        private static JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private static readonly JsonMergeSettings ReplaceJsonMergeSettings = new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Replace,
            MergeNullValueHandling = MergeNullValueHandling.Merge
        };

        public static TDto ToDto<TDto>(this ContentElement content)
            where TDto : ContentElementDto
        {
            var serialized = JsonSerializer.SerializeToNode(content);
            var deserialized = serialized?.Deserialize<TDto>(Options);
            return deserialized;
        }

        public static ContentItem ToContentItem(this ContentItemDto content)
        {
            var serialized = JsonSerializer.SerializeToNode(content);
            var deserialized = serialized?.Deserialize<ContentItem>(Options);
            return deserialized;
        }

        public static TDto ToDto<TDto>(this ContentElementDto contentDto)
            where TDto : ContentElementDto
        {
            var serialized = JsonSerializer.SerializeToNode(contentDto);
            var deserialized = serialized?.Deserialize<TDto>(Options);
            return deserialized;
        }

        public static IList<TDto> OfDtoType<TDto>(this IList<ContentItemDto> contentDtos)
            where TDto : ContentElementDto
        {
            return contentDtos.OfDtoType<TDto>("ItemDto");
        }

        public static IList<TDto> OfDtoType<TDto>(this IList<ContentItemDto> contentDtos, string schemaItemExtension)
            where TDto : ContentElementDto
        {
            var originalDtos = contentDtos.ToArray();
            var results = new List<TDto>();
            int i = 0;
            foreach (var contentDto in originalDtos)
            {
                if (contentDto is TDto)
                {
                    results.Add(contentDto as TDto);
                }
                else
                {
                    if (contentDto.ContentType == typeof(TDto).Name.Replace(schemaItemExtension, ""))
                    {
                        var typedContentDto = contentDto.ToDto<TDto>();
                        contentDtos[i] = typedContentDto as ContentItemDto;
                        results.Add(typedContentDto);
                    }
                }
                i++;
            }

            return results;
        }

        public static ContentItem FromDto<TDto>(this ContentItem contentItem, TDto dto, JsonMergeSettings jsonMergeSettings = null)
            where TDto : ContentItemDto
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var jsonNode = JsonSerializer.SerializeToNode(dto);
            return contentItem.Merge(jsonNode, jsonMergeSettings ?? ReplaceJsonMergeSettings);
        }

        internal static void AdditionalPropertiesToCamelCase(this ContentElementDto contentElementDto)
        {
            if (contentElementDto.AdditionalProperties != null && contentElementDto.AdditionalProperties.Any())
            {
                var additionalProperties = new Dictionary<string, object>();
                foreach (var key in contentElementDto.AdditionalProperties.Keys)
                {
                    var value = contentElementDto.AdditionalProperties[key];
                    contentElementDto.AdditionalProperties.Remove(key);
                    var camelCaseKey = char.ToLowerInvariant(key[0]) + key.Substring(1);
                    additionalProperties.Add(camelCaseKey, value);
                }
                contentElementDto.AdditionalProperties = additionalProperties;
            }
        }
    }
}
