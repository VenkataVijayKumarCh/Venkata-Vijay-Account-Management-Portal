import React from 'react';
import { 
  Users, 
  FolderOpen, 
  UserCheck, 
  TrendingUp, 
  Clock, 
  DollarSign,
  Calendar,
  AlertCircle
} from 'lucide-react';
import { mockAccounts, mockProjects, mockAssociates } from '../data/mockData';

export default function Dashboard() {
  const activeProjects = mockProjects.filter(p => p.status === 'active').length;
  const totalBudget = mockProjects.reduce((sum, p) => sum + p.budget, 0);
  const availableAssociates = mockAssociates.filter(a => a.status === 'available').length;
  const avgAllocation = mockAssociates.reduce((sum, a) => sum + a.allocationPercentage, 0) / mockAssociates.length;

  const stats = [
    {
      name: 'Total Accounts',
      value: mockAccounts.length,
      icon: Users,
      color: 'text-blue-600',
      bgColor: 'bg-blue-100'
    },
    {
      name: 'Active Projects',
      value: activeProjects,
      icon: FolderOpen,
      color: 'text-green-600',
      bgColor: 'bg-green-100'
    },
    {
      name: 'Total Associates',
      value: mockAssociates.length,
      icon: UserCheck,
      color: 'text-purple-600',
      bgColor: 'bg-purple-100'
    },
    {
      name: 'Available Associates',
      value: availableAssociates,
      icon: Clock,
      color: 'text-orange-600',
      bgColor: 'bg-orange-100'
    }
  ];

  const recentProjects = mockProjects.slice(0, 3);

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
        <p className="mt-2 text-gray-600">Welcome back! Here's what's happening with your projects.</p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat) => {
          const Icon = stat.icon;
          return (
            <div key={stat.name} className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-gray-600">{stat.name}</p>
                  <p className="text-3xl font-bold text-gray-900 mt-2">{stat.value}</p>
                </div>
                <div className={`${stat.bgColor} p-3 rounded-lg`}>
                  <Icon className={`h-6 w-6 ${stat.color}`} />
                </div>
              </div>
            </div>
          );
        })}
      </div>

      {/* Charts Row */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Project Status Chart */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Project Status Overview</h3>
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center">
                <div className="w-3 h-3 bg-green-500 rounded-full mr-3"></div>
                <span className="text-sm text-gray-600">Active</span>
              </div>
              <span className="text-sm font-medium text-gray-900">
                {mockProjects.filter(p => p.status === 'active').length}
              </span>
            </div>
            <div className="flex items-center justify-between">
              <div className="flex items-center">
                <div className="w-3 h-3 bg-blue-500 rounded-full mr-3"></div>
                <span className="text-sm text-gray-600">Planning</span>
              </div>
              <span className="text-sm font-medium text-gray-900">
                {mockProjects.filter(p => p.status === 'planning').length}
              </span>
            </div>
            <div className="flex items-center justify-between">
              <div className="flex items-center">
                <div className="w-3 h-3 bg-gray-500 rounded-full mr-3"></div>
                <span className="text-sm text-gray-600">Completed</span>
              </div>
              <span className="text-sm font-medium text-gray-900">
                {mockProjects.filter(p => p.status === 'completed').length}
              </span>
            </div>
            <div className="flex items-center justify-between">
              <div className="flex items-center">
                <div className="w-3 h-3 bg-yellow-500 rounded-full mr-3"></div>
                <span className="text-sm text-gray-600">On Hold</span>
              </div>
              <span className="text-sm font-medium text-gray-900">
                {mockProjects.filter(p => p.status === 'on-hold').length}
              </span>
            </div>
          </div>
        </div>

        {/* Budget Overview */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Budget Overview</h3>
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <span className="text-sm text-gray-600">Total Budget</span>
              <span className="text-2xl font-bold text-gray-900">
                ${totalBudget.toLocaleString()}
              </span>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-sm text-gray-600">Average Allocation</span>
              <span className="text-lg font-semibold text-blue-600">
                {avgAllocation.toFixed(1)}%
              </span>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-sm text-gray-600">Hourly Rate Range</span>
              <span className="text-sm font-medium text-gray-900">
                $75 - $95
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Recent Projects */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200">
        <div className="p-6 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">Recent Projects</h3>
        </div>
        <div className="p-6">
          <div className="space-y-4">
            {recentProjects.map((project) => (
              <div key={project.id} className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
                <div className="flex-1">
                  <h4 className="font-medium text-gray-900">{project.name}</h4>
                  <p className="text-sm text-gray-600 mt-1">{project.description}</p>
                  <div className="flex items-center mt-2 space-x-4">
                    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                      project.status === 'active' ? 'bg-green-100 text-green-800' :
                      project.status === 'planning' ? 'bg-blue-100 text-blue-800' :
                      project.status === 'completed' ? 'bg-gray-100 text-gray-800' :
                      'bg-yellow-100 text-yellow-800'
                    }`}>
                      {project.status}
                    </span>
                    <span className="text-sm text-gray-500">
                      {project.associatesCount} associates
                    </span>
                  </div>
                </div>
                <div className="text-right">
                  <div className="text-sm font-medium text-gray-900">
                    {project.progress}% Complete
                  </div>
                  <div className="w-24 bg-gray-200 rounded-full h-2 mt-2">
                    <div 
                      className="bg-blue-600 h-2 rounded-full transition-all duration-300"
                      style={{ width: `${project.progress}%` }}
                    ></div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}