import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../services/api';

function PaymentPage() {
  const { reservationId } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [processing, setProcessing] = useState(false);
  const [reservation, setReservation] = useState(null);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);
  
  // Mock form state
  const [cardNumber, setCardNumber] = useState('');
  const [expiry, setExpiry] = useState('');
  const [cvv, setCvv] = useState('');
  const [cardName, setCardName] = useState('');

  useEffect(() => {
    const fetchReservation = async () => {
      try {
        // Fetch reservation details to show amount
        const response = await api.get(`/reservations/${reservationId}`);
        setReservation(response.data.data);
      } catch (err) {
        setError('Failed to load reservation details.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchReservation();
  }, [reservationId]);

  const handlePayment = async (e) => {
    e.preventDefault();
    setProcessing(true);
    setError('');

    try {
      // Validate mock inputs (basic check)
      if (!cardNumber || !expiry || !cvv || !cardName) {
        throw new Error('Please fill in all payment details.');
      }

      const paymentData = {
        reservationId: parseInt(reservationId),
        amount: reservation.totalPrice,
        paymentMethod: 'CreditCard',
        transactionReference: 'TXN-' + Date.now()
      };

      await api.post('/payments/process', paymentData);
      setSuccess(true);
      
      setTimeout(() => {
        navigate('/rentals');
      }, 3000);

    } catch (err) {
      console.error('Payment failed:', err);
      setError(err.response?.data?.Message || err.message || 'Payment processing failed.');
    } finally {
      setProcessing(false);
    }
  };

  const handleCancelPayment = async () => {
    if (confirm('Are you sure you want to cancel the payment? This will reject the reservation.')) {
      try {
        await api.post(`/reservations/${reservationId}/cancel-payment`);
        navigate('/rentals');
      } catch (err) {
        console.error('Failed to cancel payment:', err);
        setError('Failed to cancel payment properly.');
      }
    }
  };

  if (loading) return <div className="text-center mt-5"><div className="spinner-border text-primary"></div></div>;
  if (!reservation) return <div className="container mt-5"><div className="alert alert-danger">Reservation not found.</div></div>;

  if (success) {
    return (
      <div className="container mt-5 text-center">
        <div className="card shadow-sm border-0 p-5">
          <div className="mb-4 text-success">
            <i className="bi bi-check-circle-fill display-1"></i>
          </div>
          <h2 className="mb-3">Payment Successful!</h2>
          <p className="lead text-muted">Your reservation for <strong>{reservation.vehicle?.brand} {reservation.vehicle?.model}</strong> is confirmed.</p>
          <p>Redirecting you to your rentals...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="container py-5">
      <div className="row justify-content-center">
        <div className="col-md-6">
          <div className="card shadow-sm border-0">
            <div className="card-header bg-primary text-white py-3">
              <h4 className="mb-0"><i className="bi bi-credit-card me-2"></i>Secure Payment</h4>
            </div>
            <div className="card-body p-4">
              <div className="alert alert-info mb-4">
                <div className="d-flex justify-content-between align-items-center">
                    <span>Total Amount:</span>
                    <strong className="h4 mb-0">${reservation.totalPrice}</strong>
                </div>
              </div>

              {error && <div className="alert alert-danger">{error}</div>}

              <form onSubmit={handlePayment}>
                <div className="mb-3">
                  <label className="form-label text-muted small fw-bold">Name on Card</label>
                  <input 
                    type="text" 
                    className="form-control" 
                    placeholder="JOHN DOE"
                    value={cardName}
                    onChange={(e) => setCardName(e.target.value)}
                  />
                </div>

                <div className="mb-3">
                  <label className="form-label text-muted small fw-bold">Card Number</label>
                  <input 
                    type="text" 
                    className="form-control" 
                    placeholder="0000 0000 0000 0000"
                    value={cardNumber}
                    onChange={(e) => setCardNumber(e.target.value)}
                  />
                </div>

                <div className="row">
                  <div className="col-md-6 mb-3">
                    <label className="form-label text-muted small fw-bold">Expiry Date</label>
                    <input 
                      type="text" 
                      className="form-control" 
                      placeholder="MM/YY"
                      value={expiry}
                      onChange={(e) => setExpiry(e.target.value)}
                    />
                  </div>
                  <div className="col-md-6 mb-3">
                    <label className="form-label text-muted small fw-bold">CVV</label>
                    <input 
                      type="text" 
                      className="form-control" 
                      placeholder="123"
                      value={cvv}
                      onChange={(e) => setCvv(e.target.value)}
                    />
                  </div>
                </div>

                <div className="d-grid mt-4">
                  <button type="submit" className="btn btn-primary btn-lg" disabled={processing}>
                    {processing ? (
                        <>
                            <span className="spinner-border spinner-border-sm me-2"></span>
                            Processing...
                        </>
                    ) : `Pay $${reservation.totalPrice}`}
                  </button>
                  <button type="button" className="btn btn-link text-muted mt-2" onClick={handleCancelPayment}>
                    Cancel Payment
                  </button>
                </div>
              </form>
            </div>
            <div className="card-footer bg-light text-center small text-muted">
                <i className="bi bi-lock-fill me-1"></i> This is a secure mock payment gateway.
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default PaymentPage;
