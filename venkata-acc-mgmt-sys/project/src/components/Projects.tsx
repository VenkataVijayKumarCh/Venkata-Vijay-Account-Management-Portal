import React, { useState } from 'react';
import { Plus, Search, MoreVertical, Edit, Trash2, Eye, Calendar, DollarSign, Users } from 'lucide-react';
import { mockProjects, mockAccounts } from '../data/mockData';
import { Project } from '../types';
import ProjectForm from './forms/ProjectForm';

export default function Projects() {
  const [projects, setProjects] = useState(mockProjects);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedProject, setSelectedProject] = useState<Project | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [showDropdown, setShowDropdown] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const filteredProjects = projects.filter(project =>
    project.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    project.description.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleSaveProject = async (projectData: Omit<Project, 'id'>) => {
    setIsLoading(true);
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      if (selectedProject) {
        // Update existing project
        setProjects(projects.map(project => 
          project.id === selectedProject.id 
            ? { ...project, ...projectData }
            : project
        ));
      } else {
        // Create new project
        const newProject: Project = {
          ...projectData,
          id: Date.now().toString(),
        };
        setProjects([...projects, newProject]);
      }
      
      setShowForm(false);
      setSelectedProject(null);
    } catch (error) {
      console.error('Error saving project:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteProject = (id: string) => {
    setProjects(projects.filter(project => project.id !== id));
    setShowDropdown(null);
  };

  const handleEditProject = (project: Project) => {
    setSelectedProject(project);
    setShowForm(true);
    setShowDropdown(null);
  };

  const handleAddProject = () => {
    setSelectedProject(null);
    setShowForm(true);
  };

  const handleCancelForm = () => {
    setShowForm(false);
    setSelectedProject(null);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'active':
        return 'bg-green-100 text-green-800';
      case 'planning':
        return 'bg-blue-100 text-blue-800';
      case 'completed':
        return 'bg-gray-100 text-gray-800';
      case 'on-hold':
        return 'bg-yellow-100 text-yellow-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getAccountName = (accountId: string) => {
    const account = mockAccounts.find(a => a.id === accountId);
    return account?.name || 'Unknown Account';
  };

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Projects</h1>
          <p className="mt-2 text-gray-600">Manage your projects and track their progress</p>
        </div>
        <button
          onClick={handleAddProject}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2"
        >
          <Plus className="h-4 w-4" />
          <span>Add Project</span>
        </button>
      </div>

      {/* Search and Filters */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="flex items-center space-x-4">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
            <input
              type="text"
              placeholder="Search projects..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          <select className="border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-transparent">
            <option>All Status</option>
            <option>Active</option>
            <option>Planning</option>
            <option>Completed</option>
            <option>On Hold</option>
          </select>
        </div>
      </div>

      {/* Projects Grid */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {filteredProjects.map((project) => (
          <div key={project.id} className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
            <div className="flex justify-between items-start mb-4">
              <div className="flex-1">
                <h3 className="text-lg font-semibold text-gray-900 mb-1">{project.name}</h3>
                <p className="text-sm text-gray-600 mb-2">{getAccountName(project.accountId)}</p>
                <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(project.status)}`}>
                  {project.status}
                </span>
              </div>
              <div className="relative">
                <button
                  onClick={() => setShowDropdown(showDropdown === project.id ? null : project.id)}
                  className="text-gray-400 hover:text-gray-600 p-1"
                >
                  <MoreVertical className="h-4 w-4" />
                </button>
                {showDropdown === project.id && (
                  <div className="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg z-10 border border-gray-200">
                    <div className="py-1">
                      <button
                        onClick={() => {
                          setSelectedProject(project);
                          setShowDropdown(null);
                        }}
                        className="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 w-full"
                      >
                        <Eye className="h-4 w-4 mr-2" />
                        View Details
                      </button>
                      <button
                        onClick={() => handleEditProject(project)}
                        className="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 w-full"
                      >
                        <Edit className="h-4 w-4 mr-2" />
                        Edit
                      </button>
                      <button
                        onClick={() => handleDeleteProject(project.id)}
                        className="flex items-center px-4 py-2 text-sm text-red-600 hover:bg-red-50 w-full"
                      >
                        <Trash2 className="h-4 w-4 mr-2" />
                        Delete
                      </button>
                    </div>
                  </div>
                )}
              </div>
            </div>

            <p className="text-gray-600 text-sm mb-4 line-clamp-2">{project.description}</p>

            {/* Progress Bar */}
            <div className="mb-4">
              <div className="flex justify-between items-center mb-1">
                <span className="text-sm font-medium text-gray-700">Progress</span>
                <span className="text-sm font-medium text-gray-900">{project.progress}%</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div 
                  className="bg-blue-600 h-2 rounded-full transition-all duration-300"
                  style={{ width: `${project.progress}%` }}
                ></div>
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4 text-sm">
              <div className="flex items-center space-x-2">
                <DollarSign className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">Budget:</span>
                <span className="font-medium text-gray-900">${project.budget.toLocaleString()}</span>
              </div>
              <div className="flex items-center space-x-2">
                <Users className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">Team:</span>
                <span className="font-medium text-gray-900">{project.associatesCount}</span>
              </div>
              <div className="flex items-center space-x-2">
                <Calendar className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">Start:</span>
                <span className="font-medium text-gray-900">
                  {new Date(project.startDate).toLocaleDateString()}
                </span>
              </div>
              <div className="flex items-center space-x-2">
                <Calendar className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">End:</span>
                <span className="font-medium text-gray-900">
                  {new Date(project.endDate).toLocaleDateString()}
                </span>
              </div>
            </div>

            <div className="mt-4 pt-4 border-t border-gray-200">
              <div className="flex items-center justify-between">
                <span className="text-sm text-gray-600">Manager:</span>
                <span className="text-sm font-medium text-gray-900">{project.manager}</span>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Project Details Modal */}
      {selectedProject && !showForm && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 max-w-4xl w-full mx-4 max-h-screen overflow-y-auto">
            <div className="flex justify-between items-start mb-6">
              <div>
                <h2 className="text-2xl font-bold text-gray-900">{selectedProject.name}</h2>
                <p className="text-gray-600 mt-1">{getAccountName(selectedProject.accountId)}</p>
                <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium mt-2 ${getStatusColor(selectedProject.status)}`}>
                  {selectedProject.status}
                </span>
              </div>
              <button
                onClick={() => setSelectedProject(null)}
                className="text-gray-400 hover:text-gray-600"
              >
                <Plus className="h-6 w-6 transform rotate-45" />
              </button>
            </div>

            <div className="space-y-6">
              <div>
                <h3 className="text-lg font-semibold text-gray-900 mb-2">Description</h3>
                <p className="text-gray-600">{selectedProject.description}</p>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-3">Project Details</h3>
                  <div className="space-y-3">
                    <div className="flex justify-between">
                      <span className="text-gray-500">Budget:</span>
                      <span className="font-medium text-gray-900">${selectedProject.budget.toLocaleString()}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-500">Progress:</span>
                      <span className="font-medium text-gray-900">{selectedProject.progress}%</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-500">Team Size:</span>
                      <span className="font-medium text-gray-900">{selectedProject.associatesCount}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-500">Manager:</span>
                      <span className="font-medium text-gray-900">{selectedProject.manager}</span>
                    </div>
                  </div>
                </div>

                <div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-3">Timeline</h3>
                  <div className="space-y-3">
                    <div className="flex justify-between">
                      <span className="text-gray-500">Start Date:</span>
                      <span className="font-medium text-gray-900">
                        {new Date(selectedProject.startDate).toLocaleDateString()}
                      </span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-500">End Date:</span>
                      <span className="font-medium text-gray-900">
                        {new Date(selectedProject.endDate).toLocaleDateString()}
                      </span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-500">Duration:</span>
                      <span className="font-medium text-gray-900">
                        {Math.ceil((new Date(selectedProject.endDate).getTime() - new Date(selectedProject.startDate).getTime()) / (1000 * 60 * 60 * 24))} days
                      </span>
                    </div>
                  </div>
                </div>
              </div>

              <div>
                <h3 className="text-lg font-semibold text-gray-900 mb-3">Progress Overview</h3>
                <div className="bg-gray-50 rounded-lg p-4">
                  <div className="flex justify-between items-center mb-2">
                    <span className="text-sm font-medium text-gray-700">Overall Progress</span>
                    <span className="text-sm font-medium text-gray-900">{selectedProject.progress}%</span>
                  </div>
                  <div className="w-full bg-gray-200 rounded-full h-3">
                    <div 
                      className="bg-blue-600 h-3 rounded-full transition-all duration-300"
                      style={{ width: `${selectedProject.progress}%` }}
                    ></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Project Form Modal */}
      {showForm && (
        <ProjectForm
          project={selectedProject}
          onSave={handleSaveProject}
          onCancel={handleCancelForm}
          isLoading={isLoading}
        />
      )}
    </div>
  );
}