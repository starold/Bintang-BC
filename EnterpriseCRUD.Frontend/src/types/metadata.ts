// Metadata types matching the backend EntityMetadata/FieldMetadata models

export interface EnumChoice {
    value: number;
    label: string;
}

export interface FieldMetadata {
    name: string;
    label: string;
    fieldType: string; // string, int, decimal, date, datetime, email, enum, guid, reference, bool
    isRequired: boolean;
    isSearchable: boolean;
    isFilterable: boolean;
    isSortable: boolean;
    showInList: boolean;
    showInShow: boolean;
    showInCreate: boolean;
    showInEdit: boolean;
    referenceEntity?: string;
    referenceDisplayField?: string;
    choices?: EnumChoice[];
    maxLength: number;
    min?: number;
    max?: number;
    defaultValue?: string;
    placeholder?: string;
    viewRoles: string[];
    editRoles: string[];
    isReadOnly: boolean;
    validationRegex?: string;
    tooltip?: string;
}

export interface EntityMetadata {
    name: string;
    displayName: string;
    displayNameSingular: string;
    entityTypeName: string;
    icon: string;
    fields: FieldMetadata[];
    defaultSortField: string;
    defaultSortOrder: string;
    defaultPageSize: number;
    allowedRoles: string[];
    includes: string[];
    group?: string;
}
