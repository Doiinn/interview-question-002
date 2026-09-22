import axios from 'axios';

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_BACKEND_API_URL || 'http://localhost:5178',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
});

export const apiClientWithToken = axios.create({ 
  baseURL: import.meta.env.VITE_BACKEND_API_URL || 'http://localhost:5178'
});

apiClientWithToken.interceptors.request.use(
  (config) => {
    const token = sessionStorage.getItem('token');
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

apiClient.interceptors.response.use(
  (response) => response.data,
  (error) => {
    // error handling
    if (!error.response) {
      return Promise.reject({ message: 'network problem, retry again' });
    }
    return Promise.reject(error.response.data);
  }
);
