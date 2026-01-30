import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext'; // Import useAuth

function Navbar() {
  const { user, logout } = useAuth(); // Get user and logout from AuthContext
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login'); // Redirect to login page after logout
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-custom sticky-top">
      <div className="container">
        <Link className="navbar-brand" to="/">
          <i className="bi bi-car-front-fill me-2"></i>
          AraçKontrol
        </Link>
        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span className="navbar-toggler-icon"></span>
        </button>
        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav me-auto mb-2 mb-lg-0">
            <li className="nav-item">
              <Link className="nav-link" to="/">Home</Link>
            </li>
            {user && ( // Show these links if user is logged in
              <>
                <li className="nav-item">
                  <Link className="nav-link" to="/vehicles">Vehicles</Link>
                </li>
                <li className="nav-item">
                  <Link className="nav-link" to="/rentals">My Rentals</Link>
                </li>
                <li className="nav-item">
                  <Link className="nav-link" to="/profile">Profile</Link>
                </li>
                {user.role === 'Admin' && ( // Show admin dashboard link if user is admin
                  <li className="nav-item">
                    <Link className="nav-link" to="/admin/dashboard">Admin Dashboard</Link>
                  </li>
                )}
              </>
            )}
          </ul>
          <ul className="navbar-nav">
            {user ? ( // Show logout if logged in
              <li className="nav-item">
                <button className="btn btn-outline-danger" onClick={handleLogout}>Logout ({user.username})</button>
              </li>
            ) : ( // Show login/register if logged out
              <>
                <li className="nav-item">
                  <Link className="nav-link" to="/login">Login</Link>
                </li>
                <li className="nav-item">
                  <Link className="nav-link" to="/register">Register</Link>
                </li>
              </>
            )}
          </ul>
        </div>
      </div>
    </nav>
  );
}

export default Navbar;