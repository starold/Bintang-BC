export interface ServiceResult<T> {
  success: boolean;
  message: string;
  data: T;
  errorCode: string | null;
  statusCode: number;
  errors: string[];
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface AuthResponseDto {
  token: string;
  email: string;
  role: string;
}

export interface StudentProfileDto {
  userId: string;
  fullName: string;
  address: string;
  dateOfBirth: string;
  phoneNumber?: string;
  isApproved: boolean;
  approvalStatus: string;
  email: string;
}

export interface CourseDto {
  id: number;
  name: string;
  code: string;
  credits: number;
  description?: string;
  teacherId?: number;
  teacherName?: string;
  isDeleted: boolean;
}

export interface TeacherDto {
  id: number;
  fullName: string;
  email: string;
  department: string;
}

export interface DashboardSummaryDto {
  totalStudents: number;
  totalCourses: number;
  totalTeachers: number;
  pendingApprovals: number;
}

export {};
