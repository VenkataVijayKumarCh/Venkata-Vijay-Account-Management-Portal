export interface User {
  id: string;
  email: string;
  name: string;
  role: 'admin' | 'manager' | 'user';
  avatar?: string;
}

export interface Account {
  id: string;
  name: string;
  description: string;
  status: 'active' | 'inactive' | 'pending';
  createdAt: string;
  contactEmail: string;
  contactPhone: string;
  projectsCount: number;
  associatesCount: number;
}

export interface Project {
  id: string;
  name: string;
  description: string;
  accountId: string;
  status: 'planning' | 'active' | 'completed' | 'on-hold';
  startDate: string;
  endDate: string;
  budget: number;
  progress: number;
  associatesCount: number;
  manager: string;
}

export interface Associate {
  id: string;
  name: string;
  email: string;
  role: string;
  type: 'FTE' | 'Contractor' | 'Intern';
  skills: string[];
  status: 'available' | 'allocated' | 'unavailable';
  currentProject?: string;
  allocationPercentage: number;
  hourlyRate: number;
  avatar?: string;
}

export interface Allocation {
  id: string;
  associateId: string;
  projectId: string;
  startDate: string;
  endDate: string;
  percentage: number;
  role: string;
  status: 'active' | 'completed' | 'planned';
}