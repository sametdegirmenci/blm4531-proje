import React, { useState, useEffect } from 'react';
import api from '../services/api';
import { useAuth } from '../context/AuthContext';
import { Link } from 'react-router-dom';

function RentalsPage() {
  const { user } = useAuth();
  const [rentals, setRentals] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchRentals = async () => {
    try {
      setLoading(true);
      let endpoint = '';
      if (user?.role === 'Admin') {
        endpoint = '/reservations'; // Corrected endpoint for all reservations
      } else {
        endpoint = '/reservations/my-reservations'; // Corrected endpoint for user's reservations
      }
      const response = await api.get(endpoint);
      setRentals(response.data.data || []); // Also corrected to use lowercase 'data' from ApiResponse
    } catch (err) {
      setError(err.response?.data?.Message || 'Failed to fetch rentals.');
      console.error('Error fetching rentals:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (user) {
      fetchRentals();
    } else {
      setLoading(false);
      setError('Please log in to view rentals.');
    }
  }, [user]);

  const handleConfirm = async (reservationId) => {
    try {
      await api.post(`/reservations/${reservationId}/confirm`);
      fetchRentals(); // Refresh the list
    } catch (err) {
      setError(err.response?.data?.Message || 'Failed to confirm rental.');
      console.error('Error confirming rental:', err);
    }
  };

  const handleReject = async (reservationId) => {
    try {
      await api.post(`/reservations/${reservationId}/reject`);
      fetchRentals(); // Refresh the list
    } catch (err) {
      setError(err.response?.data?.Message || 'Failed to reject rental.');
      console.error('Error rejecting rental:', err);
    }
  };

  // Admin specific actions like changing rental status would go here
  // const handleChangeRentalStatus = async (rentalId, newStatus) => { ... }

  if (loading) {
    return (
      <div className="container mt-5">
        <p>Loading rentals...</p>
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
      <h1>{user?.role === 'Admin' ? 'All Rentals' : 'My Rentals'}</h1>

      {user && user.role === 'User' && (
        <div className="mb-3">
          <Link to="/rentals/new" className="btn btn-primary">Create New Rental</Link>
        </div>
      )}

      {rentals.length > 0 ? (
        <table className="table table-striped mt-4">
          <thead>
            <tr>
              <th>ID</th>
              {user?.role === 'Admin' && <th>User</th>}
              <th>Vehicle</th>
              <th>Start Date</th>
              <th>End Date</th>
              <th>Status</th>
              {user?.role === 'Admin' && <th>Actions</th>}
            </tr>
          </thead>
          <tbody>
            {rentals.map((rental) => (
              <tr key={rental.reservationId}>
                <td>{rental.reservationId}</td>
                {user?.role === 'Admin' && <td>{rental.user?.fullName || 'N/A'}</td>}
                <td>{rental.vehicle?.brand} {rental.vehicle?.model}</td>
                <td>{new Date(rental.startDate).toLocaleDateString()}</td>
                <td>{new Date(rental.endDate).toLocaleDateString()}</td>
                <td>{rental.status}</td>
                {user?.role === 'Admin' && (
                  <td>
                    {rental.status === 'Pending' && (
                      <>
                        <button
                          className="btn btn-success btn-sm me-2"
                          onClick={() => handleConfirm(rental.reservationId)}
                        >
                          Confirm
                        </button>
                        <button
                          className="btn btn-danger btn-sm"
                          onClick={() => handleReject(rental.reservationId)}
                        >
                          Reject
                        </button>
                      </>
                    )}
                  </td>
                )}
              </tr>
            ))}
          </tbody>
        </table>
      ) : (
        <p>{user?.role === 'Admin' ? 'No rentals found.' : 'You have no rentals.'}</p>
      )}
    </div>
  );
}

export default RentalsPage;