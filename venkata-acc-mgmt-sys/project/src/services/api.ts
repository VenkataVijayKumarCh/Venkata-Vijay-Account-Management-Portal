// API Configuration and Service Layer for ASP.NET Core Backend
const API_BASE_URL = import.meta.env.VITE_REACT_APP_API_URL || 'https://localhost:7001/api';

// API Response Types
export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
  errors?: string[];
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// HTTP Client with Authentication
class ApiClient {
  private baseURL: string;

  constructor(baseURL: string) {
    this.baseURL = baseURL;
  }

  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<ApiResponse<T>> {
    const token = localStorage.getItem('authToken');
    
    const config: RequestInit = {
      headers: {
        'Content-Type': 'application/json',
        ...(token && { Authorization: `Bearer ${token}` }),
        ...options.headers,
      },
      ...options,
    };

    try {
      const response = await fetch(`${this.baseURL}${endpoint}`, config);
      
      if (!response.ok) {
        if (response.status === 401) {
          // Handle unauthorized - redirect to login
          localStorage.removeItem('authToken');
          localStorage.removeItem('user');
          window.location.href = '/login';
        }
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('API request failed:', error);
      throw error;
    }
  }

  async get<T>(endpoint: string): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, { method: 'GET' });
  }

  async post<T>(endpoint: string, data: any): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async put<T>(endpoint: string, data: any): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  async delete<T>(endpoint: string): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, { method: 'DELETE' });
  }
}

const apiClient = new ApiClient(API_BASE_URL);

// Authentication API
export const authApi = {
  login: async (email: string, password: string) => {
    return apiClient.post<{ token: string; user: any }>('/auth/login', {
      email,
      password,
    });
  },

  register: async (userData: {
    email: string;
    password: string;
    name: string;
    role: string;
  }) => {
    return apiClient.post<{ token: string; user: any }>('/auth/register', userData);
  },

  refreshToken: async () => {
    return apiClient.post<{ token: string }>('/auth/refresh', {});
  },

  logout: async () => {
    return apiClient.post<void>('/auth/logout', {});
  },
};

// Accounts API
export const accountsApi = {
  getAll: async (page = 1, pageSize = 10, search = '') => {
    return apiClient.get<PaginatedResponse<any>>(
      `/accounts?page=${page}&pageSize=${pageSize}&search=${encodeURIComponent(search)}`
    );
  },

  getById: async (id: string) => {
    return apiClient.get<any>(`/accounts/${id}`);
  },

  create: async (accountData: any) => {
    return apiClient.post<any>('/accounts', accountData);
  },

  update: async (id: string, accountData: any) => {
    return apiClient.put<any>(`/accounts/${id}`, accountData);
  },

  delete: async (id: string) => {
    return apiClient.delete<void>(`/accounts/${id}`);
  },

  getProjects: async (accountId: string) => {
    return apiClient.get<any[]>(`/accounts/${accountId}/projects`);
  },
};

// Projects API
export const projectsApi = {
  getAll: async (page = 1, pageSize = 10, search = '', status = '') => {
    return apiClient.get<PaginatedResponse<any>>(
      `/projects?page=${page}&pageSize=${pageSize}&search=${encodeURIComponent(search)}&status=${status}`
    );
  },

  getById: async (id: string) => {
    return apiClient.get<any>(`/projects/${id}`);
  },

  create: async (projectData: any) => {
    return apiClient.post<any>('/projects', projectData);
  },

  update: async (id: string, projectData: any) => {
    return apiClient.put<any>(`/projects/${id}`, projectData);
  },

  delete: async (id: string) => {
    return apiClient.delete<void>(`/projects/${id}`);
  },

  getAssociates: async (projectId: string) => {
    return apiClient.get<any[]>(`/projects/${projectId}/associates`);
  },

  updateProgress: async (id: string, progress: number) => {
    return apiClient.put<any>(`/projects/${id}/progress`, { progress });
  },
};

// Associates API
export const associatesApi = {
  getAll: async (page = 1, pageSize = 10, search = '', status = '', role = '') => {
    return apiClient.get<PaginatedResponse<any>>(
      `/associates?page=${page}&pageSize=${pageSize}&search=${encodeURIComponent(search)}&status=${status}&role=${role}`
    );
  },

  getById: async (id: string) => {
    return apiClient.get<any>(`/associates/${id}`);
  },

  create: async (associateData: any) => {
    return apiClient.post<any>('/associates', associateData);
  },

  update: async (id: string, associateData: any) => {
    return apiClient.put<any>(`/associates/${id}`, associateData);
  },

  delete: async (id: string) => {
    return apiClient.delete<void>(`/associates/${id}`);
  },

  getAvailable: async () => {
    return apiClient.get<any[]>('/associates/available');
  },

  updateStatus: async (id: string, status: string) => {
    return apiClient.put<any>(`/associates/${id}/status`, { status });
  },
};

// Allocations API
export const allocationsApi = {
  getAll: async (page = 1, pageSize = 10, search = '', status = '') => {
    return apiClient.get<PaginatedResponse<any>>(
      `/allocations?page=${page}&pageSize=${pageSize}&search=${encodeURIComponent(search)}&status=${status}`
    );
  },

  getById: async (id: string) => {
    return apiClient.get<any>(`/allocations/${id}`);
  },

  create: async (allocationData: any) => {
    return apiClient.post<any>('/allocations', allocationData);
  },

  update: async (id: string, allocationData: any) => {
    return apiClient.put<any>(`/allocations/${id}`, allocationData);
  },

  delete: async (id: string) => {
    return apiClient.delete<void>(`/allocations/${id}`);
  },

  getByAssociate: async (associateId: string) => {
    return apiClient.get<any[]>(`/allocations/associate/${associateId}`);
  },

  getByProject: async (projectId: string) => {
    return apiClient.get<any[]>(`/allocations/project/${projectId}`);
  },
};

// Dashboard API
export const dashboardApi = {
  getStats: async () => {
    return apiClient.get<{
      totalAccounts: number;
      activeProjects: number;
      totalAssociates: number;
      availableAssociates: number;
      totalBudget: number;
      avgAllocation: number;
    }>('/dashboard/stats');
  },

  getRecentProjects: async (limit = 5) => {
    return apiClient.get<any[]>(`/dashboard/recent-projects?limit=${limit}`);
  },

  getProjectStatusDistribution: async () => {
    return apiClient.get<{ status: string; count: number }[]>('/dashboard/project-status');
  },
};

export default apiClient;