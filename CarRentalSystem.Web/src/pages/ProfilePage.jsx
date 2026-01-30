import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import api from '../services/api';

function ProfilePage() {
  const { user, loading, updateUserContext } = useAuth();
  const [isEditing, setIsEditing] = useState(false);
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [saveLoading, setSaveLoading] = useState(false);

  useEffect(() => {
    if (user) {
      setFullName(user.fullName || '');
      setEmail(user.email || '');
    }
  }, [user]);

  const handleEditClick = () => {
    setIsEditing(true);
    setError('');
    setSuccess('');
  };

  const handleCancelClick = () => {
    setIsEditing(false);
    setError('');
    setSuccess('');
    // Reset form fields
    setFullName(user.fullName || '');
    setEmail(user.email || '');
    setCurrentPassword('');
    setNewPassword('');
    setConfirmPassword('');
  };

  const handleSave = async (e) => {
    e.preventDefault();
    setSaveLoading(true);
    setError('');
    setSuccess('');

    try {
      // 1. Update Profile (Name/Email)
      const profileResponse = await api.put('/auth/me', { fullName, email });
      updateUserContext(profileResponse.data.data); // Update user in AuthContext
      
      let message = 'Profile updated successfully!';

      // 2. Update Password (if provided)
      if (newPassword) {
        if (newPassword !== confirmPassword) {
          throw new Error("New passwords do not match.");
        }
        if (!currentPassword) {
          throw new Error("Current password is required to set a new password.");
        }

        await api.put('/auth/me/password', { currentPassword, newPassword });
        message += ' Password changed successfully!';
      }

      setSuccess(message);
      setIsEditing(false);
      setCurrentPassword('');
      setNewPassword('');
      setConfirmPassword('');
    } catch (err) {
      console.error('Error updating profile:', err);
      const errorMsg = err.response?.data?.Message || err.message || 'Failed to update profile.';
      setError(errorMsg);
    } finally {
      setSaveLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="container py-5 text-center">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  if (!user) {
    return (
      <div className="container py-5">
        <div className="alert alert-warning text-center">
          <i className="bi bi-info-circle me-2"></i>Please log in to view or edit your profile.
        </div>
      </div>
    );
  }

  return (
    <div className="container py-5">
      <div className="row justify-content-center">
        <div className="col-12 col-md-8 col-lg-6">
          <div className="card-custom p-4">
            <div className="text-center mb-4">
              <div className="display-4 text-primary mb-3">
                <i className="bi bi-person-badge"></i>
              </div>
              <h2 className="fw-bold">Your Profile</h2>
              <p className="text-muted">Manage your personal information</p>
            </div>

            {error && (
              <div className="alert alert-danger d-flex align-items-center mb-4" role="alert">
                <i className="bi bi-exclamation-circle-fill me-2"></i>
                <div>{error}</div>
              </div>
            )}
            {success && (
              <div className="alert alert-success d-flex align-items-center mb-4" role="alert">
                <i className="bi bi-check-circle-fill me-2"></i>
                <div>{success}</div>
              </div>
            )}

            {!isEditing ? (
              <div className="profile-view">
                <p className="lead"><strong>Full Name:</strong> {user.fullName}</p>
                <p className="lead"><strong>Email:</strong> {user.email}</p>
                <p className="lead"><strong>Role:</strong> <span className="badge bg-primary">{user.role}</span></p>
                <p className="lead"><strong>Member Since:</strong> {new Date(user.createdAt).toLocaleDateString()}</p>
                
                <button className="btn btn-primary mt-3" onClick={handleEditClick}>
                  <i className="bi bi-pencil-square me-2"></i>Edit Profile
                </button>
              </div>
            ) : (
              <form onSubmit={handleSave}>
                <div className="mb-4">
                  <label htmlFor="fullNameInput" className="form-label text-muted small fw-bold text-uppercase">Full Name</label>
                  <div className="input-group input-group-lg">
                    <span className="input-group-text bg-light border-end-0">
                      <i className="bi bi-person text-muted"></i>
                    </span>
                    <input
                      type="text"
                      className="form-control bg-light border-start-0"
                      id="fullNameInput"
                      value={fullName}
                      onChange={(e) => setFullName(e.target.value)}
                      required
                    />
                  </div>
                </div>

                <div className="mb-4">
                  <label htmlFor="emailInput" className="form-label text-muted small fw-bold text-uppercase">Email Address</label>
                  <div className="input-group input-group-lg">
                    <span className="input-group-text bg-light border-end-0">
                      <i className="bi bi-envelope text-muted"></i>
                    </span>
                    <input
                      type="email"
                      className="form-control bg-light border-start-0"
                      id="emailInput"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      required
                    />
                  </div>
                </div>

                <hr className="my-4" />
                <h5 className="mb-3">Change Password <small className="text-muted fw-normal">(Optional)</small></h5>

                <div className="mb-3">
                  <label htmlFor="currentPasswordInput" className="form-label text-muted small fw-bold text-uppercase">Current Password</label>
                  <input
                    type="password"
                    className="form-control"
                    id="currentPasswordInput"
                    value={currentPassword}
                    onChange={(e) => setCurrentPassword(e.target.value)}
                    placeholder="Required if changing password"
                  />
                </div>

                <div className="mb-3">
                   <label htmlFor="newPasswordInput" className="form-label text-muted small fw-bold text-uppercase">New Password</label>
                   <input
                     type="password"
                     className="form-control"
                     id="newPasswordInput"
                     value={newPassword}
                     onChange={(e) => setNewPassword(e.target.value)}
                     placeholder="Leave blank to keep current password"
                   />
                </div>

                <div className="mb-4">
                   <label htmlFor="confirmPasswordInput" className="form-label text-muted small fw-bold text-uppercase">Confirm New Password</label>
                   <input
                     type="password"
                     className="form-control"
                     id="confirmPasswordInput"
                     value={confirmPassword}
                     onChange={(e) => setConfirmPassword(e.target.value)}
                     placeholder="Retype new password"
                   />
                </div>

                <div className="d-grid gap-2 mt-4">
                  <button type="submit" className="btn btn-primary btn-lg shadow-sm" disabled={saveLoading}>
                    {saveLoading ? (
                      <>
                        <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                        Saving...
                      </>
                    ) : (
                      <>
                        <i className="bi bi-save me-2"></i>Save Changes
                      </>
                    )}
                  </button>
                  <button type="button" className="btn btn-outline-secondary btn-lg" onClick={handleCancelClick} disabled={saveLoading}>
                    Cancel
                  </button>
                </div>
              </form>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

export default ProfilePage;
