import { useState, useEffect } from 'react';
import type { EntityMetadata } from '../types/metadata';

const API_URL = 'http://localhost:5146/api/v1';

/**
 * Hook to fetch entity metadata from the backend.
 * Called once on app startup — metadata is cached in state.
 */
export const useMetadata = () => {
    const [metadata, setMetadata] = useState<EntityMetadata[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchMetadata = async () => {
            try {
                const response = await fetch(`${API_URL}/metadata`);
                if (!response.ok) throw new Error('Failed to fetch metadata');
                const data: EntityMetadata[] = await response.json();
                setMetadata(data);
            } catch (err) {
                setError(err instanceof Error ? err.message : 'Unknown error');
            } finally {
                setLoading(false);
            }
        };

        fetchMetadata();
    }, []);

    return { metadata, loading, error };
};

/**
 * Get metadata for a specific entity by name.
 */
export const useEntityMetadata = (entityName: string) => {
    const { metadata, loading, error } = useMetadata();
    const entityMeta = metadata.find(m => m.name === entityName);
    return { metadata: entityMeta, loading, error };
};
