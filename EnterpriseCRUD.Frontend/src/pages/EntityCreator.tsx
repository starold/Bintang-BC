import React from 'react';
import {
    SimpleForm,
    TextInput,
    SelectInput,
    BooleanInput,
    ArrayInput,
    SimpleFormIterator,
    useNotify,
    Title
} from 'react-admin';
import { Box, Card, CardContent, Typography } from '@mui/material';

const EntityCreator: React.FC = () => {
    const notify = useNotify();

    const handleSubmit = async (data: any) => {
        try {
            const response = await fetch(`${import.meta.env.VITE_API_URL}/api/v1/discovery/entities`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                },
                body: JSON.stringify({
                    name: data.name.toLowerCase(),
                    displayName: data.displayName || data.name,
                    displayNameSingular: data.displayNameSingular || data.name,
                    entityTypeName: data.displayName || data.name,
                    icon: data.icon || 'list',
                    fields: data.fields.map((f: any) => ({
                        ...f,
                        label: f.label || f.name,
                        showInList: true,
                        showInShow: true,
                        showInCreate: true,
                        showInEdit: true,
                        isSortable: true,
                        isFilterable: true,
                        isSearchable: f.fieldType === 'string',
                        viewRoles: [],
                        editRoles: []
                    })),
                    defaultSortField: 'Id',
                    defaultSortOrder: 'desc',
                    defaultPageSize: 25,
                    allowedRoles: [],
                    includes: [],
                    group: null
                })
            });

            if (response.ok) {
                notify('Entity created successfully. Refreshing page...', { type: 'success' });
                // We need to reload to fetch new metadata
                setTimeout(() => {
                    window.location.href = '/';
                }, 2000);
            } else {
                const err = await response.json();
                notify(`Error: ${err.error || 'Failed to create entity'}`, { type: 'warning' });
            }
        } catch (error) {
            notify('System error occurred', { type: 'warning' });
        }
    };

    return (
        <Box mt={2}>
            <Title title="Create New Entity" />
            <Card>
                <CardContent>
                    <Typography variant="h6" gutterBottom>
                        Schema Designer
                    </Typography>
                    <Typography variant="body2" color="textSecondary" mb={3}>
                        Define a new database table and its fields. The system will automatically generate the backend code and UI.
                    </Typography>

                    <SimpleForm onSubmit={handleSubmit}>
                        <Box sx={{ display: 'flex', gap: 2, mb: 4 }}>
                            <TextInput source="name" label="Table Name (lowercased)" fullWidth required />
                            <TextInput source="displayName" label="Display Name" fullWidth required />
                            <TextInput source="icon" label="MUI Icon Name" defaultValue="list" fullWidth />
                        </Box>

                        <Typography variant="subtitle1" gutterBottom>Fields</Typography>
                        <ArrayInput source="fields">
                            <SimpleFormIterator inline>
                                <TextInput source="name" label="Column Name" required />
                                <SelectInput source="fieldType" label="Type" choices={[
                                    { id: 'string', name: 'String' },
                                    { id: 'int', name: 'Integer' },
                                    { id: 'decimal', name: 'Decimal' },
                                    { id: 'bool', name: 'Boolean' },
                                    { id: 'date', name: 'Date' },
                                    { id: 'datetime', name: 'DateTime' },
                                    { id: 'guid', name: 'Guid' },
                                ]} required />
                                <BooleanInput source="isRequired" label="Required" defaultValue={false} />
                            </SimpleFormIterator>
                        </ArrayInput>
                    </SimpleForm>
                </CardContent>
            </Card>
        </Box>
    );
};

export default EntityCreator;
