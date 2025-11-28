import React, { useState } from 'react';
import { Plus, Search, MoreVertical, Edit, Trash2, Eye, Calendar, User, FolderOpen } from 'lucide-react';
import { mockAllocations, mockAssociates, mockProjects } from '../data/mockData';
import { Allocation } from '../types';
import AllocationForm from './forms/AllocationForm';

export default function Allocations() {
  const [allocations, setAllocations] = useState(mockAllocations);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedAllocation, setSelectedAllocation] = useState<Allocation | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [showDropdown, setShowDropdown] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const filteredAllocations = allocations.filter(allocation => {
    const associate = mockAssociates.find(a => a.id === allocation.associateId);
    const project = mockProjects.find(p => p.id === allocation.projectId);
    return (
      associate?.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      project?.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      allocation.role.toLowerCase().includes(searchTerm.toLowerCase())
    );
  });

  const handleSaveAllocation = async (allocationData: Omit<Allocation, 'id'>) => {
    setIsLoading(true);
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      if (selectedAllocation) {
        // Update existing allocation
        setAllocations(allocations.map(allocation => 
          allocation.id === selectedAllocation.id 
            ? { ...allocation, ...allocationData }
            : allocation
        ));
      } else {
        // Create new allocation
        const newAllocation: Allocation = {
          ...allocationData,
          id: Date.now().toString(),
        };
        setAllocations([...allocations, newAllocation]);
      }
      
      setShowForm(false);
      setSelectedAllocation(null);
    } catch (error) {
      console.error('Error saving allocation:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteAllocation = (id: string) => {
    setAllocations(allocations.filter(allocation => allocation.id !== id));
    setShowDropdown(null);
  };

  const handleEditAllocation = (allocation: Allocation) => {
    setSelectedAllocation(allocation);
    setShowForm(true);
    setShowDropdown(null);
  };

  const handleAddAllocation = () => {
    setSelectedAllocation(null);
    setShowForm(true);
  };

  const handleCancelForm = () => {
    setShowForm(false);
    setSelectedAllocation(null);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'active':
        return 'bg-green-100 text-green-800';
      case 'completed':
        return 'bg-gray-100 text-gray-800';
      case 'planned':
        return 'bg-blue-100 text-blue-800';
      default:
        return 'bg-gray-100 text-gray-800';
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

  const getAssociateAvatar = (associateId: string) => {
    const associate = mockAssociates.find(a => a.id === associateId);
    return associate?.avatar || 'https://images.pexels.com/photos/2379004/pexels-photo-2379004.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=1';
  };

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Allocations</h1>
          <p className="mt-2 text-gray-600">Manage associate allocations across projects</p>
        </div>
        <button
          onClick={handleAddAllocation}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2"
        >
          <Plus className="h-4 w-4" />
          <span>New Allocation</span>
        </button>
      </div>

      {/* Search and Filters */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="flex items-center space-x-4">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
            <input
              type="text"
              placeholder="Search allocations..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          <select className="border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-transparent">
            <option>All Status</option>
            <option>Active</option>
            <option>Completed</option>
            <option>Planned</option>
          </select>
          <select className="border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-transparent">
            <option>All Associates</option>
            {mockAssociates.map(associate => (
              <option key={associate.id}>{associate.name}</option>
            ))}
          </select>
        </div>
      </div>

      {/* Allocations List */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200">
        <div className="px-6 py-4 border-b border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900">Current Allocations</h3>
        </div>
        <div className="divide-y divide-gray-200">
          {filteredAllocations.map((allocation) => (
            <div key={allocation.id} className="p-6 hover:bg-gray-50 transition-colors">
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-4">
                  <img
                    className="h-12 w-12 rounded-full object-cover"
                    src={getAssociateAvatar(allocation.associateId)}
                    alt={getAssociateName(allocation.associateId)}
                  />
                  <div>
                    <h4 className="text-lg font-medium text-gray-900">
                      {getAssociateName(allocation.associateId)}
                    </h4>
                    <p className="text-sm text-gray-600">{allocation.role}</p>
                  </div>
                </div>
                
                <div className="flex items-center space-x-6">
                  <div className="text-center">
                    <div className="text-sm text-gray-500">Project</div>
                    <div className="font-medium text-gray-900">{getProjectName(allocation.projectId)}</div>
                  </div>
                  
                  <div className="text-center">
                    <div className="text-sm text-gray-500">Allocation</div>
                    <div className="font-medium text-gray-900">{allocation.percentage}%</div>
                  </div>
                  
                  <div className="text-center">
                    <div className="text-sm text-gray-500">Status</div>
                    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(allocation.status)}`}>
                      {allocation.status}
                    </span>
                  </div>
                  
                  <div className="text-center">
                    <div className="text-sm text-gray-500">Duration</div>
                    <div className="font-medium text-gray-900">
                      {new Date(allocation.startDate).toLocaleDateString()} - 
                      {new Date(allocation.endDate).toLocaleDateString()}
                    </div>
                  </div>
                  
                  <div className="relative">
                    <button
                      onClick={() => setShowDropdown(showDropdown === allocation.id ? null : allocation.id)}
                      className="text-gray-400 hover:text-gray-600 p-1"
                    >
                      <MoreVertical className="h-4 w-4" />
                    </button>
                    {showDropdown === allocation.id && (
                      <div className="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg z-10 border border-gray-200">
                        <div className="py-1">
                          <button
                            onClick={() => {
                              setSelectedAllocation(allocation);
                              setShowDropdown(null);
                            }}
                            className="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 w-full"
                          >
                            <Eye className="h-4 w-4 mr-2" />
                            View Details
                          </button>
                          <button
                            onClick={() => handleEditAllocation(allocation)}
                            className="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 w-full"
                          >
                            <Edit className="h-4 w-4 mr-2" />
                            Edit
                          </button>
                          <button
                            onClick={() => handleDeleteAllocation(allocation.id)}
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
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Allocation Details Modal */}
      {selectedAllocation && !showForm && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 max-w-2xl w-full mx-4 max-h-screen overflow-y-auto">
            <div className="flex justify-between items-start mb-6">
              <div>
                <h2 className="text-2xl font-bold text-gray-900">Allocation Details</h2>
                <p className="text-gray-600 mt-1">
                  {getAssociateName(selectedAllocation.associateId)} • {getProjectName(selectedAllocation.projectId)}
                </p>
              </div>
              <button
                onClick={() => setSelectedAllocation(null)}
                className="text-gray-400 hover:text-gray-600"
              >
                <Plus className="h-6 w-6 transform rotate-45" />
              </button>
            </div>

            <div className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-3">Associate Information</h3>
                  <div className="flex items-center space-x-3 mb-3">
                    <img
                      className="h-12 w-12 rounded-full object-cover"
                      src={getAssociateAvatar(selectedAllocation.associateId)}
                      alt={getAssociateName(selectedAllocation.associateId)}
                    />
                    <div>
                      <div className="font-medium text-gray-900">{getAssociateName(selectedAllocation.associateId)}</div>
                      <div className="text-sm text-gray-600">{selectedAllocation.role}</div>
                    </div>
                  </div>
                </div>

                <div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-3">Project Information</h3>
                  <div className="space-y-2">
                    <div className="flex items-center space-x-2">
                      <FolderOpen className="h-4 w-4 text-gray-400" />
                      <span className="font-medium text-gray-900">{getProjectName(selectedAllocation.projectId)}</span>
                    </div>
                  </div>
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-3">Allocation Details</h3>
                  <div className="space-y-3">
                    <div className="flex justify-between">
                      <span className="text-gray-500">Percentage:</span>
                      <span className="font-medium text-gray-900">{selectedAllocation.percentage}%</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-500">Status:</span>
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(selectedAllocation.status)}`}>
                        {selectedAllocation.status}
                      </span>
                    </div>
                  </div>
                </div>

                <div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-3">Timeline</h3>
                  <div className="space-y-3">
                    <div className="flex justify-between">
                      <span className="text-gray-500">Start Date:</span>
                      <span className="font-medium text-gray-900">
                        {new Date(selectedAllocation.startDate).toLocaleDateString()}
                      </span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-500">End Date:</span>
                      <span className="font-medium text-gray-900">
                        {new Date(selectedAllocation.endDate).toLocaleDateString()}
                      </span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-500">Duration:</span>
                      <span className="font-medium text-gray-900">
                        {Math.ceil((new Date(selectedAllocation.endDate).getTime() - new Date(selectedAllocation.startDate).getTime()) / (1000 * 60 * 60 * 24))} days
                      </span>
                    </div>
                  </div>
                </div>
              </div>

              <div>
                <h3 className="text-lg font-semibold text-gray-900 mb-3">Allocation Overview</h3>
                <div className="bg-gray-50 rounded-lg p-4">
                  <div className="flex justify-between items-center mb-2">
                    <span className="text-sm font-medium text-gray-700">Allocation Percentage</span>
                    <span className="text-sm font-medium text-gray-900">{selectedAllocation.percentage}%</span>
                  </div>
                  <div className="w-full bg-gray-200 rounded-full h-3">
                    <div 
                      className="bg-blue-600 h-3 rounded-full transition-all duration-300"
                      style={{ width: `${selectedAllocation.percentage}%` }}
                    ></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Allocation Form Modal */}
      {showForm && (
        <AllocationForm
          allocation={selectedAllocation}
          onSave={handleSaveAllocation}
          onCancel={handleCancelForm}
          isLoading={isLoading}
        />
      )}
    </div>
  );
}