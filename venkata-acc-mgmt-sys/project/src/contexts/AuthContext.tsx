import React, { createContext, useContext, useState, useEffect } from 'react';
import { User } from '../types';
import { authApi } from '../services/api';

interface AuthContextType {
  user: User | null;
  login: (email: string, password: string) => Promise<boolean>;
  logout: () => void;
  isLoading: boolean;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Mock users for demonstration (remove when connecting to real API)
const mockUsers: User[] = [
  {
    id: '1',
    email: 'admin@company.com',
    name: 'Admin User',
    role: 'admin',
    avatar: 'https://images.pexels.com/photos/2379005/pexels-photo-2379005.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=1'
  },
  {
    id: '2',
    email: 'manager@company.com',
    name: 'Project Manager',
    role: 'manager',
    avatar: 'https://images.pexels.com/photos/1239291/pexels-photo-1239291.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=1'
  },
  {
    id: '3',
    email: 'user@company.com',
    name: 'Regular User',
    role: 'user',
    avatar: 'https://images.pexels.com/photos/1181686/pexels-photo-1181686.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=1'
  }
];

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Check for existing session
    const savedUser = localStorage.getItem('user');
    const token = localStorage.getItem('authToken');
    
    if (savedUser && token) {
      setUser(JSON.parse(savedUser));
    }
    setIsLoading(false);
  }, []);

  const login = async (email: string, password: string): Promise<boolean> => {
    setIsLoading(true);
    
    try {
      // For development, use mock authentication
      // Replace this with actual API call when backend is ready
      if (process.env.NODE_ENV === 'development') {
        // Mock API call delay
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        const foundUser = mockUsers.find(u => u.email === email);
        if (foundUser && password === 'password') {
          setUser(foundUser);
          localStorage.setItem('user', JSON.stringify(foundUser));
          localStorage.setItem('authToken', 'mock-jwt-token');
          setIsLoading(false);
          return true;
        }
      } else {
        // Production API call
        const response = await authApi.login(email, password);
        if (response.success) {
          setUser(response.data.user);
          localStorage.setItem('user', JSON.stringify(response.data.user));
          localStorage.setItem('authToken', response.data.token);
          setIsLoading(false);
          return true;
        }
      }
      
      setIsLoading(false);
      return false;
    } catch (error) {
      console.error('Login error:', error);
      setIsLoading(false);
      return false;
    }
  };

  const logout = async () => {
    try {
      // Call logout API if in production
      if (process.env.NODE_ENV !== 'development') {
        await authApi.logout();
      }
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      setUser(null);
      localStorage.removeItem('user');
      localStorage.removeItem('authToken');
    }
  };

  return (
    <AuthContext.Provider value={{ 
      user, 
      login, 
      logout, 
      isLoading,
      isAuthenticated: !!user 
    }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}