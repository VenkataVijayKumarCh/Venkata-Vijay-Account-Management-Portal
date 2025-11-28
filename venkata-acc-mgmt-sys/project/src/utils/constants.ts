// API Endpoints for ASP.NET Core Backend
export const API_ENDPOINTS = {
  // Authentication
  AUTH: {
    LOGIN: '/auth/login',
    REGISTER: '/auth/register',
    REFRESH: '/auth/refresh',
    LOGOUT: '/auth/logout',
  },
  
  // Accounts
  ACCOUNTS: {
    BASE: '/accounts',
    BY_ID: (id: string) => `/accounts/${id}`,
    PROJECTS: (id: string) => `/accounts/${id}/projects`,
  },
  
  // Projects
  PROJECTS: {
    BASE: '/projects',
    BY_ID: (id: string) => `/projects/${id}`,
    ASSOCIATES: (id: string) => `/projects/${id}/associates`,
    PROGRESS: (id: string) => `/projects/${id}/progress`,
  },
  
  // Associates
  ASSOCIATES: {
    BASE: '/associates',
    BY_ID: (id: string) => `/associates/${id}`,
    AVAILABLE: '/associates/available',
    STATUS: (id: string) => `/associates/${id}/status`,
  },
  
  // Allocations
  ALLOCATIONS: {
    BASE: '/allocations',
    BY_ID: (id: string) => `/allocations/${id}`,
    BY_ASSOCIATE: (id: string) => `/allocations/associate/${id}`,
    BY_PROJECT: (id: string) => `/allocations/project/${id}`,
  },
  
  // Dashboard
  DASHBOARD: {
    STATS: '/dashboard/stats',
    RECENT_PROJECTS: '/dashboard/recent-projects',
    PROJECT_STATUS: '/dashboard/project-status',
  },
};

// Application Constants
export const APP_CONFIG = {
  NAME: 'ProjectHub',
  VERSION: '1.0.0',
  API_TIMEOUT: 30000,
  PAGINATION: {
    DEFAULT_PAGE_SIZE: 10,
    MAX_PAGE_SIZE: 100,
  },
};

// Status Options
export const STATUS_OPTIONS = {
  ACCOUNT: [
    { value: 'active', label: 'Active', color: 'green' },
    { value: 'inactive', label: 'Inactive', color: 'gray' },
    { value: 'pending', label: 'Pending', color: 'yellow' },
  ],
  PROJECT: [
    { value: 'planning', label: 'Planning', color: 'blue' },
    { value: 'active', label: 'Active', color: 'green' },
    { value: 'completed', label: 'Completed', color: 'gray' },
    { value: 'on-hold', label: 'On Hold', color: 'yellow' },
  ],
  ASSOCIATE: [
    { value: 'available', label: 'Available', color: 'green' },
    { value: 'allocated', label: 'Allocated', color: 'blue' },
    { value: 'unavailable', label: 'Unavailable', color: 'red' },
  ],
  ALLOCATION: [
    { value: 'active', label: 'Active', color: 'green' },
    { value: 'completed', label: 'Completed', color: 'gray' },
    { value: 'planned', label: 'Planned', color: 'blue' },
  ],
};

// Role Options
export const ROLE_OPTIONS = [
  { value: 'admin', label: 'Administrator' },
  { value: 'manager', label: 'Project Manager' },
  { value: 'user', label: 'User' },
  { value: 'developer', label: 'Developer' },
  { value: 'designer', label: 'Designer' },
  { value: 'devops', label: 'DevOps Engineer' },
  { value: 'analyst', label: 'Business Analyst' },
  { value: 'tester', label: 'QA Tester' },
];

// Associate Type Options
export const ASSOCIATE_TYPE_OPTIONS = [
  { value: 'FTE', label: 'Full-Time Employee (FTE)', color: 'blue' },
  { value: 'Contractor', label: 'Contractor', color: 'purple' },
  { value: 'Intern', label: 'Intern', color: 'orange' },
];

// Validation Rules
export const VALIDATION_RULES = {
  EMAIL: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
  PHONE: /^\+?[\d\s\-\(\)]+$/,
  PASSWORD: {
    MIN_LENGTH: 8,
    REQUIRE_UPPERCASE: true,
    REQUIRE_LOWERCASE: true,
    REQUIRE_NUMBER: true,
    REQUIRE_SPECIAL: true,
  },
  ALLOCATION_PERCENTAGE: {
    MIN: 0,
    MAX: 100,
  },
  HOURLY_RATE: {
    MIN: 0,
    MAX: 1000,
  },
};

// Date Formats
export const DATE_FORMATS = {
  DISPLAY: 'MMM dd, yyyy',
  INPUT: 'yyyy-MM-dd',
  DATETIME: 'MMM dd, yyyy HH:mm',
  API: 'yyyy-MM-dd\'T\'HH:mm:ss.SSS\'Z\'',
};

// Error Messages
export const ERROR_MESSAGES = {
  NETWORK: 'Network error. Please check your connection.',
  UNAUTHORIZED: 'You are not authorized to perform this action.',
  FORBIDDEN: 'Access denied.',
  NOT_FOUND: 'The requested resource was not found.',
  SERVER_ERROR: 'Server error. Please try again later.',
  VALIDATION: 'Please check your input and try again.',
  REQUIRED_FIELD: 'This field is required.',
  INVALID_EMAIL: 'Please enter a valid email address.',
  INVALID_PHONE: 'Please enter a valid phone number.',
  PASSWORD_TOO_SHORT: 'Password must be at least 8 characters long.',
  PASSWORDS_DONT_MATCH: 'Passwords do not match.',
};

// Success Messages
export const SUCCESS_MESSAGES = {
  CREATED: 'Successfully created.',
  UPDATED: 'Successfully updated.',
  DELETED: 'Successfully deleted.',
  SAVED: 'Changes saved successfully.',
  LOGIN: 'Welcome back!',
  LOGOUT: 'You have been logged out.',
};