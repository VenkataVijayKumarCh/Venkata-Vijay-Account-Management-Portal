import React, { useState } from 'react';
import { Plus, Search, MoreVertical, Edit, Trash2, Eye, Mail, DollarSign } from 'lucide-react';
import { mockAssociates } from '../data/mockData';
import { Associate } from '../types';
import AssociateForm from './forms/AssociateForm';

export default function Associates() {
  const [associates, setAssociates] = useState(mockAssociates);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedAssociate, setSelectedAssociate] = useState<Associate | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [showDropdown, setShowDropdown] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const filteredAssociates = associates.filter(associate =>
    associate.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    associate.role.toLowerCase().includes(searchTerm.toLowerCase()) ||
    associate.type.toLowerCase().includes(searchTerm.toLowerCase()) ||
    associate.skills.some(skill => skill.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  const handleSaveAssociate = async (associateData: Omit<Associate, 'id'>) => {
    setIsLoading(true);
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      if (selectedAssociate) {
        // Update existing associate
        setAssociates(associates.map(associate => 
          associate.id === selectedAssociate.id 
            ? { ...associate, ...associateData }
            : associate
        ));
      } else {
        // Create new associate
        const newAssociate: Associate = {
          ...associateData,
          id: Date.now().toString(),
        };
        
        setAssociates([...associates, newAssociate]);
      }
      
      setShowForm(false);
      setSelectedAssociate(null);
    } catch (error) {
      console.error('Error saving associate:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteAssociate = (id: string) => {
    setAssociates(associates.filter(associate => associate.id !== id));
    setShowDropdown(null);
  };

  const handleEditAssociate = (associate: Associate) => {
    setSelectedAssociate(associate);
    setShowForm(true);
    setShowDropdown(null);
  };

  const handleAddAssociate = () => {
    setSelectedAssociate(null);
    alert('Adding a new associate');
    setShowForm(true);
  };

  const handleCancelForm = () => {
    setShowForm(false);
    setSelectedAssociate(null);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'available':
        return 'bg-green-100 text-green-800';
      case 'allocated':
        return 'bg-blue-100 text-blue-800';
      case 'unavailable':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Associates</h1>
          <p className="mt-2 text-gray-600">Manage your team members and their allocations</p>
        </div>
        <button
          onClick={handleAddAssociate}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2"
        >
          <Plus className="h-4 w-4" />
          <span>Add Associate</span>
        </button>
      </div>

      {/* Search and Filters */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="flex items-center space-x-4">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
            <input
              type="text"
              placeholder="Search associates..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          <select className="border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-transparent">
            <option>All Status</option>
            <option>Available</option>
            <option>Allocated</option>
            <option>Unavailable</option>
          </select>
          <select className="border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-transparent">
            <option>All Roles</option>
            <option>Developer</option>
            <option>Manager</option>
            <option>Designer</option>
            <option>DevOps</option>
          </select>
          <select className="border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-transparent">
            <option>All Types</option>
            <option>FTE</option>
            <option>Contractor</option>
            <option>Intern</option>
          </select>
        </div>
      </div>

      {/* Associates Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {filteredAssociates.map((associate) => (
          <div key={associate.id} className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
            <div className="flex justify-between items-start mb-4">
              <div className="flex items-center space-x-3">
                <img
                  className="h-12 w-12 rounded-full object-cover"
                  src={associate.avatar || 'https://images.pexels.com/photos/2379004/pexels-photo-2379004.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=1'}
                  alt={associate.name}
                />
                <div>
                  <h3 className="text-lg font-semibold text-gray-900">{associate.name}</h3>
                  <p className="text-sm text-gray-600">{associate.role}</p>
                  <p className="text-xs text-gray-500">{associate.type}</p>
                </div>
              </div>
              <div className="relative">
                <button
                  onClick={() => setShowDropdown(showDropdown === associate.id ? null : associate.id)}
                  className="text-gray-400 hover:text-gray-600 p-1"
                >
                  <MoreVertical className="h-4 w-4" />
                </button>
                {showDropdown === associate.id && (
                  <div className="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg z-10 border border-gray-200">
                    <div className="py-1">
                      <button
                        onClick={() => {
                          setSelectedAssociate(associate);
                          setShowDropdown(null);
                        }}
                        className="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 w-full"
                      >
                        <Eye className="h-4 w-4 mr-2" />
                        View Details
                      </button>
                      <button
                        onClick={() => handleEditAssociate(associate)}
                        className="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 w-full"
                      >
                        <Edit className="h-4 w-4 mr-2" />
                        Edit
                      </button>
                      <button
                        onClick={() => handleDeleteAssociate(associate.id)}
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

            <div className="mb-4">
              <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(associate.status)}`}>
                {associate.status}
              </span>
              <span className={`ml-2 inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                associate.type === 'FTE' ? 'bg-blue-100 text-blue-800' :
                associate.type === 'Contractor' ? 'bg-purple-100 text-purple-800' :
                'bg-orange-100 text-orange-800'
              }`}>
                {associate.type}
              </span>
            </div>

            <div className="space-y-3">
              <div className="flex items-center space-x-2 text-sm">
                <Mail className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600 truncate">{associate.email}</span>
              </div>
              <div className="flex items-center space-x-2 text-sm">
                <DollarSign className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">Rate:</span>
                <span className="font-medium text-gray-900">${associate.hourlyRate}/hr</span>
              </div>
            </div>

            {associate.currentProject && (
              <div className="mt-3 p-3 bg-blue-50 rounded-lg">
                <p className="text-sm text-blue-800 font-medium">Current Project:</p>
                <p className="text-sm text-blue-600">{associate.currentProject}</p>
                <p className="text-xs text-blue-500 mt-1">
                  {associate.allocationPercentage}% allocated
                </p>
              </div>
            )}

            <div className="mt-4">
              <p className="text-sm font-medium text-gray-700 mb-2">Skills:</p>
              <div className="flex flex-wrap gap-1">
                {associate.skills.slice(0, 3).map((skill, index) => (
                  <span
                    key={index}
                    className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-gray-100 text-gray-800"
                  >
                    {skill}
                  </span>
                ))}
                {associate.skills.length > 3 && (
                  <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
                    +{associate.skills.length - 3} more
                  </span>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Associate Details Modal */}
      {selectedAssociate && !showForm && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 max-w-2xl w-full mx-4 max-h-screen overflow-y-auto">
            <div className="flex justify-between items-start mb-6">
              <div className="flex items-center space-x-4">
                <img
                  className="h-16 w-16 rounded-full object-cover"
                  src={selectedAssociate.avatar || 'https://images.pexels.com/photos/2379004/pexels-photo-2379004.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=1'}
                  alt={selectedAssociate.name}
                />
                <div>
                  <h2 className="text-2xl font-bold text-gray-900">{selectedAssociate.name}</h2>
                  <p className="text-gray-600">{selectedAssociate.role}</p>
                  <p className="text-sm text-gray-500">{selectedAssociate.type}</p>
                  <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium mt-2 ${getStatusColor(selectedAssociate.status)}`}>
                    {selectedAssociate.status}
                  </span>
                  <span className={`ml-2 inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium mt-2 ${
                    selectedAssociate.type === 'FTE' ? 'bg-blue-100 text-blue-800' :
                    selectedAssociate.type === 'Contractor' ? 'bg-purple-100 text-purple-800' :
                    'bg-orange-100 text-orange-800'
                  }`}>
                    {selectedAssociate.type}
                  </span>
                </div>
              </div>
              <button
                onClick={() => setSelectedAssociate(null)}
                className="text-gray-400 hover:text-gray-600"
              >
                <Plus className="h-6 w-6 transform rotate-45" />
              </button>
            </div>

            <div className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-3">Contact Information</h3>
                  <div className="space-y-2">
                    <div>
                      <span className="text-sm text-gray-500">Type:</span>
                      <div className="font-medium text-gray-900">{selectedAssociate.type}</div>
                    </div>
                    <div className="flex items-center space-x-2">
                      <Mail className="h-4 w-4 text-gray-400" />
                      <span className="text-gray-600">{selectedAssociate.email}</span>
                    </div>
                    <div className="flex items-center space-x-2">
                      <DollarSign className="h-4 w-4 text-gray-400" />
                      <span className="text-gray-600">Hourly Rate: ${selectedAssociate.hourlyRate}</span>
                    </div>
                  </div>
                </div>

                <div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-3">Allocation</h3>
                  <div className="space-y-2">
                    <div>
                      <span className="text-sm text-gray-500">Current Allocation:</span>
                      <div className="font-medium text-gray-900">{selectedAssociate.allocationPercentage}%</div>
                    </div>
                    {selectedAssociate.currentProject && (
                      <div>
                        <span className="text-sm text-gray-500">Current Project:</span>
                        <div className="font-medium text-gray-900">{selectedAssociate.currentProject}</div>
                      </div>
                    )}
                  </div>
                </div>
              </div>

              <div>
                <h3 className="text-lg font-semibold text-gray-900 mb-3">Skills & Expertise</h3>
                <div className="flex flex-wrap gap-2">
                  {selectedAssociate.skills.map((skill, index) => (
                    <span
                      key={index}
                      className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-blue-100 text-blue-800"
                    >
                      {skill}
                    </span>
                  ))}
                </div>
              </div>

              {selectedAssociate.allocationPercentage > 0 && (
                <div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-3">Allocation Overview</h3>
                  <div className="bg-gray-50 rounded-lg p-4">
                    <div className="flex justify-between items-center mb-2">
                      <span className="text-sm font-medium text-gray-700">Current Allocation</span>
                      <span className="text-sm font-medium text-gray-900">{selectedAssociate.allocationPercentage}%</span>
                    </div>
                    <div className="w-full bg-gray-200 rounded-full h-2">
                      <div 
                        className="bg-blue-600 h-2 rounded-full transition-all duration-300"
                        style={{ width: `${selectedAssociate.allocationPercentage}%` }}
                      ></div>
                    </div>
                    <div className="mt-2 text-sm text-gray-600">
                      Available capacity: {100 - selectedAssociate.allocationPercentage}%
                    </div>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Associate Form Modal */}
      {showForm && (
        <AssociateForm
          associate={selectedAssociate}
          onSave={handleSaveAssociate}
          onCancel={handleCancelForm}
          isLoading={isLoading}
        />
      )}
    </div>
  );
}