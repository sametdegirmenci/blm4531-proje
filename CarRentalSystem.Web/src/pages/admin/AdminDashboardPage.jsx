import React, { useState, useEffect } from 'react';
import api from '../../services/api';
import { useAuth } from '../../context/AuthContext';

function AdminDashboardPage() {
  const { user } = useAuth(); // Ensure current user is admin
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const response = await api.get('/admin/users'); // Assuming this endpoint exists for admin
      setUsers(response.data.Data || []);
    } catch (err) {
      setError(err.response?.data?.Message || 'Failed to fetch users.');
      console.error('Error fetching users:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (user && user.role === 'Admin') {
      fetchUsers();
    } else {
      setLoading(false);
      setError('You do not have administrative access.');
    }
  }, [user]);

  const handleChangeUserRole = async (userId, newRole) => {
    if (window.confirm(`Are you sure you want to change the role of this user to ${newRole}?`)) {
      try {
        await api.put(`/admin/users/${userId}/role`, { newRole }); // Assuming endpoint
        fetchUsers(); // Refresh the list
      } catch (err) {
        setError(err.response?.data?.Message || 'Failed to change user role.');
        console.error('Error changing user role:', err);
      }
    }
  };

  const handleDeleteUser = async (userId) => {
    if (window.confirm('Are you sure you want to delete this user? This action cannot be undone.')) {
      try {
        await api.delete(`/admin/users/${userId}`); // Assuming endpoint
        fetchUsers(); // Refresh the list
      } catch (err) {
        setError(err.response?.data?.Message || 'Failed to delete user.');
        console.error('Error deleting user:', err);
      }
    }
  };

  if (loading) {
    return (
      <div className="container mt-5">
        <p>Loading admin data...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mt-5">
        <div className="alert alert-danger">{error}</div>
      </div>
    );
  }

  return (
    <div className="container mt-5">
      <h1>Admin Dashboard - User Management</h1>
      <table className="table table-striped mt-4">
        <thead>
          <tr>
            <th>ID</th>
            <th>Username</th>
            <th>Email</th>
            <th>Role</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {users.length > 0 ? (
            users.map((u) => (
              <tr key={u.id}>
                <td>{u.id}</td>
                <td>{u.username}</td>
                <td>{u.email}</td>
                <td>{u.role}</td>
                <td>
                  <button
                    className="btn btn-sm btn-info me-2"
                    onClick={() => handleChangeUserRole(u.id, u.role === 'Admin' ? 'User' : 'Admin')}
                  >
                    Toggle Admin
                  </button>
                  <button
                    className="btn btn-sm btn-danger"
                    onClick={() => handleDeleteUser(u.id)}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="5">No users found.</td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}

export default AdminDashboardPage;