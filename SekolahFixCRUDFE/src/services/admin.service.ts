import api from './api';
import type { CourseDto, PaginatedResult, ServiceResult, StudentProfileDto, TeacherDto } from '../types';

export const adminService = {
  getSummary: async () => {
    // Note: If the backend doesn't have a combined summary endpoint, we could fetch them separately
    // For now, let's assume there's a way to get these counts
    const courses = await api.get<ServiceResult<PaginatedResult<CourseDto>>>('/admin/courses');
    const students = await api.get<ServiceResult<PaginatedResult<StudentProfileDto>>>('/admin/students');
    const teachers = await api.get<ServiceResult<PaginatedResult<TeacherDto>>>('/admin/teachers');
    
    const studentItems = students.data.data.items;

    return {
      totalCourses: courses.data.data.totalCount,
      totalStudents: students.data.data.totalCount,
      totalTeachers: teachers.data.data.totalCount,
      pendingApprovals: studentItems.filter(s => s.approvalStatus === 'Pending').length
    };
  },

  getStudents: async (page = 1, size = 10) => {
    const response = await api.get<ServiceResult<PaginatedResult<StudentProfileDto>>>(`/admin/students?pageNumber=${page}&pageSize=${size}`);
    return response.data;
  },

  approveStudent: async (id: number) => {
    const response = await api.post<ServiceResult<any>>(`/admin/student/approve/${id}`);
    return response.data;
  },

  rejectStudent: async (id: number) => {
    const response = await api.post<ServiceResult<any>>(`/admin/student/reject/${id}`);
    return response.data;
  },

  getCourses: async (page = 1, size = 10) => {
    const response = await api.get<ServiceResult<PaginatedResult<CourseDto>>>(`/admin/courses?pageNumber=${page}&pageSize=${size}`);
    return response.data;
  },

  createCourse: async (data: any) => {
    const response = await api.post<ServiceResult<CourseDto>>('/admin/course', data);
    return response.data;
  },

  updateCourse: async (id: number, data: any) => {
    const response = await api.put<ServiceResult<CourseDto>>(`/admin/course/${id}`, data);
    return response.data;
  },

  deleteCourse: async (id: number) => {
    const response = await api.delete<ServiceResult<any>>(`/admin/course/${id}`);
    return response.data;
  },

  getTeachers: async (page = 1, size = 10) => {
    const response = await api.get<ServiceResult<PaginatedResult<TeacherDto>>>(`/admin/teachers?pageNumber=${page}&pageSize=${size}`);
    return response.data;
  }
};
