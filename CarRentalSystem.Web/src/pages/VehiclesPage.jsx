import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import api from '../services/api';
import { useAuth } from '../context/AuthContext'; // Import useAuth
import VehicleForm from '../components/VehicleForm'; // Import VehicleForm

function VehiclesPage() {
  const { user } = useAuth(); // Get user from AuthContext
  const [vehicles, setVehicles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchQuery, setSearchQuery] = useState('');
  const [filteredVehicles, setFilteredVehicles] = useState([]);
  const [showVehicleForm, setShowVehicleForm] = useState(false);
  const [editingVehicle, setEditingVehicle] = useState(null);

  const fetchVehicles = async () => {
    try {
      const response = await api.get('/vehicles');
      setVehicles(response.data.data || []);
      setFilteredVehicles(response.data.data || []);
    } catch (err) {
      setError('Failed to fetch vehicles.');
      console.error('Error fetching vehicles:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchVehicles();
  }, []);

  useEffect(() => {
    const lowerCaseQuery = searchQuery.toLowerCase();
    const results = vehicles.filter(vehicle =>
      vehicle.brand.toLowerCase().includes(lowerCaseQuery) ||
      vehicle.model.toLowerCase().includes(lowerCaseQuery) ||
      (vehicle.year && vehicle.year.toString().includes(lowerCaseQuery))
    );
    setFilteredVehicles(results);
  }, [searchQuery, vehicles]);

  const handleAddVehicleClick = () => {
    setEditingVehicle(null); // Clear any editing state
    setShowVehicleForm(true);
  };

  const handleEditVehicleClick = (vehicle) => {
    setEditingVehicle(vehicle);
    setShowVehicleForm(true);
  };

  const handleDeleteVehicle = async (id) => {
    if (window.confirm('Are you sure you want to delete this vehicle?')) {
      try {
        await api.delete(`/vehicles/${id}`);
        fetchVehicles(); // Refresh the list
      } catch (err) {
        setError('Failed to delete vehicle.');
        console.error('Error deleting vehicle:', err);
      }
    }
  };

  const handleFormSubmit = () => {
    setShowVehicleForm(false);
    setEditingVehicle(null);
    fetchVehicles(); // Refresh the list after add/edit
  };

  const handleFormCancel = () => {
    setShowVehicleForm(false);
    setEditingVehicle(null);
  };

  if (loading) {
    return (
      <div className="container mt-5">
        <p>Loading vehicles...</p>
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
      <h1>Available Vehicles</h1>

      {user && user.role === 'Admin' && (
        <div className="mb-3">
          <button className="btn btn-success" onClick={handleAddVehicleClick}>Add New Vehicle</button>
        </div>
      )}

      {showVehicleForm && (
        <div className="mb-4">
          <VehicleForm
            vehicleData={editingVehicle}
            onSubmit={handleFormSubmit}
            onCancel={handleFormCancel}
          />
        </div>
      )}

      <div className="mb-3">
        <input
          type="text"
          className="form-control"
          placeholder="Search by make, model, year, or type..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
        />
      </div>

      <div className="row">
        {filteredVehicles.length > 0 ? (
          filteredVehicles.map((vehicle) => (
            <div key={vehicle.vehicleId} className="col-md-4 mb-4">
              <div className="card">
                <img src={vehicle.imageUrl || 'https://via.placeholder.com/150'} className="card-img-top" alt={`${vehicle.brand} ${vehicle.model}`} />
                <div className="card-body">
                  <h5 className="card-title">{vehicle.brand} {vehicle.model} ({vehicle.year})</h5>
                  <p className="card-text">Daily Rate: ${vehicle.pricePerDay}</p>
                  {user && user.role === 'Admin' && (
                    <>
                      <button className="btn btn-warning btn-sm me-2" onClick={() => handleEditVehicleClick(vehicle)}>Edit</button>
                      <button className="btn btn-danger btn-sm" onClick={() => handleDeleteVehicle(vehicle.vehicleId)}>Delete</button>
                    </>
                  )}
                </div>
              </div>
            </div>
          ))
        ) : (
          <p>No vehicles found.</p>
        )}
      </div>
    </div>
  );
}

export default VehiclesPage;
