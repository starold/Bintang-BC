import React from 'react';
import {
    List, Datagrid, TextField, DateField, NumberField, EmailField,
    EditButton, DeleteButton, TextInput, SelectInput, SearchInput,
    FilterButton, TopToolbar, CreateButton, ExportButton,
} from 'react-admin';
import type { FieldMetadata, EntityMetadata } from '../types/metadata';

interface DynamicListProps {
    metadata: EntityMetadata;
}

/**
 * Renders a field column in the datagrid based on metadata field type.
 */
const renderField = (field: FieldMetadata) => {
    const source = field.name.charAt(0).toLowerCase() + field.name.slice(1);
    const key = `field-${field.name}`;

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
            // Display the resolved display field from the backend
            return <TextField key={key} source={`${source.replace('Id', '')}Display`} label={field.label} />;
        case 'enum':
            if (field.choices) {
                const choiceMap = Object.fromEntries(field.choices.map(c => [c.value, c.label]));
                return (
                    <TextField
                        key={key}
                        source={source}
                        label={field.label}
                        // @ts-ignore - React Admin supports format on TextField
                        format={(v: number) => choiceMap[v] || v}
                    />
                );
            }
            return <TextField key={key} source={source} label={field.label} />;
        default:
            return <TextField key={key} source={source} label={field.label} />;
    }
};

/**
 * Renders filter inputs based on metadata.
 */
const buildFilters = (metadata: EntityMetadata) => {
    const filters: React.ReactElement[] = [
        <SearchInput key="search" source="q" alwaysOn />,
    ];

    metadata.fields
        .filter(f => f.isFilterable)
        .forEach(field => {
            const source = field.name.charAt(0).toLowerCase() + field.name.slice(1);
            if (field.fieldType === 'enum' && field.choices) {
                filters.push(
                    <SelectInput
                        key={`filter-${field.name}`}
                        source={source}
                        label={field.label}
                        choices={field.choices.map(c => ({ id: c.value, name: c.label }))}
                    />
                );
            } else if (field.fieldType === 'reference' && field.referenceEntity) {
                filters.push(
                    <TextInput
                        key={`filter-${field.name}`}
                        source={source}
                        label={field.label}
                    />
                );
            }
        });

    return filters;
};

const ListActions = () => (
    <TopToolbar>
        <FilterButton />
        <CreateButton />
        <ExportButton />
    </TopToolbar>
);

/**
 * Dynamic list component — auto-generates datagrid columns from metadata.
 */
const DynamicList: React.FC<DynamicListProps> = ({ metadata }) => {
    const permissions = localStorage.getItem('role') || '';
    const listFields = metadata.fields
        .filter(f => f.showInList)
        .filter(f => f.viewRoles.length === 0 || f.viewRoles.includes(permissions));

    const filters = buildFilters(metadata);
    const sortField = metadata.defaultSortField.charAt(0).toLowerCase() + metadata.defaultSortField.slice(1);

    return (
        <List
            filters={filters}
            actions={<ListActions />}
            sort={{ field: sortField, order: metadata.defaultSortOrder.toUpperCase() as 'ASC' | 'DESC' }}
            perPage={metadata.defaultPageSize}
        >
            <Datagrid rowClick="show">
                {listFields.map(field => renderField(field))}
                <EditButton />
                <DeleteButton />
            </Datagrid>
        </List>
    );
};

export default DynamicList;
