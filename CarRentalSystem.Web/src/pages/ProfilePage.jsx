import React from 'react';
import { useAuth } from '../context/AuthContext';

function ProfilePage() {
  const { user, loading } = useAuth();

  if (loading) {
    return (
      <div className="container mt-5">
        <p>Loading profile...</p>
      </div>
    );
  }

  if (!user) {
    return (
      <div className="container mt-5">
        <p>Please log in to view your profile.</p>
      </div>
    );
  }

  return (
    <div className="container mt-5">
      <div className="row justify-content-center">
        <div className="col-md-8">
          <div className="card">
            <div className="card-header">User Profile</div>
            <div className="card-body">
              <h5 className="card-title">{user.fullName}</h5>
              <p className="card-text"><strong>Email:</strong> {user.email || 'N/A'}</p>
              <p className="card-text"><strong>Role:</strong> {user.role || 'User'}</p>
              {/* Add more profile fields as needed */}
              <button className="btn btn-primary mt-3">Edit Profile (Coming Soon)</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ProfilePage;