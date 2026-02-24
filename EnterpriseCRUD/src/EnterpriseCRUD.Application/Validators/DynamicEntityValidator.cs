using EnterpriseCRUD.Domain.Metadata;
using FluentValidation;

namespace EnterpriseCRUD.Application.Validators;

/// <summary>
/// Dynamic validator that builds validation rules from entity metadata.
/// No need to write per-entity validators.
/// </summary>
public class DynamicEntityValidator : AbstractValidator<Dictionary<string, object?>>
{
    public DynamicEntityValidator(EntityMetadata metadata)
    {
        foreach (var field in metadata.Fields)
        {
            if (!field.ShowInCreate && !field.ShowInEdit) continue;

            var camelKey = char.ToLowerInvariant(field.Name[0]) + field.Name[1..];

            if (field.IsRequired)
            {
                RuleFor(d => d)
                    .Must(d => d.ContainsKey(camelKey) || d.ContainsKey(field.Name))
                    .WithMessage($"'{field.Label}' is required.")
                    .Must(d =>
                    {
                        d.TryGetValue(camelKey, out var val);
                        if (val == null) d.TryGetValue(field.Name, out val);
                        return val != null && !string.IsNullOrWhiteSpace(val.ToString());
                    })
                    .WithMessage($"'{field.Label}' cannot be empty.");
            }

            if (field.MaxLength > 0)
            {
                RuleFor(d => d)
                    .Must(d =>
                    {
                        d.TryGetValue(camelKey, out var val);
                        if (val == null) d.TryGetValue(field.Name, out val);
                        return val == null || val.ToString()!.Length <= field.MaxLength;
                    })
                    .WithMessage($"'{field.Label}' must not exceed {field.MaxLength} characters.");
            }

            if (field.FieldType == "email")
            {
                RuleFor(d => d)
                    .Must(d =>
                    {
                        d.TryGetValue(camelKey, out var val);
                        if (val == null) d.TryGetValue(field.Name, out val);
                        if (val == null || string.IsNullOrEmpty(val.ToString())) return true;
                        return val.ToString()!.Contains('@') && val.ToString()!.Contains('.');
                    })
                    .WithMessage($"'{field.Label}' must be a valid email address.");
            }

            if (field.Min.HasValue || field.Max.HasValue)
            {
                RuleFor(d => d)
                    .Must(d =>
                    {
                        d.TryGetValue(camelKey, out var val);
                        if (val == null) d.TryGetValue(field.Name, out val);
                        if (val == null) return true;
                        if (!double.TryParse(val.ToString(), out var num)) return true;
                        if (field.Min.HasValue && num < field.Min.Value) return false;
                        if (field.Max.HasValue && num > field.Max.Value) return false;
                        return true;
                    })
                    .WithMessage($"'{field.Label}' must be between {field.Min ?? double.MinValue} and {field.Max ?? double.MaxValue}.");
            }
        }
    }
}
