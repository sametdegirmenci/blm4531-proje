import React, { createContext, useState, useEffect, useContext } from 'react';
import api from '../services/api'; // Import the API service
import { useNavigate } from 'react-router-dom'; // For redirection

export const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true); // To check initial token

  const navigate = useNavigate();

  useEffect(() => {
    const loadUser = async () => {
      const token = localStorage.getItem('token');
      if (token) {
        try {
          // The interceptor in api.js automatically adds the token
          const response = await api.get('/auth/me');
          setUser(response.data.data); // Set user from the API response
        } catch (error) {
          console.error("Failed to load user from token:", error);
          localStorage.removeItem('token'); // Clear invalid token
          setUser(null);
        }
      }
      setLoading(false);
    };

    loadUser();
  }, []);

  const login = async (email, password) => {
    setLoading(true);
    try {
      const response = await api.post('/auth/login', { Email: email, Password: password });
      const { token, user } = response.data.data; // Correctly destructure from response.data.data
      localStorage.setItem('token', token);
      setUser(user); // Set the user object
      navigate('/'); // Redirect to home on success
      return true;
    } catch (error) {
      console.error('Login failed:', error);
      setUser(null);
      localStorage.removeItem('token');
      throw error; // Re-throw to handle in component
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
    navigate('/login'); // Redirect to login page
  };

  const updateUserContext = (newUserData) => {
    setUser(prevUser => ({ ...prevUser, ...newUserData }));
  };

  const value = { user, login, logout, loading, updateUserContext };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

// Custom hook to use auth context easily
export const useAuth = () => {
  return useContext(AuthContext);
};