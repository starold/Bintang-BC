import React from 'react';
import {
    Show, SimpleShowLayout, TextField, DateField, NumberField, EmailField,
} from 'react-admin';
import type { FieldMetadata, EntityMetadata } from '../types/metadata';

interface DynamicShowProps {
    metadata: EntityMetadata;
}

const renderShowField = (field: FieldMetadata) => {
    const source = field.name.charAt(0).toLowerCase() + field.name.slice(1);
    const key = `show-${field.name}`;

    switch (field.fieldType) {
        case 'email':
            return <EmailField key={key} source={source} label={field.label} />;
        case 'date':
            return <DateField key={key} source={source} label={field.label} />;
        case 'datetime':
            return <DateField key={key} source={source} label={field.label} showTime />;
        case 'int':
        case 'decimal':
            return <NumberField key={key} source={source} label={field.label} />;
        case 'reference':
            return <TextField key={key} source={`${source.replace('Id', '')}Display`} label={field.label} />;
        default:
            return <TextField key={key} source={source} label={field.label} />;
    }
};

/**
 * Dynamic show/detail view — auto-generates fields from metadata.
 */
const DynamicShow: React.FC<DynamicShowProps> = ({ metadata }) => {
    const showFields = metadata.fields.filter(f => f.showInShow);

    return (
        <Show>
            <SimpleShowLayout>
                {showFields.map(field => renderShowField(field))}
            </SimpleShowLayout>
        </Show>
    );
};

export default DynamicShow;
