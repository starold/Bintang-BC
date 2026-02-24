import React from 'react';
import { Create, SimpleForm } from 'react-admin';
import DynamicInput from '../components/DynamicInput';
import type { EntityMetadata } from '../types/metadata';

interface DynamicCreateProps {
    metadata: EntityMetadata;
}

/**
 * Dynamic create form — auto-generates inputs from metadata.
 */
const DynamicCreate: React.FC<DynamicCreateProps> = ({ metadata }) => {
    const createFields = metadata.fields.filter(f => f.showInCreate);

    return (
        <Create>
            <SimpleForm>
                {createFields.map(field => (
                    <DynamicInput key={field.name} field={field} />
                ))}
            </SimpleForm>
        </Create>
    );
};

export default DynamicCreate;
