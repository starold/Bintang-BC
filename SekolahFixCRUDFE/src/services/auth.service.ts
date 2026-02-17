import api from './api';
import type { AuthResponseDto, ServiceResult } from '../types';

export const authService = {
  login: async (email: string, password: string) => {
    const response = await api.post<ServiceResult<AuthResponseDto>>('/auth/login', { email, password });
    return response.data;
  },
  
  register: async (data: any) => {
    const response = await api.post<ServiceResult<AuthResponseDto>>('/auth/register', data);
    return response.data;
  }
};
