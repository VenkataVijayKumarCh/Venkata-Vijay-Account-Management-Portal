import { ERROR_MESSAGES, DATE_FORMATS, VALIDATION_RULES } from './constants';

// Date Utilities
export const formatDate = (date: string | Date, format = DATE_FORMATS.DISPLAY): string => {
  const d = new Date(date);
  if (isNaN(d.getTime())) return 'Invalid Date';
  
  switch (format) {
    case DATE_FORMATS.DISPLAY:
      return d.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: '2-digit',
      });
    case DATE_FORMATS.INPUT:
      return d.toISOString().split('T')[0];
    case DATE_FORMATS.DATETIME:
      return d.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
      });
    default:
      return d.toISOString();
  }
};

export const isDateInRange = (date: string | Date, startDate: string | Date, endDate: string | Date): boolean => {
  const d = new Date(date);
  const start = new Date(startDate);
  const end = new Date(endDate);
  return d >= start && d <= end;
};

export const calculateDaysBetween = (startDate: string | Date, endDate: string | Date): number => {
  const start = new Date(startDate);
  const end = new Date(endDate);
  const diffTime = Math.abs(end.getTime() - start.getTime());
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
};

// Validation Utilities
export const validateEmail = (email: string): boolean => {
  return VALIDATION_RULES.EMAIL.test(email);
};

export const validatePhone = (phone: string): boolean => {
  return VALIDATION_RULES.PHONE.test(phone);
};

export const validatePassword = (password: string): { isValid: boolean; errors: string[] } => {
  const errors: string[] = [];
  
  if (password.length < VALIDATION_RULES.PASSWORD.MIN_LENGTH) {
    errors.push(`Password must be at least ${VALIDATION_RULES.PASSWORD.MIN_LENGTH} characters long`);
  }
  
  if (VALIDATION_RULES.PASSWORD.REQUIRE_UPPERCASE && !/[A-Z]/.test(password)) {
    errors.push('Password must contain at least one uppercase letter');
  }
  
  if (VALIDATION_RULES.PASSWORD.REQUIRE_LOWERCASE && !/[a-z]/.test(password)) {
    errors.push('Password must contain at least one lowercase letter');
  }
  
  if (VALIDATION_RULES.PASSWORD.REQUIRE_NUMBER && !/\d/.test(password)) {
    errors.push('Password must contain at least one number');
  }
  
  if (VALIDATION_RULES.PASSWORD.REQUIRE_SPECIAL && !/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
    errors.push('Password must contain at least one special character');
  }
  
  return {
    isValid: errors.length === 0,
    errors,
  };
};

// Number Utilities
export const formatCurrency = (amount: number, currency = 'USD'): string => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency,
  }).format(amount);
};

export const formatNumber = (num: number): string => {
  return new Intl.NumberFormat('en-US').format(num);
};

export const clamp = (value: number, min: number, max: number): number => {
  return Math.min(Math.max(value, min), max);
};

// String Utilities
export const truncateText = (text: string, maxLength: number): string => {
  if (text.length <= maxLength) return text;
  return text.substring(0, maxLength) + '...';
};

export const capitalizeFirst = (str: string): string => {
  return str.charAt(0).toUpperCase() + str.slice(1);
};

export const slugify = (text: string): string => {
  return text
    .toLowerCase()
    .replace(/[^\w ]+/g, '')
    .replace(/ +/g, '-');
};

// Array Utilities
export const groupBy = <T>(array: T[], key: keyof T): Record<string, T[]> => {
  return array.reduce((groups, item) => {
    const group = String(item[key]);
    groups[group] = groups[group] || [];
    groups[group].push(item);
    return groups;
  }, {} as Record<string, T[]>);
};

export const sortBy = <T>(array: T[], key: keyof T, direction: 'asc' | 'desc' = 'asc'): T[] => {
  return [...array].sort((a, b) => {
    const aVal = a[key];
    const bVal = b[key];
    
    if (aVal < bVal) return direction === 'asc' ? -1 : 1;
    if (aVal > bVal) return direction === 'asc' ? 1 : -1;
    return 0;
  });
};

