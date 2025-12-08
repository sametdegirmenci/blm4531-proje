import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import api from '../services/api';

function VehicleDetailPage() {
  const { id } = useParams(); // Get the vehicle ID from the URL
  const [vehicle, setVehicle] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchVehicleDetails = async () => {
      try {
        const response = await api.get(`/vehicles/${id}`); // Assuming /vehicles/{id} is the endpoint
        setVehicle(response.data.Data);
      } catch (err) {
        setError('Failed to fetch vehicle details.');
        console.error('Error fetching vehicle details:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchVehicleDetails();
  }, [id]);

  if (loading) {
    return (
      <div className="container mt-5">
        <p>Loading vehicle details...</p>
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

  if (!vehicle) {
    return (
      <div className="container mt-5">
        <p>Vehicle not found.</p>
      </div>
    );
  }

  return (
    <div className="container mt-5">
      <div className="card mb-3">
        <div className="row g-0">
          <div className="col-md-4">
            <img src={vehicle.imageUrl || 'https://via.placeholder.com/300'} className="img-fluid rounded-start" alt={`${vehicle.make} ${vehicle.model}`} />
          </div>
          <div className="col-md-8">
            <div className="card-body">
              <h1 className="card-title">{vehicle.make} {vehicle.model} ({vehicle.year})</h1>
              <p className="card-text"><strong>Type:</strong> {vehicle.type}</p>
              <p className="card-text"><strong>Daily Rate:</strong> ${vehicle.dailyRate}</p>
              <p className="card-text"><strong>Color:</strong> {vehicle.color}</p>
              <p className="card-text"><strong>License Plate:</strong> {vehicle.licensePlate}</p>
              <p className="card-text"><strong>Description:</strong> {vehicle.description || 'No description available.'}</p>
              
              {/* "Rent Now" button will link to the rental process later */}
              <Link to="/rentals/new" className="btn btn-success me-2">Rent Now</Link>
              <Link to="/vehicles" className="btn btn-secondary">Back to Vehicles</Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default VehicleDetailPage;