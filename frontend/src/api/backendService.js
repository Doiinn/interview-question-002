import { apiClient, apiClientWithToken } from './apiClient';

export const authenUser = (userData) => {
  return apiClient.post('/api/auth', userData);
};

export const registerUser = (userData) => {
  return apiClient.post('/api/register', userData);
};

export const getMember = () => {
  return apiClientWithToken.get('/api/member');
}