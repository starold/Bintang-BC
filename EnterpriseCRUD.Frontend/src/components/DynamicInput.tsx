import React from 'react';
import {
    TextInput, NumberInput, DateInput, SelectInput,
    ReferenceInput, AutocompleteInput, BooleanInput,
} from 'react-admin';
import type { FieldMetadata } from '../types/metadata';

interface DynamicInputProps {
    field: FieldMetadata;
    isEdit?: boolean;
}

/**
 * Renders a form input based on the field's metadata type.
 */
const DynamicInput: React.FC<DynamicInputProps> = ({ field, isEdit = false }) => {
    const source = field.name.charAt(0).toLowerCase() + field.name.slice(1);
    const key = `input-${field.name}`;
    const permissions = localStorage.getItem('role') || '';

    // Check field-level permissions
    if (field.viewRoles.length > 0 && !field.viewRoles.includes(permissions)) {
        return null;
    }

    const isFieldReadOnly = field.isReadOnly || (field.editRoles.length > 0 && !field.editRoles.includes(permissions));
    const validate = field.isRequired ? { required: true } : undefined;

    const common = {
        key,
        source,
        label: field.label,
        fullWidth: true,
        helperText: field.tooltip || (field.placeholder ? `e.g. ${field.placeholder}` : undefined),
        isRequired: field.isRequired,
        disabled: isFieldReadOnly,
    };

    switch (field.fieldType) {
        case 'email':
            return <TextInput {...common} type="email" validate={validate ? [() => undefined] : undefined} />;
        case 'int':
            return (
                <NumberInput
                    {...common}
                    min={field.min}
                    max={field.max}
                />
            );
        case 'decimal':
            return (
                <NumberInput
                    {...common}
                    min={field.min}
                    max={field.max}
                    step={0.01}
                />
            );
        case 'date':
            return <DateInput {...common} />;
        case 'datetime':
            return <DateInput {...common} />;
        case 'bool':
            return <BooleanInput {...common} />;
        case 'enum':
            if (field.choices) {
                return (
                    <SelectInput
                        {...common}
                        choices={field.choices.map(c => ({ id: c.value, name: c.label }))}
                    />
                );
            }
            return <TextInput {...common} />;
        case 'reference':
            if (field.referenceEntity) {
                return (
                    <ReferenceInput
                        key={key}
                        source={source}
                        reference={field.referenceEntity}
                        label={field.label}
                    >
                        <AutocompleteInput
                            optionText={field.referenceDisplayField
                                ? (record: Record<string, unknown>) => {
                                    const displayField = field.referenceDisplayField!;
                                    const camelField = displayField.charAt(0).toLowerCase() + displayField.slice(1);
                                    return (record?.[camelField] as string) || (record?.id as string) || '';
                                }
                                : 'id'
                            }
                            fullWidth
                        />
                    </ReferenceInput>
                );
            }
            return <TextInput {...common} />;
        case 'guid':
            if (isEdit) {
                return <TextInput {...common} disabled />;
            }
            return null;
        default:
            return (
                <TextInput
                    {...common}
                    multiline={field.maxLength > 500}
                />
            );
    }
};

export default DynamicInput;
