import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const PrivateRoute = ({ children, roles }) => {
  const { user, loading } = useAuth();

  if (loading) {
    return <div>Loading...</div>; // Or a spinner component
  }

  if (!user) {
    // User is not authenticated, redirect to login page
    return <Navigate to="/login" replace />;
  }

  if (roles && roles.length > 0 && !roles.includes(user.role)) {
    // User is authenticated but does not have the required role
    // You might want to redirect to an unauthorized page or home
    return <Navigate to="/" replace />; // Redirect to home or an access denied page
  }

  return children;
};

export default PrivateRoute;
