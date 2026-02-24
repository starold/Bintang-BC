using System.Text;
using EnterpriseCRUD.Domain.Metadata;

namespace EnterpriseCRUD.Infrastructure.Services;

/// <summary>
/// Service to generate C# source files based on entity metadata.
/// This fulfills the "Code Generation Pipeline" requirement.
/// </summary>
public class CodeGenerationService
{
    private readonly string _basePath;

    public CodeGenerationService(string basePath)
    {
        _basePath = basePath;
    }

    public void GenerateEntity(EntityMetadata metadata)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using EnterpriseCRUD.Domain.Entities;");
        sb.AppendLine("using EnterpriseCRUD.Domain.Enums;");
        sb.AppendLine();
        sb.AppendLine("namespace EnterpriseCRUD.Domain.Entities;");
        sb.AppendLine();
        sb.AppendLine($"public class {metadata.EntityTypeName} : BaseEntity");
        sb.AppendLine("{");

        foreach (var field in metadata.Fields)
        {
            if (field.Name == "Id" || field.Name == "CreatedAt" || field.Name == "UpdatedAt") continue;

            var type = MapMetadataTypeToCSharp(field.FieldType);
            var nullable = field.IsRequired ? "" : "?";
            
            sb.AppendLine($"    public {type}{nullable} {field.Name} {{ get; set; }}");
        }

        sb.AppendLine("}");

        var path = Path.Combine(_basePath, "EnterpriseCRUD.Domain", "Entities", $"{metadata.EntityTypeName}.cs");
        WriteFile(path, sb.ToString());
    }

    public void GenerateConfiguration(EntityMetadata metadata)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using EnterpriseCRUD.Domain.Entities;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
        sb.AppendLine();
        sb.AppendLine("namespace EnterpriseCRUD.Infrastructure.Data.Configurations;");
        sb.AppendLine();
        sb.AppendLine($"public class {metadata.EntityTypeName}Configuration : IEntityTypeConfiguration<{metadata.EntityTypeName}>");
        sb.AppendLine("{");
        sb.AppendLine($"    public void Configure(EntityTypeBuilder<{metadata.EntityTypeName}> builder)");
        sb.AppendLine("    {");
        sb.AppendLine($"        builder.ToTable(\"{metadata.Name}\");");
        
        foreach (var field in metadata.Fields)
        {
            if (field.MaxLength > 0 && field.FieldType == "string")
            {
                sb.AppendLine($"        builder.Property(x => x.{field.Name}).HasMaxLength({field.MaxLength});");
            }
            if (field.IsRequired && field.Name != "Id")
            {
                sb.AppendLine($"        builder.Property(x => x.{field.Name}).IsRequired();");
            }
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        var path = Path.Combine(_basePath, "EnterpriseCRUD.Infrastructure", "Data", "Configurations", $"{metadata.EntityTypeName}Configuration.cs");
        WriteFile(path, sb.ToString());
    }

    private string MapMetadataTypeToCSharp(string fieldType)
    {
        return fieldType switch
        {
            "int" => "int",
            "decimal" => "decimal",
            "date" or "datetime" => "DateTime",
            "bool" => "bool",
            "guid" or "reference" => "Guid",
            _ => "string"
        };
    }

    private void WriteFile(string path, string content)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        File.WriteAllText(path, content);
    }
}
