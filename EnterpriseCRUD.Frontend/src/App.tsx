import { useEffect, useState, useMemo } from 'react';
import { Admin, Resource, Loading, CustomRoutes, Menu, Layout } from 'react-admin';
import { Route } from 'react-router-dom';
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import SchoolIcon from '@mui/icons-material/School';
import PersonIcon from '@mui/icons-material/Person';
import MeetingRoomIcon from '@mui/icons-material/MeetingRoom';
import BookIcon from '@mui/icons-material/Book';
import HowToRegIcon from '@mui/icons-material/HowToReg';
import authProvider from './providers/authProvider';
import dataProvider from './providers/dataProvider';
import type { EntityMetadata } from './types/metadata';
import DynamicList from './resources/DynamicList';
import DynamicCreate from './resources/DynamicCreate';
import DynamicEdit from './resources/DynamicEdit';
import DynamicShow from './resources/DynamicShow';
import EntityCreator from './pages/EntityCreator';

const API_URL = `${import.meta.env.VITE_API_URL}/api/v1`;

// Map icon names from metadata to MUI icons
const iconMap: Record<string, React.ComponentType> = {
    school: SchoolIcon,
    person: PersonIcon,
    meeting_room: MeetingRoomIcon,
    book: BookIcon,
    how_to_reg: HowToRegIcon,
};

const MyMenu = ({ metadata }: { metadata: EntityMetadata[] }) => (
    <Menu>
        <Menu.DashboardItem />
        {metadata.map(entity => (
            <Menu.Item
                key={entity.name}
                to={`/${entity.name}`}
                primaryText={entity.displayName}
                leftIcon={iconMap[entity.icon] || SchoolIcon}
            />
        ))}
        {localStorage.getItem('role') === 'Admin' && (
            <Menu.Item to="/create-entity" primaryText="Create Entity" leftIcon={<AddCircleOutlineIcon />} />
        )}
    </Menu>
);

const MyLayout = (props: any) => {
    const [metadata, setMetadata] = useState<EntityMetadata[]>([]);

    useEffect(() => {
        const fetchMetadata = async () => {
            const response = await fetch(`${API_URL}/metadata`);
            if (response.ok) {
                const data = await response.json();
                setMetadata(data);
            }
        };
        fetchMetadata();
    }, []);

    const permissions = localStorage.getItem('role') || '';
    const visibleMetadata = metadata.filter((entity: EntityMetadata) => {
        if (entity.allowedRoles.length === 0) return true;
        return entity.allowedRoles.includes(permissions);
    });

    return <Layout {...props} menu={() => <MyMenu metadata={visibleMetadata} />} />;
};

/**
 * Main application — fetches metadata and dynamically generates
 * React Admin resources for every registered entity.
 */
const App = () => {
    const [metadata, setMetadata] = useState<EntityMetadata[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchMetadata = async () => {
            try {
                const response = await fetch(`${API_URL}/metadata`);
                if (response.ok) {
                    const data: EntityMetadata[] = await response.json();
                    setMetadata(data);
                }
            } catch (err) {
                console.error('Failed to fetch metadata:', err);
            } finally {
                setLoading(false);
            }
        };
        fetchMetadata();
    }, []);

    const permissions = localStorage.getItem('role') || '';

    const visibleMetadata = useMemo(() => {
        return metadata.filter(entity => {
            if (entity.allowedRoles.length === 0) return true;
            return entity.allowedRoles.includes(permissions);
        });
    }, [metadata, permissions]);

    if (loading) return <Loading />;

    return (
        <Admin
            authProvider={authProvider}
            dataProvider={dataProvider}
            layout={MyLayout}
            title="Enterprise CRUD"
        >
            {visibleMetadata.map(entity => (
                <Resource
                    key={entity.name}
                    name={entity.name}
                    options={{ label: entity.displayName, group: entity.group }}
                    icon={iconMap[entity.icon] || SchoolIcon}
                    list={() => <DynamicList metadata={entity} />}
                    create={() => <DynamicCreate metadata={entity} />}
                    edit={() => <DynamicEdit metadata={entity} />}
                    show={() => <DynamicShow metadata={entity} />}
                />
            ))}
            <CustomRoutes>
                <Route path="/create-entity" element={<EntityCreator />} />
            </CustomRoutes>
        </Admin>
    );
};

export default App;
