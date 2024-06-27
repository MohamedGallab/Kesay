import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:7095/api',
});

api.interceptors.request.use(config => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export const login = async (credentials) => {
    try {
        const response = await api.post('/auth/login', credentials);
        return response;
    } catch (error) {
        if (error.response && error.response.data.errors) {
            throw error.response.data;
        } else {
            throw new Error('An error occurred');
        }
    }
};

export const addTask = async (task) => {
    try {
        const response = await api.post('/tasks', task);
        return response;
    } catch (error) {
        if (error.response && error.response.data.errors) {
            throw error.response.data;
        } else {
            throw new Error('An error occurred');
        }
    }
};

export const updateTask = async (id, task) => {
    try {
        const response = await api.put(`/tasks/${id}`, task);
        return response;
    } catch (error) {
        if (error.response && error.response.data.errors) {
            throw error.response.data;
        } else {
            throw new Error('An error occurred');
        }
    }
};

export const deleteTask = async (id) => {
    try {
        await api.delete(`/tasks/${id}`);
    } catch (error) {
        if (error.response && error.response.data.errors) {
            throw error.response.data;
        } else {
            throw new Error('An error occurred');
        }
    }
};

export const getTasks = async () => {
    try {
        const response = await api.get('/tasks');
        return response;
    } catch (error) {
        if (error.response && error.response.data.errors) {
            throw error.response.data;
        } else {
            throw new Error('An error occurred');
        }
    }
};

export const register = async (credentials) => {
    try {
        const response = await api.post('/auth/register', credentials);
        return response;
    } catch (error) {
        if (error.response && error.response.data.errors) {
            throw error.response.data;
        } else {
            throw new Error('An error occurred');
        }
    }
};
