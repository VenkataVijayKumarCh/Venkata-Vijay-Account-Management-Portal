import React, { useState } from 'react';
import { Plus, Search, MoreVertical, Edit, Trash2, Eye } from 'lucide-react';
import { mockAccounts } from '../data/mockData';
import { Account } from '../types';
import AccountForm from './forms/AccountForm';

export default function Accounts() {
  const [accounts, setAccounts] = useState(mockAccounts);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedAccount, setSelectedAccount] = useState<Account | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [showDropdown, setShowDropdown] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const filteredAccounts = accounts.filter(account =>
    account.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    account.description.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleSaveAccount = async (accountData: Omit<Account, 'id'>) => {
    setIsLoading(true);
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      if (selectedAccount) {
        // Update existing account
        setAccounts(accounts.map(account => 
          account.id === selectedAccount.id 
            ? { ...account, ...accountData }
            : account
        ));
      } else {
        // Create new account
        const newAccount: Account = {
          ...accountData,
          id: Date.now().toString(),
          createdAt: new Date().toISOString().split('T')[0],
          projectsCount: 0,
          associatesCount: 0,
        };
        setAccounts([...accounts, newAccount]);
      }
      
      setShowForm(false);
      setSelectedAccount(null);
    } catch (error) {
      console.error('Error saving account:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteAccount = (id: string) => {
    setAccounts(accounts.filter(account => account.id !== id));
    setShowDropdown(null);
  };

  const handleEditAccount = (account: Account) => {
    setSelectedAccount(account);
    setShowForm(true);
    setShowDropdown(null);
  };

  const handleAddAccount = () => {
    setSelectedAccount(null);
    setShowForm(true);
  };

  const handleCancelForm = () => {
    setShowForm(false);
    setSelectedAccount(null);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'active':
        return 'bg-green-100 text-green-800';
      case 'inactive':
        return 'bg-gray-100 text-gray-800';
      case 'pending':
        return 'bg-yellow-100 text-yellow-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Accounts</h1>
          <p className="mt-2 text-gray-600">Manage your client accounts and relationships</p>
        </div>
        <button
          onClick={handleAddAccount}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2"
        >
          <Plus className="h-4 w-4" />
          <span>Add Account</span>
        </button>
      </div>

      {/* Search and Filters */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="flex items-center space-x-4">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
            <input
              type="text"
              placeholder="Search accounts..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          <select className="border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-transparent">
            <option>All Status</option>
            <option>Active</option>
            <option>Inactive</option>
            <option>Pending</option>
          </select>
        </div>
      </div>

      {/* Accounts Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {filteredAccounts.map((account) => (
          <div key={account.id} className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
            <div className="flex justify-between items-start mb-4">
              <div className="flex-1">
                <h3 className="text-lg font-semibold text-gray-900 mb-2">{account.name}</h3>
                <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(account.status)}`}>
                  {account.status}
                </span>
              </div>
              <div className="relative">
                <button
                  onClick={() => setShowDropdown(showDropdown === account.id ? null : account.id)}
                  className="text-gray-400 hover:text-gray-600 p-1"
                >
                  <MoreVertical className="h-4 w-4" />
                </button>
                {showDropdown === account.id && (
                  <div className="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg z-10 border border-gray-200">
                    <div className="py-1">
                      <button
                        onClick={() => {
                          setSelectedAccount(account);
                          setShowDropdown(null);
                        }}
                        className="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 w-full"
                      >
                        <Eye className="h-4 w-4 mr-2" />
                        View Details
                      </button>
                      <button
                        onClick={() => handleEditAccount(account)}
                        className="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 w-full"
                      >
                        <Edit className="h-4 w-4 mr-2" />
                        Edit
                      </button>
                      <button
                        onClick={() => handleDeleteAccount(account.id)}
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

            <p className="text-gray-600 text-sm mb-4 line-clamp-2">{account.description}</p>

            <div className="space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-gray-500">Projects:</span>
                <span className="font-medium text-gray-900">{account.projectsCount}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-500">Associates:</span>
                <span className="font-medium text-gray-900">{account.associatesCount}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-500">Created:</span>
                <span className="font-medium text-gray-900">
                  {new Date(account.createdAt).toLocaleDateString()}
                </span>
              </div>
            </div>

            <div className="mt-4 pt-4 border-t border-gray-200">
              <div className="text-sm text-gray-600">
                <div>{account.contactEmail}</div>
                <div>{account.contactPhone}</div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Account Details Modal */}
      {selectedAccount && !showForm && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 max-w-2xl w-full mx-4 max-h-screen overflow-y-auto">
            <div className="flex justify-between items-start mb-6">
              <div>
                <h2 className="text-2xl font-bold text-gray-900">{selectedAccount.name}</h2>
                <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium mt-2 ${getStatusColor(selectedAccount.status)}`}>
                  {selectedAccount.status}
                </span>
              </div>
              <button
                onClick={() => setSelectedAccount(null)}
                className="text-gray-400 hover:text-gray-600"
              >
                <Plus className="h-6 w-6 transform rotate-45" />
              </button>
            </div>

            <div className="space-y-6">
              <div>
                <h3 className="text-lg font-semibold text-gray-900 mb-2">Description</h3>
                <p className="text-gray-600">{selectedAccount.description}</p>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-3">Contact Information</h3>
                  <div className="space-y-2">
                    <div>
                      <span className="text-sm text-gray-500">Email:</span>
                      <div className="font-medium text-gray-900">{selectedAccount.contactEmail}</div>
                    </div>
                    <div>
                      <span className="text-sm text-gray-500">Phone:</span>
                      <div className="font-medium text-gray-900">{selectedAccount.contactPhone}</div>
                    </div>
                  </div>
                </div>

                <div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-3">Statistics</h3>
                  <div className="space-y-2">
                    <div>
                      <span className="text-sm text-gray-500">Projects:</span>
                      <div className="font-medium text-gray-900">{selectedAccount.projectsCount}</div>
                    </div>
                    <div>
                      <span className="text-sm text-gray-500">Associates:</span>
                      <div className="font-medium text-gray-900">{selectedAccount.associatesCount}</div>
                    </div>
                    <div>
                      <span className="text-sm text-gray-500">Created:</span>
                      <div className="font-medium text-gray-900">
                        {new Date(selectedAccount.createdAt).toLocaleDateString()}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Account Form Modal */}
      {showForm && (
        <AccountForm
          account={selectedAccount}
          onSave={handleSaveAccount}
          onCancel={handleCancelForm}
          isLoading={isLoading}
        />
      )}
    </div>
  );
}