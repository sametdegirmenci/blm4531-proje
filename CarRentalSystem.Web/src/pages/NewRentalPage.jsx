import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import api from '../services/api';
import DatePicker from 'react-datepicker'; // Need to install react-datepicker
import 'react-datepicker/dist/react-datepicker.css';
import { useAuth } from '../context/AuthContext';


function NewRentalPage() {
  const { id } = useParams(); // Potentially pre-selected vehicle ID
  const navigate = useNavigate();
  const { user } = useAuth();

  const [vehicles, setVehicles] = useState([]);
  const [selectedVehicleId, setSelectedVehicleId] = useState(id || '');
  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const fetchVehicles = async () => {
      try {
        const response = await api.get('/vehicles/available');
        setVehicles(response.data.data || []);
        if (id && !response.data.data.some(v => v.vehicleId === id)) {
          setError('Pre-selected vehicle not found or not available.');
        }
      } catch (err) {
        setError('Failed to fetch available vehicles for selection.');
        console.error('Error fetching vehicles:', err);
      }
    };
    fetchVehicles();
  }, [id]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    setLoading(true);

    if (!selectedVehicleId || !startDate || !endDate) {
      setError('Please fill in all required fields.');
      setLoading(false);
      return;
    }

    if (startDate >= endDate) {
      setError('End date must be after start date.');
      setLoading(false);
      return;
    }

    try {
      // Helper to format date as YYYY-MM-DD using local time
      const formatDateForApi = (date) => {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
      };

      const rentalData = {
        vehicleId: parseInt(selectedVehicleId), // Ensure it's an integer
        startDate: formatDateForApi(startDate),
        endDate: formatDateForApi(endDate),
      };

      const response = await api.post('/reservations', rentalData);
      setSuccess('Reservation booked successfully!');
      setTimeout(() => {
        navigate('/rentals'); // Redirect to rentals list
      }, 1500);

    } catch (err) {
      console.error('Rental booking failed:', err);
      setError(err.response?.data?.Message || 'Rental booking failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mt-5">
      <div className="row justify-content-center">
        <div className="col-md-8">
          <div className="card">
            <div className="card-header">Book New Rental</div>
            <div className="card-body">
              <form onSubmit={handleSubmit}>
                {error && <div className="alert alert-danger">{error}</div>}
                {success && <div className="alert alert-success">{success}</div>}

                <div className="mb-3">
                  <label htmlFor="vehicleSelect" className="form-label">Select Vehicle</label>
                  <select
                    className="form-select"
                    id="vehicleSelect"
                    value={selectedVehicleId}
                    onChange={(e) => setSelectedVehicleId(e.target.value)}
                    required
                    disabled={!!id} // Disable if vehicle is pre-selected
                  >
                    <option value="">-- Choose a vehicle --</option>
                    {vehicles.map((v) => (
                      <option key={v.vehicleId} value={v.vehicleId}>
                        {v.brand} {v.model} ({v.year}) - ${v.pricePerDay}/day
                      </option>
                    ))}
                  </select>
                </div>

                <div className="mb-3">
                  <label className="form-label">Start Date</label>
                  <DatePicker
                    selected={startDate}
                    onChange={(date) => setStartDate(date)}
                    selectsStart
                    startDate={startDate}
                    endDate={endDate}
                    minDate={new Date()}
                    dateFormat="yyyy/MM/dd"
                    className="form-control"
                    required
                  />
                </div>

                <div className="mb-3">
                  <label className="form-label">End Date</label>
                  <DatePicker
                    selected={endDate}
                    onChange={(date) => setEndDate(date)}
                    selectsEnd
                    startDate={startDate}
                    endDate={endDate}
                    minDate={startDate || new Date()} // End date must be after start date
                    dateFormat="yyyy/MM/dd"
                    className="form-control"
                    required
                  />
                </div>

                {/* Availability Check will go here - requires backend endpoint */}
                {/* <button type="button" className="btn btn-info mb-3">Check Availability</button> */}

                <button type="submit" className="btn btn-primary" disabled={loading}>
                  {loading ? 'Booking...' : 'Book Rental'}
                </button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default NewRentalPage;
