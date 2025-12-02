import axios from 'axios';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7137';

export const api = axios.create({
  baseURL: `${API_URL}/api`,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle responses
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Auth API
export const authApi = {
  login: (email: string, password: string) =>
    api.post('/auth/login', { email, password }),
  register: (data: any) => api.post('/auth/register', data),
};

// Attendance API
export const attendanceApi = {
  getAll: (date?: string) => api.get('/attendance', { params: { date } }),
  getMyAttendance: (startDate?: string, endDate?: string) =>
    api.get('/attendance/my-attendance', { params: { startDate, endDate } }),
  checkIn: (data: any) => api.post('/attendance/check-in', data),
  checkOut: (data: any) => api.post('/attendance/check-out', data),
};

// Tasks API
export const tasksApi = {
  getAll: (status?: string) => api.get('/tasks', { params: { status } }),
  getById: (id: number) => api.get(`/tasks/${id}`),
  create: (data: any) => api.post('/tasks', data),
  update: (id: number, data: any) => api.put(`/tasks/${id}`, data),
  delete: (id: number) => api.delete(`/tasks/${id}`),
};

// Leave API
export const leaveApi = {
  getAll: (status?: string) => api.get('/leave', { params: { status } }),
  getById: (id: number) => api.get(`/leave/${id}`),
  create: (data: any) => api.post('/leave', data),
  approve: (data: any) => api.post('/leave/approve', data),
  delete: (id: number) => api.delete(`/leave/${id}`),
};

// Users API
export const usersApi = {
  getAll: (role?: string, isActive?: boolean) =>
    api.get('/users', { params: { role, isActive } }),
  getById: (id: number) => api.get(`/users/${id}`),
  update: (id: number, data: any) => api.put(`/users/${id}`, data),
  delete: (id: number) => api.delete(`/users/${id}`),
};

// Dashboard API
export const dashboardApi = {
  getStats: () => api.get('/dashboard/stats'),
  getRecentActivity: () => api.get('/dashboard/recent-activity'),
  getAttendanceSummary: (days: number = 7) =>
    api.get('/dashboard/attendance-summary', { params: { days } }),
};
