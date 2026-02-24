import React from 'react';
import { Edit, SimpleForm } from 'react-admin';
import DynamicInput from '../components/DynamicInput';
import type { EntityMetadata } from '../types/metadata';

interface DynamicEditProps {
    metadata: EntityMetadata;
}

/**
 * Dynamic edit form — auto-generates inputs from metadata.
 */
const DynamicEdit: React.FC<DynamicEditProps> = ({ metadata }) => {
    const editFields = metadata.fields.filter(f => f.showInEdit);

    return (
        <Edit>
            <SimpleForm>
                {editFields.map(field => (
                    <DynamicInput key={field.name} field={field} isEdit />
                ))}
            </SimpleForm>
        </Edit>
    );
};

export default DynamicEdit;
