import React, { useState, useEffect } from 'react';
import api from '../services/api';

function VehicleForm({ vehicleData, onSubmit, onCancel }) {
  const [brand, setBrand] = useState('');
  const [model, setModel] = useState('');
  const [year, setYear] = useState('');
  const [pricePerDay, setPricePerDay] = useState('');
  const [licensePlate, setLicensePlate] = useState('');
  const [imageUrl, setImageUrl] = useState('');
  const [isAvailable, setIsAvailable] = useState(true);
  const [error, setError] = useState('');

  const isEditing = !!vehicleData;

  useEffect(() => {
    if (isEditing) {
      setBrand(vehicleData.brand || '');
      setModel(vehicleData.model || '');
      setYear(vehicleData.year || '');
      setPricePerDay(vehicleData.pricePerDay || '');
      setLicensePlate(vehicleData.licensePlate || '');
      setImageUrl(vehicleData.imageUrl || '');
      setIsAvailable(vehicleData.isAvailable);
    }
  }, [vehicleData, isEditing]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    const payload = {
      brand,
      model,
      year: parseInt(year),
      pricePerDay: parseFloat(pricePerDay),
      licensePlate,
      imageUrl: imageUrl || null, // Send null if imageUrl is empty
    };
    
    // Add isAvailable only when editing
    if (isEditing) {
      payload.isAvailable = isAvailable;
    }

    try {
      if (isEditing) {
        await api.put(`/vehicles/${vehicleData.vehicleId}`, payload);
      } else {
        await api.post('/vehicles', payload);
      }
      onSubmit();
    } catch (err) {
      console.error('Failed to save vehicle:', err);
      setError(err.response?.data?.Message || 'Failed to save vehicle.');
    }
  };

  return (
    <div className="card p-4">
      <h3>{isEditing ? 'Edit Vehicle' : 'Add New Vehicle'}</h3>
      <form onSubmit={handleSubmit}>
        {error && <div className="alert alert-danger">{error}</div>}

        <div className="mb-3">
          <label className="form-label">Brand</label>
          <input type="text" className="form-control" value={brand} onChange={(e) => setBrand(e.target.value)} required />
        </div>
        <div className="mb-3">
          <label className="form-label">Model</label>
          <input type="text" className="form-control" value={model} onChange={(e) => setModel(e.target.value)} required />
        </div>
        <div className="mb-3">
          <label className="form-label">Year</label>
          <input type="number" className="form-control" value={year} onChange={(e) => setYear(e.target.value)} required />
        </div>
        <div className="mb-3">
          <label className="form-label">Price Per Day</label>
          <input type="number" step="0.01" className="form-control" value={pricePerDay} onChange={(e) => setPricePerDay(e.target.value)} required />
        </div>
        <div className="mb-3">
          <label className="form-label">License Plate</label>
          <input type="text" className="form-control" value={licensePlate} onChange={(e) => setLicensePlate(e.target.value)} required />
        </div>
        <div className="mb-3">
          <label className="form-label">Image URL</label>
          <input type="url" className="form-control" value={imageUrl} onChange={(e) => setImageUrl(e.target.value)} />
        </div>
        
        {isEditing && (
          <div className="mb-3 form-check">
            <input
              type="checkbox"
              className="form-check-input"
              id="isAvailableCheck"
              checked={isAvailable}
              onChange={(e) => setIsAvailable(e.target.checked)}
            />
            <label className="form-check-label" htmlFor="isAvailableCheck">Available</label>
          </div>
        )}

        <button type="submit" className="btn btn-primary me-2">Save Vehicle</button>
        <button type="button" className="btn btn-secondary" onClick={onCancel}>Cancel</button>
      </form>
    </div>
  );
}

export default VehicleForm;
