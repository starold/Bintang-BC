import type { AuthProvider } from 'react-admin';

const API_URL = 'http://localhost:5146/api/v1';

interface LoginResponse {
    success: boolean;
    token: string;
    email: string;
    fullName: string;
    roles: string[];
}

const authProvider: AuthProvider = {
    login: async ({ username, password }: { username: string; password: string }) => {
        const response = await fetch(`${API_URL}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: username, password }),
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.errors?.[0] || 'Login failed');
        }

        const data: LoginResponse = await response.json();
        localStorage.setItem('token', data.token);
        localStorage.setItem('email', data.email);
        localStorage.setItem('fullName', data.fullName);
        localStorage.setItem('roles', JSON.stringify(data.roles));
        return Promise.resolve();
    },

    logout: () => {
        localStorage.removeItem('token');
        localStorage.removeItem('email');
        localStorage.removeItem('fullName');
        localStorage.removeItem('roles');
        return Promise.resolve();
    },

    checkAuth: () => {
        return localStorage.getItem('token') ? Promise.resolve() : Promise.reject();
    },

    checkError: (error: { status: number }) => {
        if (error.status === 401 || error.status === 403) {
            localStorage.removeItem('token');
            return Promise.reject();
        }
        return Promise.resolve();
    },

    getIdentity: () => {
        const fullName = localStorage.getItem('fullName') || 'User';
        const email = localStorage.getItem('email') || '';
        return Promise.resolve({ id: email, fullName });
    },

    getPermissions: () => {
        const roles = localStorage.getItem('roles');
        return Promise.resolve(roles ? JSON.parse(roles) : []);
    },
};

export default authProvider;
