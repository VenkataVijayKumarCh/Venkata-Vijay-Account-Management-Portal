import { Account, Project, Associate, Allocation } from '../types';

export const mockAccounts: Account[] = [
  {
    id: '1',
    name: 'TechCorp Solutions',
    description: 'Enterprise software development and consulting services',
    status: 'active',
    createdAt: '2024-01-15',
    contactEmail: 'contact@techcorp.com',
    contactPhone: '+1-555-0123',
    projectsCount: 3,
    associatesCount: 8
  },
  {
    id: '2',
    name: 'Digital Innovations Ltd',
    description: 'Digital transformation and cloud migration services',
    status: 'active',
    createdAt: '2024-02-20',
    contactEmail: 'info@digitalinnovations.com',
    contactPhone: '+1-555-0456',
    projectsCount: 2,
    associatesCount: 5
  },
  {
    id: '3',
    name: 'StartupX',
    description: 'Agile startup focused on mobile app development',
    status: 'pending',
    createdAt: '2024-03-10',
    contactEmail: 'team@startupx.com',
    contactPhone: '+1-555-0789',
    projectsCount: 1,
    associatesCount: 3
  }
];

export const mockProjects: Project[] = [
  {
    id: '1',
    name: 'ERP System Modernization',
    description: 'Complete overhaul of legacy ERP system with modern architecture',
    accountId: '1',
    status: 'active',
    startDate: '2024-01-01',
    endDate: '2024-12-31',
    budget: 500000,
    progress: 65,
    associatesCount: 6,
    manager: 'Sarah Johnson'
  },
  {
    id: '2',
    name: 'Mobile Banking App',
    description: 'Cross-platform mobile banking application with advanced security',
    accountId: '1',
    status: 'planning',
    startDate: '2024-04-01',
    endDate: '2024-10-31',
    budget: 350000,
    progress: 15,
    associatesCount: 4,
    manager: 'Mike Chen'
  },
  {
    id: '3',
    name: 'Cloud Migration Initiative',
    description: 'Migration of on-premises infrastructure to AWS cloud',
    accountId: '2',
    status: 'active',
    startDate: '2024-02-15',
    endDate: '2024-08-15',
    budget: 200000,
    progress: 40,
    associatesCount: 3,
    manager: 'Alex Rodriguez'
  },
  {
    id: '4',
    name: 'E-commerce Platform',
    description: 'Custom e-commerce platform with inventory management',
    accountId: '3',
    status: 'completed',
    startDate: '2023-09-01',
    endDate: '2024-03-01',
    budget: 150000,
    progress: 100,
    associatesCount: 4,
    manager: 'Emily Davis'
  }
];

export const mockAssociates: Associate[] = [
  {
    id: '1',
    name: 'John Smith',
    email: 'john.smith@company.com',
    role: 'Senior Developer',
    type: 'FTE',
    skills: ['React', 'Node.js', 'PostgreSQL', 'TypeScript'],
    status: 'allocated',
    currentProject: 'ERP System Modernization',
    allocationPercentage: 100,
    hourlyRate: 85,
    avatar: 'https://images.pexels.com/photos/2379004/pexels-photo-2379004.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=1'
  },
  {
    id: '2',
    name: 'Sarah Johnson',
    email: 'sarah.johnson@company.com',
    role: 'Project Manager',
    type: 'FTE',
    skills: ['Project Management', 'Agile', 'Scrum', 'Leadership'],
    status: 'allocated',
    currentProject: 'ERP System Modernization',
    allocationPercentage: 80,
    hourlyRate: 95,
    avatar: 'https://images.pexels.com/photos/1239291/pexels-photo-1239291.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=1'
  },
  {
    id: '3',
    name: 'Mike Chen',
    email: 'mike.chen@company.com',
    role: 'UI/UX Designer',
    type: 'Contractor',
    skills: ['Figma', 'Adobe XD', 'User Research', 'Prototyping'],
    status: 'available',
    allocationPercentage: 0,
    hourlyRate: 75,
    avatar: 'https://images.pexels.com/photos/1181686/pexels-photo-1181686.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=1'
  },
  {
    id: '4',
    name: 'Emily Davis',
    email: 'emily.davis@company.com',
    role: 'Full Stack Developer',
    type: 'FTE',
    skills: ['Python', 'Django', 'React', 'PostgreSQL'],
    status: 'allocated',
    currentProject: 'Cloud Migration Initiative',
    allocationPercentage: 75,
    hourlyRate: 80,
    avatar: 'https://images.pexels.com/photos/2379005/pexels-photo-2379005.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=1'
  },
  {
    id: '5',
    name: 'Alex Rodriguez',
    email: 'alex.rodriguez@company.com',
    role: 'DevOps Engineer',
    type: 'FTE',
    skills: ['AWS', 'Docker', 'Kubernetes', 'CI/CD'],
    status: 'allocated',
    currentProject: 'Cloud Migration Initiative',
    allocationPercentage: 90,
    hourlyRate: 90,
    avatar: 'https://images.pexels.com/photos/2379004/pexels-photo-2379004.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=1'
  },
  {
    id: '6',
    name: 'Jessica Park',
    email: 'jessica.park@company.com',
    role: 'Junior Developer',
    type: 'Intern',
    skills: ['JavaScript', 'HTML', 'CSS', 'Git'],
    status: 'available',
    allocationPercentage: 0,
    hourlyRate: 25,
    avatar: 'https://images.pexels.com/photos/1239291/pexels-photo-1239291.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=1'
  },
  {
    id: '7',
    name: 'David Wilson',
    email: 'david.wilson@company.com',
    role: 'Security Consultant',
    type: 'Contractor',
    skills: ['Cybersecurity', 'Penetration Testing', 'Risk Assessment', 'Compliance'],
    status: 'allocated',
    currentProject: 'ERP System Modernization',
    allocationPercentage: 50,
    hourlyRate: 120,
    avatar: 'https://images.pexels.com/photos/1181686/pexels-photo-1181686.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=1'
  }
];

export const mockAllocations: Allocation[] = [
  {
    id: '1',
    associateId: '1',
    projectId: '1',
    startDate: '2024-01-01',
    endDate: '2024-12-31',
    percentage: 100,
    role: 'Senior Developer',
    status: 'active'
  },
  {
    id: '2',
    associateId: '2',
    projectId: '1',
    startDate: '2024-01-01',
    endDate: '2024-12-31',
    percentage: 80,
    role: 'Project Manager',
    status: 'active'
  },
  {
    id: '3',
    associateId: '4',
    projectId: '3',
    startDate: '2024-02-15',
    endDate: '2024-08-15',
    percentage: 75,
    role: 'Full Stack Developer',
    status: 'active'
  },
  {
    id: '4',
    associateId: '5',
    projectId: '3',
    startDate: '2024-02-15',
    endDate: '2024-08-15',
    percentage: 90,
    role: 'DevOps Engineer',
    status: 'active'
  }
];