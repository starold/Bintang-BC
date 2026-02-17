import api from './api';
import type { CourseDto, PaginatedResult, ServiceResult, StudentProfileDto } from '../types';

export const studentService = {
  getProfile: async () => {
    const response = await api.get<ServiceResult<StudentProfileDto>>('/student/profile');
    return response.data;
  },

  updateProfile: async (data: any) => {
    const response = await api.put<ServiceResult<StudentProfileDto>>('/student/profile', data);
    return response.data;
  },

  getAvailableCourses: async (page = 1, size = 10) => {
    const response = await api.get<ServiceResult<PaginatedResult<CourseDto>>>(`/student/courses?pageNumber=${page}&pageSize=${size}`);
    return response.data;
  },

  enroll: async (courseIds: number[]) => {
    const response = await api.post<ServiceResult<any>>('/student/enroll', { courseIds });
    return response.data;
  }
};
