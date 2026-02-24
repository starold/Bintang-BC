import { type DataProvider, fetchUtils } from 'react-admin';

const API_URL = `${import.meta.env.VITE_API_URL}/api/v1`;

const httpClient = (url: string, options: fetchUtils.Options = {}) => {
    const token = localStorage.getItem('token');
    const headers = new Headers(options.headers as HeadersInit || {});
    if (token) {
        headers.set('Authorization', `Bearer ${token}`);
    }
    return fetchUtils.fetchJson(url, { ...options, headers });
};

/**
 * Custom data provider mapping React Admin's data methods
 * to the generic CRUD API endpoints.
 */
const dataProvider: DataProvider = {
    getList: async (resource, params) => {
        const { page = 1, perPage = 25 } = params.pagination || {};
        const { field = 'id', order = 'ASC' } = params.sort || {};

        const query = new URLSearchParams({
            _page: String(page),
            _perPage: String(perPage),
            _sortField: field,
            _sortOrder: order,
        });

        // Add filters
        if (params.filter) {
            Object.keys(params.filter).forEach(key => {
                if (key === 'q') {
                    query.set('_search', params.filter[key]);
                } else {
                    query.set(`filter[${key}]`, params.filter[key]);
                }
            });
        }

        const { json } = await httpClient(`${API_URL}/${resource}?${query.toString()}`);
        return {
            data: json.data.map((item: Record<string, unknown>) => ({ ...item, id: item.id })),
            total: json.total,
        };
    },

    getOne: async (resource, params) => {
        const { json } = await httpClient(`${API_URL}/${resource}/${params.id}`);
        return { data: { ...json, id: json.id } };
    },

    getMany: async (resource, params) => {
        const results = await Promise.all(
            params.ids.map(id => httpClient(`${API_URL}/${resource}/${id}`))
        );
        return {
            data: results.map(({ json }) => ({ ...json, id: json.id })),
        };
    },

    getManyReference: async (resource, params) => {
        const { page = 1, perPage = 25 } = params.pagination || {};
        const { field = 'id', order = 'ASC' } = params.sort || {};

        const query = new URLSearchParams({
            _page: String(page),
            _perPage: String(perPage),
            _sortField: field,
            _sortOrder: order,
            [`filter[${params.target}]`]: String(params.id),
        });

        const { json } = await httpClient(`${API_URL}/${resource}?${query.toString()}`);
        return {
            data: json.data.map((item: Record<string, unknown>) => ({ ...item, id: item.id })),
            total: json.total,
        };
    },

    create: async (resource, params) => {
        const { json } = await httpClient(`${API_URL}/${resource}`, {
            method: 'POST',
            body: JSON.stringify(params.data),
        });
        return { data: { ...json, id: json.id } };
    },

    update: async (resource, params) => {
        const { json } = await httpClient(`${API_URL}/${resource}/${params.id}`, {
            method: 'PUT',
            body: JSON.stringify(params.data),
        });
        return { data: { ...json, id: json.id } };
    },

    updateMany: async (resource, params) => {
        await Promise.all(
            params.ids.map(id =>
                httpClient(`${API_URL}/${resource}/${id}`, {
                    method: 'PUT',
                    body: JSON.stringify(params.data),
                })
            )
        );
        return { data: params.ids };
    },

    delete: async (resource, params) => {
        const { json } = await httpClient(`${API_URL}/${resource}/${params.id}`, {
            method: 'DELETE',
        });
        return { data: { ...json, id: params.id } };
    },

    deleteMany: async (resource, params) => {
        await Promise.all(
            params.ids.map(id =>
                httpClient(`${API_URL}/${resource}/${id}`, { method: 'DELETE' })
            )
        );
        return { data: params.ids };
    },
};

export default dataProvider;
