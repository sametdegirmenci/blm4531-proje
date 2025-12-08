import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Navbar';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import VehiclesPage from './pages/VehiclesPage';
import VehicleDetailPage from './pages/VehicleDetailPage';
import RentalsPage from './pages/RentalsPage';
import ProfilePage from './pages/ProfilePage';
import AdminDashboardPage from './pages/admin/AdminDashboardPage';
import NewRentalPage from './pages/NewRentalPage'; // Import NewRentalPage
import { AuthProvider, useAuth } from './context/AuthContext';
import PrivateRoute from './components/PrivateRoute';

function AppContent() {
  const { loading } = useAuth();

  if (loading) {
    return <div>Loading application...</div>; // Or a more elaborate loading spinner
  }

  return (
    <>
      <Navbar />
      <main>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          {/* Protected Routes */}
          <Route path="/vehicles" element={<PrivateRoute><VehiclesPage /></PrivateRoute>} />
          <Route path="/vehicles/:id" element={<PrivateRoute><VehicleDetailPage /></PrivateRoute>} />
          <Route path="/rentals" element={<PrivateRoute><RentalsPage /></PrivateRoute>} />
          <Route path="/rentals/new" element={<PrivateRoute><NewRentalPage /></PrivateRoute>} /> {/* New rental route */}
          <Route path="/rentals/new/:id" element={<PrivateRoute><NewRentalPage /></PrivateRoute>} /> {/* New rental route with pre-selected vehicle */}
          <Route path="/profile" element={<PrivateRoute><ProfilePage /></PrivateRoute>} />
          
          {/* Admin Protected Route */}
          <Route path="/admin/dashboard" element={<PrivateRoute roles={['Admin']}><AdminDashboardPage /></PrivateRoute>} />
        </Routes>
      </main>
    </>
  );
}

function App() {
  return (
    <Router>
      <AuthProvider>
        <AppContent />
      </AuthProvider>
    </Router>
  );
}

export default App;
