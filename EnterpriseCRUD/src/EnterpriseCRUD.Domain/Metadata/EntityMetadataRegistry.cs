using EnterpriseCRUD.Domain.Entities;
using EnterpriseCRUD.Domain.Enums;

namespace EnterpriseCRUD.Domain.Metadata;

/// <summary>
/// Central registry that holds metadata for all entities.
/// To add a new entity to the system, simply add a new entry here.
/// No controller, service, or frontend changes needed.
/// </summary>
public static class EntityMetadataRegistry
{
    private static readonly Dictionary<string, EntityMetadata> _registry = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, Type> _typeMap = new(StringComparer.OrdinalIgnoreCase);

    static EntityMetadataRegistry()
    {
        RegisterAllEntities();
    }

    /// <summary>Get metadata for a specific entity by route name.</summary>
    public static EntityMetadata? GetMetadata(string entityName)
    {
        return _registry.TryGetValue(entityName, out var meta) ? meta : null;
    }

    /// <summary>Get the CLR type for a specific entity by route name.</summary>
    public static Type? GetEntityType(string entityName)
    {
        return _typeMap.TryGetValue(entityName, out var type) ? type : null;
    }

    /// <summary>Get all registered entity metadata.</summary>
    public static IReadOnlyDictionary<string, EntityMetadata> GetAll() => _registry;

    /// <summary>Get all registered entity metadata as a list (for migration).</summary>
    public static IEnumerable<EntityMetadata> GetAllMetadata() => _registry.Values;