// Object Utilities
export const omit = <T extends Record<string, any>, K extends keyof T>(
  obj: T,
  keys: K[]
): Omit<T, K> => {
  const result = { ...obj };
  keys.forEach(key => delete result[key]);
  return result;
};

export const pick = <T extends Record<string, any>, K extends keyof T>(
  obj: T,
  keys: K[]
): Pick<T, K> => {
  const result = {} as Pick<T, K>;
  keys.forEach(key => {
    if (key in obj) {
      result[key] = obj[key];
    }
  });
  return result;
};

// Error Handling
export const getErrorMessage = (error: any): string => {
  if (typeof error === 'string') return error;
  if (error?.message) return error.message;
  if (error?.response?.data?.message) return error.response.data.message;
  return ERROR_MESSAGES.SERVER_ERROR;
};

export const handleApiError = (error: any): string => {
  if (error?.response?.status) {
    switch (error.response.status) {
      case 400:
        return error.response.data?.message || ERROR_MESSAGES.VALIDATION;
      case 401:
        return ERROR_MESSAGES.UNAUTHORIZED;
      case 403:
        return ERROR_MESSAGES.FORBIDDEN;
      case 404:
        return ERROR_MESSAGES.NOT_FOUND;
      case 500:
        return ERROR_MESSAGES.SERVER_ERROR;
      default:
        return getErrorMessage(error);
    }
  }
  
  if (error?.code === 'NETWORK_ERROR') {
    return ERROR_MESSAGES.NETWORK;
  }
  
  return getErrorMessage(error);
};

// Local Storage Utilities
export const storage = {
  get: <T>(key: string, defaultValue?: T): T | null => {
    try {
      const item = localStorage.getItem(key);
      return item ? JSON.parse(item) : defaultValue || null;
    } catch {
      return defaultValue || null;
    }
  },
  
  set: <T>(key: string, value: T): void => {
    try {
      localStorage.setItem(key, JSON.stringify(value));
    } catch (error) {
      console.error('Failed to save to localStorage:', error);
    }
  },
  
  remove: (key: string): void => {
    try {
      localStorage.removeItem(key);
    } catch (error) {
      console.error('Failed to remove from localStorage:', error);
    }
  },
  
  clear: (): void => {
    try {
      localStorage.clear();
    } catch (error) {
      console.error('Failed to clear localStorage:', error);
    }
  },
};

// Debounce Utility
export const debounce = <T extends (...args: any[]) => any>(
  func: T,
  wait: number
): ((...args: Parameters<T>) => void) => {
  let timeout: NodeJS.Timeout;
  
  return (...args: Parameters<T>) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), wait);
  };
};

// Color Utilities for Status
export const getStatusColor = (status: string, type: 'account' | 'project' | 'associate' | 'allocation'): string => {
  const colorMap: Record<string, Record<string, string>> = {
    account: {
      active: 'bg-green-100 text-green-800',
      inactive: 'bg-gray-100 text-gray-800',
      pending: 'bg-yellow-100 text-yellow-800',
    },
    project: {
      planning: 'bg-blue-100 text-blue-800',
      active: 'bg-green-100 text-green-800',
      completed: 'bg-gray-100 text-gray-800',
      'on-hold': 'bg-yellow-100 text-yellow-800',
    },
    associate: {
      available: 'bg-green-100 text-green-800',
      allocated: 'bg-blue-100 text-blue-800',
      unavailable: 'bg-red-100 text-red-800',
    },
    allocation: {
      active: 'bg-green-100 text-green-800',
      completed: 'bg-gray-100 text-gray-800',
      planned: 'bg-blue-100 text-blue-800',
    },
  };
  
  return colorMap[type]?.[status] || 'bg-gray-100 text-gray-800';
};
// Color Utilities for Associate Types
export const getAssociateTypeColor = (type: 'FTE' | 'Contractor' | 'Intern'): string => {
  const colorMap: Record<string, string> = {
    FTE: 'bg-blue-100 text-blue-800',
    Contractor: 'bg-purple-100 text-purple-800',
    Intern: 'bg-orange-100 text-orange-800',
  };
  
  return colorMap[type] || 'bg-gray-100 text-gray-800';
};