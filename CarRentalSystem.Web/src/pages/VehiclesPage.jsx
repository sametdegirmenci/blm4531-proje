import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import api from '../services/api';
import { useAuth } from '../context/AuthContext';
import VehicleForm from '../components/VehicleForm';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';

function VehiclesPage() {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [vehicles, setVehicles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchQuery, setSearchQuery] = useState('');
  const [filteredVehicles, setFilteredVehicles] = useState([]);
  const [showVehicleForm, setShowVehicleForm] = useState(false);
  const [editingVehicle, setEditingVehicle] = useState(null);

  // New state for date filtering and renting
  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);
  const [rentingId, setRentingId] = useState(null);
  const [successMessage, setSuccessMessage] = useState('');

  const formatDateForApi = (date) => {
    if (!date) return null;
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  };

  const fetchVehicles = async (start = startDate, end = endDate) => {
    setLoading(true);
    try {
      let url = '/vehicles';
      const params = {};
      
      if (start && end) {
        url = '/vehicles/available';
        params.startDate = formatDateForApi(start);
        params.endDate = formatDateForApi(end);
      }

      const response = await api.get(url, { params });
      setVehicles(response.data.data || []);
      setFilteredVehicles(response.data.data || []);
      setError('');
    } catch (err) {
      setError('Failed to fetch vehicles.');
      console.error('Error fetching vehicles:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchVehicles();
  }, []); // Initial load

  useEffect(() => {
    const lowerCaseQuery = searchQuery.toLowerCase();
    const results = vehicles.filter(vehicle =>
      vehicle.brand.toLowerCase().includes(lowerCaseQuery) ||
      vehicle.model.toLowerCase().includes(lowerCaseQuery) ||
      (vehicle.year && vehicle.year.toString().includes(lowerCaseQuery))
    );
    setFilteredVehicles(results);
  }, [searchQuery, vehicles]);

  const handleCheckAvailability = () => {
    if (startDate && endDate) {
      if (startDate >= endDate) {
        setError('End date must be after start date.');
        return;
      }
      fetchVehicles(startDate, endDate);
    } else {
      fetchVehicles(null, null); // Reset to all vehicles if dates cleared
    }
  };

  const handleRentClick = async (vehicleId) => {
    if (!user) {
      navigate('/login');
      return;
    }

    if (!startDate || !endDate) {
      setError('Please select a pickup and return date to rent this vehicle.');
      window.scrollTo(0, 0);
      return;
    }

    if (confirm('Are you sure you want to rent this vehicle for the selected dates?')) {
      setRentingId(vehicleId);
      setError('');
      setSuccessMessage('');

      try {
        const rentalData = {
          vehicleId: vehicleId,
          startDate: formatDateForApi(startDate),
          endDate: formatDateForApi(endDate),
        };

        const response = await api.post('/reservations', rentalData);
        setSuccessMessage('Reservation initiated! Redirecting to payment...');
        
        // The API returns the created reservation in response.data.data
        // We need its ID to redirect to the payment page.
        const reservationId = response.data.data.reservationId;
        
        setTimeout(() => {
            navigate(`/payment/${reservationId}`);
        }, 1500);
      } catch (err) {
        console.error('Rent error:', err);
        setError(err.response?.data?.Message || 'Failed to create reservation.');
      } finally {
        setRentingId(null);
      }
    }
  };

  const handleAddVehicleClick = () => {
    setEditingVehicle(null);
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
        fetchVehicles();
      } catch (err) {
        setError('Failed to delete vehicle.');
        console.error('Error deleting vehicle:', err);
      }
    }
  };

  const handleFormSubmit = () => {
    setShowVehicleForm(false);
    setEditingVehicle(null);
    fetchVehicles();
  };

  const handleFormCancel = () => {
    setShowVehicleForm(false);
    setEditingVehicle(null);
  };

  if (loading && !vehicles.length) {
    return (
      <div className="container mt-5 text-center">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="container py-5">
      <div className="d-flex justify-content-between align-items-center mb-5">
        <div>
          <h1 className="mb-2">Our Fleet</h1>
          <p className="text-muted mb-0">Choose from our wide range of premium vehicles</p>
        </div>
        
        {user && user.role === 'Admin' && (
          <button className="btn btn-primary" onClick={handleAddVehicleClick}>
            <i className="bi bi-plus-lg me-2"></i>
            Add Vehicle
          </button>
        )}
      </div>

      {error && (
        <div className="alert alert-danger rounded-3 shadow-sm mb-4">
          <i className="bi bi-exclamation-triangle-fill me-2"></i>
          {error}
        </div>
      )}
      
      {successMessage && (
        <div className="alert alert-success rounded-3 shadow-sm mb-4">
          <i className="bi bi-check-circle-fill me-2"></i>
          {successMessage}
        </div>
      )}

      {showVehicleForm && (
        <div className="card-custom p-4 mb-5 border-0 bg-light">
          <h3 className="mb-4">{editingVehicle ? 'Edit Vehicle' : 'Add New Vehicle'}</h3>
          <VehicleForm
            vehicleData={editingVehicle}
            onSubmit={handleFormSubmit}
            onCancel={handleFormCancel}
          />
        </div>
      )}

      {/* Filter Section */}
      <div className="card shadow-sm mb-5 border-0">
        <div className="card-body p-4">
            <h5 className="card-title mb-3">Find Your Car</h5>
            <div className="row g-3">
                <div className="col-md-4">
                    <label className="form-label small text-muted">Pick-up Date</label>
                    <DatePicker
                        selected={startDate}
                        onChange={(date) => setStartDate(date)}
                        selectsStart
                        startDate={startDate}
                        endDate={endDate}
                        minDate={new Date()}
                        className="form-control"
                        placeholderText="Select start date"
                    />
                </div>
                <div className="col-md-4">
                    <label className="form-label small text-muted">Return Date</label>
                    <DatePicker
                        selected={endDate}
                        onChange={(date) => setEndDate(date)}
                        selectsEnd
                        startDate={startDate}
                        endDate={endDate}
                        minDate={startDate || new Date()}
                        className="form-control"
                        placeholderText="Select end date"
                    />
                </div>
                <div className="col-md-4 d-flex align-items-end">
                    <button className="btn btn-primary w-100" onClick={handleCheckAvailability}>
                        Check Availability
                    </button>
                </div>
            </div>
             <div className="mt-3">
                 <input
                  type="text"
                  className="form-control"
                  placeholder="Search by brand, model, or year..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                />
             </div>
        </div>
      </div>

      <div className="row g-4">
        {filteredVehicles.length > 0 ? (
          filteredVehicles.map((vehicle) => (
            <div key={vehicle.vehicleId} className="col-md-6 col-lg-4">
              <div className="card-custom h-100 d-flex flex-column">
                <div className="position-relative">
                  <img 
                    src={vehicle.imageUrl || 'https://placehold.co/400x250?text=No+Image'} 
                    className="card-img-top" 
                    alt={`${vehicle.brand} ${vehicle.model}`}
                    style={{ height: '240px', objectFit: 'cover' }}
                    onError={(e) => { e.target.onerror = null; e.target.src = 'https://placehold.co/400x250?text=Image+Unavailable'; }}
                  />
                  {vehicle.isAvailable ? (
                     <span className="position-absolute top-0 end-0 badge bg-success m-3">Available</span>
                  ) : (
                     <span className="position-absolute top-0 end-0 badge bg-secondary m-3">Unavailable</span>
                  )}
                </div>
                
                <div className="card-body p-4 d-flex flex-column">
                  <div className="d-flex justify-content-between align-items-start mb-2">
                    <h5 className="card-title mb-0 fw-bold">{vehicle.brand} {vehicle.model}</h5>
                    <span className="badge bg-light text-dark border">{vehicle.year}</span>
                  </div>
                  
                  <div className="mt-auto pt-3">
                    <div className="d-flex justify-content-between align-items-center mb-3">
                      <span className="text-muted small">Daily Rate</span>
                      <span className="h4 text-primary mb-0">${vehicle.pricePerDay}</span>
                    </div>

                    <div className="d-grid gap-2">
                      <button 
                        className="btn btn-primary"
                        onClick={() => handleRentClick(vehicle.vehicleId)}
                        disabled={rentingId === vehicle.vehicleId || !vehicle.isAvailable}
                      >
                         {rentingId === vehicle.vehicleId ? 'Processing...' : 'Rent Now'}
                      </button>

                      {user && user.role === 'Admin' && (
                        <div className="d-flex gap-2 mt-2">
                          <button 
                            className="btn btn-light flex-grow-1 text-muted" 
                            onClick={() => handleEditVehicleClick(vehicle)}
                          >
                            <i className="bi bi-pencil me-1"></i> Edit
                          </button>
                          <button 
                            className="btn btn-light flex-grow-1 text-danger" 
                            onClick={() => handleDeleteVehicle(vehicle.vehicleId)}
                          >
                            <i className="bi bi-trash me-1"></i> Delete
                          </button>
                        </div>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          ))
        ) : (
          <div className="col-12 text-center py-5">
            <div className="display-1 text-muted mb-3">
              <i className="bi bi-car-front"></i>
            </div>
            <h3>No vehicles found</h3>
            <p className="text-muted">Try adjusting your dates or search criteria</p>
          </div>
        )}
      </div>
    </div>
  );
}

export default VehiclesPage;