    /// <summary>Register all entities with their metadata configuration.</summary>
    private static void RegisterAllEntities()
    {
        // ─── Students ──────────────────────────────────────────
        var studentMeta = new EntityMetadata
        {
            Name = "students",
            DisplayName = "Students",
            DisplayNameSingular = "Student",
            EntityTypeName = typeof(Student).FullName!,
            Icon = "mortarboard",
            DefaultSortField = "LastName",
            DefaultSortOrder = "asc",
            Includes = new List<string> { "Enrollments" },
            Fields = new List<FieldMetadata>
            {
                new() { Name = "Id", Label = "ID", FieldType = "guid", ShowInCreate = false, ShowInEdit = false, IsFilterable = false },
                new() { Name = "FirstName", Label = "First Name", FieldType = "string", IsRequired = true, IsSearchable = true, MaxLength = 100, Placeholder = "Enter first name" },
                new() { Name = "LastName", Label = "Last Name", FieldType = "string", IsRequired = true, IsSearchable = true, MaxLength = 100, Placeholder = "Enter last name" },
                new() { Name = "Email", Label = "Email", FieldType = "email", IsRequired = true, IsSearchable = true, MaxLength = 200, Placeholder = "student@school.com" },
                new() { Name = "DateOfBirth", Label = "Date of Birth", FieldType = "date", IsRequired = true, IsFilterable = true },
                new() { Name = "Gender", Label = "Gender", FieldType = "enum", IsRequired = true, IsFilterable = true, Choices = GetEnumChoices<Gender>() },
                new() { Name = "CreatedAt", Label = "Created", FieldType = "datetime", ShowInCreate = false, ShowInEdit = false, IsSortable = true },
                new() { Name = "UpdatedAt", Label = "Updated", FieldType = "datetime", ShowInCreate = false, ShowInEdit = false, IsSortable = true, ShowInList = false },
            }
        };
        Register<Student>("students", studentMeta);
        Register<Student>("student", new EntityMetadata { Name = "student", IsVisible = false, Fields = studentMeta.Fields, EntityTypeName = studentMeta.EntityTypeName });

        // ─── Teachers ──────────────────────────────────────────
        var teacherMeta = new EntityMetadata
        {
            Name = "teachers",
            DisplayName = "Teachers",
            DisplayNameSingular = "Teacher",
            EntityTypeName = typeof(Teacher).FullName!,
            Icon = "person-badge",
            DefaultSortField = "LastName",
            DefaultSortOrder = "asc",
            Includes = new List<string> { "ClassRooms", "TeacherSubjects" },
            Fields = new List<FieldMetadata>
            {
                new() { Name = "Id", Label = "ID", FieldType = "guid", ShowInCreate = false, ShowInEdit = false, IsFilterable = false },
                new() { Name = "FirstName", Label = "First Name", FieldType = "string", IsRequired = true, IsSearchable = true, MaxLength = 100 },
                new() { Name = "LastName", Label = "Last Name", FieldType = "string", IsRequired = true, IsSearchable = true, MaxLength = 100 },
                new() { Name = "Email", Label = "Email", FieldType = "email", IsRequired = true, IsSearchable = true, MaxLength = 200 },
                new() { Name = "Specialization", Label = "Specialization", FieldType = "string", IsRequired = true, IsSearchable = true, IsFilterable = true, MaxLength = 200 },
                new() { Name = "CreatedAt", Label = "Created", FieldType = "datetime", ShowInCreate = false, ShowInEdit = false },
                new() { Name = "UpdatedAt", Label = "Updated", FieldType = "datetime", ShowInCreate = false, ShowInEdit = false, ShowInList = false },
            }
        };
        Register<Teacher>("teachers", teacherMeta);
        Register<Teacher>("teacher", new EntityMetadata { Name = "teacher", IsVisible = false, Fields = teacherMeta.Fields, EntityTypeName = teacherMeta.EntityTypeName });

        // ─── ClassRooms ────────────────────────────────────────
        var classRoomMeta = new EntityMetadata
        {
            Name = "classrooms",
            DisplayName = "Class Rooms",
            DisplayNameSingular = "Class Room",
            EntityTypeName = typeof(ClassRoom).FullName!,
            Icon = "door-open",
            DefaultSortField = "Name",
            DefaultSortOrder = "asc",
            Includes = new List<string> { "Teacher" },
            Fields = new List<FieldMetadata>
            {
                new() { Name = "Id", Label = "ID", FieldType = "guid", ShowInCreate = false, ShowInEdit = false, IsFilterable = false },
                new() { Name = "Name", Label = "Room Name", FieldType = "string", IsRequired = true, IsSearchable = true, MaxLength = 100 },
                new() { Name = "Capacity", Label = "Capacity", FieldType = "int", IsRequired = true, IsFilterable = true, Min = 1, Max = 500 },
                new() { Name = "TeacherId", Label = "Teacher", FieldType = "reference", IsRequired = true, IsFilterable = true, ReferenceEntity = "teachers", ReferenceDisplayField = "FirstName" },
                new() { Name = "CreatedAt", Label = "Created", FieldType = "datetime", ShowInCreate = false, ShowInEdit = false },
                new() { Name = "UpdatedAt", Label = "Updated", FieldType = "datetime", ShowInCreate = false, ShowInEdit = false, ShowInList = false },
            }
        };
        Register<ClassRoom>("classrooms", classRoomMeta);
        Register<ClassRoom>("classroom", new EntityMetadata { Name = "classroom", IsVisible = false, Fields = classRoomMeta.Fields, EntityTypeName = classRoomMeta.EntityTypeName }); // Alias

        // ─── Subjects ──────────────────────────────────────────
        var subjectMeta = new EntityMetadata
        {
            Name = "subjects",
            DisplayName = "Subjects",
            DisplayNameSingular = "Subject",
            EntityTypeName = typeof(Subject).FullName!,
            Icon = "book",
            DefaultSortField = "Name",
            DefaultSortOrder = "asc",
            Fields = new List<FieldMetadata>
            {
                new() { Name = "Id", Label = "ID", FieldType = "guid", ShowInCreate = false, ShowInEdit = false, IsFilterable = false },
                new() { Name = "Name", Label = "Subject Name", FieldType = "string", IsRequired = true, IsSearchable = true, MaxLength = 200 },
                new() { Name = "Code", Label = "Code", FieldType = "string", IsRequired = true, IsSearchable = true, MaxLength = 20, Placeholder = "e.g. MATH101" },
                new() { Name = "Credits", Label = "Credits", FieldType = "int", IsRequired = true, IsFilterable = true, Min = 1, Max = 10 },
                new() { Name = "CreatedAt", Label = "Created", FieldType = "datetime", ShowInCreate = false, ShowInEdit = false },
                new() { Name = "UpdatedAt", Label = "Updated", FieldType = "datetime", ShowInCreate = false, ShowInEdit = false, ShowInList = false },
            }
        };
        Register<Subject>("subjects", subjectMeta);
        Register<Subject>("subject", new EntityMetadata { Name = "subject", IsVisible = false, Fields = subjectMeta.Fields, EntityTypeName = subjectMeta.EntityTypeName });

        // ─── Enrollments ───────────────────────────────────────
        var enrollmentMeta = new EntityMetadata
        {
            Name = "enrollments",
            DisplayName = "Enrollments",
            DisplayNameSingular = "Enrollment",
            EntityTypeName = typeof(Enrollment).FullName!,
            Icon = "journal-check",
            DefaultSortField = "EnrollmentDate",
            DefaultSortOrder = "desc",
            Includes = new List<string> { "Student", "Subject", "ClassRoom" },
            Fields = new List<FieldMetadata>
            {
                new() { Name = "Id", Label = "ID", FieldType = "guid", ShowInCreate = false, ShowInEdit = false, IsFilterable = false },
                new() { Name = "StudentId", Label = "Student", FieldType = "reference", IsRequired = true, IsFilterable = true, ReferenceEntity = "students", ReferenceDisplayField = "FirstName" },
                new() { Name = "SubjectId", Label = "Subject", FieldType = "reference", IsRequired = true, IsFilterable = true, ReferenceEntity = "subjects", ReferenceDisplayField = "Name" },
                new() { Name = "ClassRoomId", Label = "Class Room", FieldType = "reference", IsRequired = true, IsFilterable = true, ReferenceEntity = "classrooms", ReferenceDisplayField = "Name" },
                new() { Name = "EnrollmentDate", Label = "Enrollment Date", FieldType = "date", IsRequired = true, IsFilterable = true, IsSortable = true },
                new() { Name = "Grade", Label = "Grade", FieldType = "string", IsFilterable = true, MaxLength = 5, Placeholder = "A, B, C..." },
                new() { Name = "CreatedAt", Label = "Created", FieldType = "datetime", ShowInCreate = false, ShowInEdit = false },
                new() { Name = "UpdatedAt", Label = "Updated", FieldType = "datetime", ShowInCreate = false, ShowInEdit = false, ShowInList = false },
            }
        };
        Register<Enrollment>("enrollments", enrollmentMeta);
        Register<Enrollment>("enrollment", new EntityMetadata { Name = "enrollment", IsVisible = false, Fields = enrollmentMeta.Fields, EntityTypeName = enrollmentMeta.EntityTypeName });

        // ─── Metadata: Modules ──────────────────────────────────
        Register<EnterpriseCRUD.Domain.Entities.Metadata.ModuleDefinition>("metadata-modules", new EntityMetadata
        {
            Name = "metadata-modules",
            DisplayName = "Metadata: Modules",
            DisplayNameSingular = "Module",
            EntityTypeName = typeof(EnterpriseCRUD.Domain.Entities.Metadata.ModuleDefinition).FullName!,
            Icon = "layers",
            Group = "System",
            Fields = new List<FieldMetadata>
            {
                new() { Name = "Id", Label = "ID", FieldType = "guid", ShowInCreate = false, ShowInEdit = false },
                new() { Name = "Name", Label = "Internal Name", FieldType = "string", IsRequired = true },
                new() { Name = "DisplayName", Label = "Display Name", FieldType = "string", IsRequired = true },
                new() { Name = "Icon", Label = "Icon", FieldType = "string" },
                new() { Name = "Group", Label = "Group", FieldType = "string", IsFilterable = true },
                new() { Name = "IsActive", Label = "Active", FieldType = "bool" },
            }
        });

        // ─── Metadata: Fields ───────────────────────────────────
        Register<EnterpriseCRUD.Domain.Entities.Metadata.FieldDefinition>("metadata-fields", new EntityMetadata
        {
            Name = "metadata-fields",
            DisplayName = "Metadata: Fields",
            DisplayNameSingular = "Field",
            EntityTypeName = typeof(EnterpriseCRUD.Domain.Entities.Metadata.FieldDefinition).FullName!,
            Icon = "list-check",
            Group = "System",
            Fields = new List<FieldMetadata>
            {
                new() { Name = "IsSearchable", Label = "Searchable", FieldType = "bool" },
            }
        });

        // ─── Audit Logs ──────────────────────────────────────────
        Register<AuditLog>("audit-logs", new EntityMetadata
        {
            Name = "audit-logs",
            DisplayName = "Audit Logs",
            DisplayNameSingular = "Audit Log",
            EntityTypeName = typeof(AuditLog).FullName!,
            Icon = "clock-history",
            Group = "System",
            DefaultSortField = "ChangedAt",
            DefaultSortOrder = "desc",
            Fields = new List<FieldMetadata>
            {
                new() { Name = "Id", Label = "ID", FieldType = "guid", ShowInCreate = false, ShowInEdit = false, ShowInList = false },
                new() { Name = "EntityName", Label = "Entity", FieldType = "string", IsFilterable = true },
                new() { Name = "EntityId", Label = "Entity ID", FieldType = "guid", ShowInList = false },
                new() { Name = "Action", Label = "Action", FieldType = "string", IsFilterable = true },
                new() { Name = "OldValues", Label = "Old Values", FieldType = "string", ShowInList = false },
                new() { Name = "NewValues", Label = "New Values", FieldType = "string", ShowInList = false },
                new() { Name = "ChangedBy", Label = "User", FieldType = "string", IsFilterable = true },
                new() { Name = "ChangedAt", Label = "Date", FieldType = "datetime", IsFilterable = true, IsSortable = true },
            }
        });
    }

    private static void Register<T>(string routeName, EntityMetadata metadata) where T : BaseEntity
    {
        _registry[routeName] = metadata;
        _typeMap[routeName] = typeof(T);
    }

    /// <summary>Helper to generate enum choices from a C# enum type.</summary>
    private static List<EnumChoice> GetEnumChoices<TEnum>() where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>()
            .Select(e => new EnumChoice { Value = Convert.ToInt32(e), Label = e.ToString() })
            .ToList();
    }
}
