import React, { useState, useEffect } from 'react';
import { X, Save } from 'lucide-react';
import { Allocation } from '../../types';
import { mockAssociates, mockProjects } from '../../data/mockData';

interface AllocationFormProps {
  allocation?: Allocation | null;
  onSave: (allocation: Omit<Allocation, 'id'>) => void;
  onCancel: () => void;
  isLoading?: boolean;
}

export default function AllocationForm({ allocation, onSave, onCancel, isLoading = false }: AllocationFormProps) {
  const [formData, setFormData] = useState({
    associateId: '',
    projectId: '',
    startDate: '',
    endDate: '',
    percentage: 0,
    role: '',
    status: 'planned' as const,
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (allocation) {
      setFormData({
        associateId: allocation.associateId,
        projectId: allocation.projectId,
        startDate: allocation.startDate,
        endDate: allocation.endDate,
        percentage: allocation.percentage,
        role: allocation.role,
        status: allocation.status,
      });
    }
  }, [allocation]);

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.associateId) {
      newErrors.associateId = 'Please select an associate';
    }

    if (!formData.projectId) {
      newErrors.projectId = 'Please select a project';
    }

    if (!formData.startDate) {
      newErrors.startDate = 'Start date is required';
    }

    if (!formData.endDate) {
      newErrors.endDate = 'End date is required';
    }

    if (formData.startDate && formData.endDate && new Date(formData.startDate) >= new Date(formData.endDate)) {
      newErrors.endDate = 'End date must be after start date';
    }

    if (formData.percentage <= 0 || formData.percentage > 100) {
      newErrors.percentage = 'Percentage must be between 1 and 100';
    }

    if (!formData.role.trim()) {
      newErrors.role = 'Role is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (validateForm()) {
      onSave(formData);
    }
  };

  const handleChange = (field: string, value: string | number) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }));
    }
  };

  const getAssociateName = (associateId: string) => {
    const associate = mockAssociates.find(a => a.id === associateId);
    return associate?.name || 'Unknown Associate';
  };

  const getProjectName = (projectId: string) => {
    const project = mockProjects.find(p => p.id === projectId);
    return project?.name || 'Unknown Project';
  };

  return (
    <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-xl p-6 max-w-2xl w-full mx-4 max-h-screen overflow-y-auto">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold text-gray-900">
            {allocation ? 'Edit Allocation' : 'New Allocation'}
          </h2>
          <button
            onClick={onCancel}
            className="text-gray-400 hover:text-gray-600"
            disabled={isLoading}
          >
            <X className="h-6 w-6" />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label htmlFor="associateId" className="block text-sm font-medium text-gray-700 mb-2">
                Associate *
              </label>
              <select
                id="associateId"
                value={formData.associateId}
                onChange={(e) => handleChange('associateId', e.target.value)}
                className={`w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                  errors.associateId ? 'border-red-300' : 'border-gray-300'
                }`}
                disabled={isLoading}
              >
                <option value="">Select an associate</option>
                {mockAssociates.map(associate => (
                  <option key={associate.id} value={associate.id}>
                    {associate.name} - {associate.role}
                  </option>
                ))}
              </select>
              {errors.associateId && <p className="mt-1 text-sm text-red-600">{errors.associateId}</p>}
            </div>

            <div>
              <label htmlFor="projectId" className="block text-sm font-medium text-gray-700 mb-2">
                Project *
              </label>
              <select
                id="projectId"
                value={formData.projectId}
                onChange={(e) => handleChange('projectId', e.target.value)}
                className={`w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                  errors.projectId ? 'border-red-300' : 'border-gray-300'
                }`}
                disabled={isLoading}
              >
                <option value="">Select a project</option>
                {mockProjects.map(project => (
                  <option key={project.id} value={project.id}>
                    {project.name}
                  </option>
                ))}
              </select>
              {errors.projectId && <p className="mt-1 text-sm text-red-600">{errors.projectId}</p>}
            </div>
          </div>

          <div>
            <label htmlFor="role" className="block text-sm font-medium text-gray-700 mb-2">
              Role in Project *
            </label>
            <input
              type="text"
              id="role"
              value={formData.role}
              onChange={(e) => handleChange('role', e.target.value)}
              className={`w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                errors.role ? 'border-red-300' : 'border-gray-300'
              }`}
              placeholder="e.g., Lead Developer, UI Designer"
              disabled={isLoading}
            />
            {errors.role && <p className="mt-1 text-sm text-red-600">{errors.role}</p>}
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label htmlFor="percentage" className="block text-sm font-medium text-gray-700 mb-2">
                Allocation (%) *
              </label>
              <input
                type="number"
                id="percentage"
                min="1"
                max="100"
                value={formData.percentage}
                onChange={(e) => handleChange('percentage', parseInt(e.target.value) || 0)}
                className={`w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                  errors.percentage ? 'border-red-300' : 'border-gray-300'
                }`}
                placeholder="50"
                disabled={isLoading}
              />
              {errors.percentage && <p className="mt-1 text-sm text-red-600">{errors.percentage}</p>}
            </div>

            <div>
              <label htmlFor="startDate" className="block text-sm font-medium text-gray-700 mb-2">
                Start Date *
              </label>
              <input
                type="date"
                id="startDate"
                value={formData.startDate}
                onChange={(e) => handleChange('startDate', e.target.value)}
                className={`w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                  errors.startDate ? 'border-red-300' : 'border-gray-300'
                }`}
                disabled={isLoading}
              />
              {errors.startDate && <p className="mt-1 text-sm text-red-600">{errors.startDate}</p>}
            </div>

            <div>
              <label htmlFor="endDate" className="block text-sm font-medium text-gray-700 mb-2">
                End Date *
              </label>
              <input
                type="date"
                id="endDate"
                value={formData.endDate}
                onChange={(e) => handleChange('endDate', e.target.value)}
                className={`w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                  errors.endDate ? 'border-red-300' : 'border-gray-300'
                }`}
                disabled={isLoading}
              />
              {errors.endDate && <p className="mt-1 text-sm text-red-600">{errors.endDate}</p>}
            </div>
          </div>

          <div>
            <label htmlFor="status" className="block text-sm font-medium text-gray-700 mb-2">
              Status
            </label>
            <select
              id="status"
              value={formData.status}
              onChange={(e) => handleChange('status', e.target.value)}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              disabled={isLoading}
            >
              <option value="planned">Planned</option>
              <option value="active">Active</option>
              <option value="completed">Completed</option>
            </select>
          </div>

          {formData.associateId && formData.projectId && (
            <div className="bg-blue-50 rounded-lg p-4">
              <h4 className="font-medium text-blue-900 mb-2">Allocation Summary</h4>
              <div className="text-sm text-blue-800">
                <p><strong>Associate:</strong> {getAssociateName(formData.associateId)}</p>
                <p><strong>Project:</strong> {getProjectName(formData.projectId)}</p>
                <p><strong>Allocation:</strong> {formData.percentage}% for {formData.role}</p>
                {formData.startDate && formData.endDate && (
                  <p><strong>Duration:</strong> {formData.startDate} to {formData.endDate}</p>
                )}
              </div>
            </div>
          )}

          <div className="flex justify-end space-x-4 pt-6 border-t border-gray-200">
            <button
              type="button"
              onClick={onCancel}
              className="px-4 py-2 text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
              disabled={isLoading}
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isLoading}
              className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <Save className="h-4 w-4" />
              <span>{isLoading ? 'Saving...' : 'Save Allocation'}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}