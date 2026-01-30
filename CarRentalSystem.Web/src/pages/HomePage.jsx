import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

function HomePage() {
  const { user } = useAuth();

  return (
    <div className="home-page">
      {/* Hero Section */}
      <section className="hero-section">
        <div className="container">
          <h1 className="hero-title">Drive Your Dreams</h1>
          <p className="hero-subtitle">
            Premium car rental service for your daily commutes and weekend getaways. 
            Experience comfort, safety, and style at the best rates.
          </p>
          <div className="d-flex justify-content-center gap-3">
            <Link to="/vehicles" className="btn btn-primary btn-lg">
              Browse Cars
            </Link>
            {!user && (
              <Link to="/register" className="btn btn-outline-light btn-lg">
                Sign Up Now
              </Link>
            )}
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="section-padding">
        <div className="container">
          <div className="row text-center mb-5">
            <div className="col-12">
              <h2 className="mb-3">Why Choose Us?</h2>
              <p className="text-muted">We provide the best experience for our customers</p>
            </div>
          </div>
          <div className="row g-4">
            <div className="col-md-4">
              <div className="card-custom p-4 text-center h-100">
                <div className="display-4 text-accent mb-3">
                  <i className="bi bi-shield-check"></i>
                </div>
                <h3>Secure & Safe</h3>
                <p className="text-muted">
                  All our vehicles are regularly inspected and insured for your peace of mind.
                </p>
              </div>
            </div>
            <div className="col-md-4">
              <div className="card-custom p-4 text-center h-100">
                <div className="display-4 text-accent mb-3">
                  <i className="bi bi-tag"></i>
                </div>
                <h3>Best Prices</h3>
                <p className="text-muted">
                  We offer competitive daily rates and special discounts for long-term rentals.
                </p>
              </div>
            </div>
            <div className="col-md-4">
              <div className="card-custom p-4 text-center h-100">
                <div className="display-4 text-accent mb-3">
                  <i className="bi bi-headset"></i>
                </div>
                <h3>24/7 Support</h3>
                <p className="text-muted">
                  Our customer support team is always available to assist you with any issues.
                </p>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Call to Action */}
      <section className="bg-white section-padding">
        <div className="container">
          <div className="row align-items-center">
            <div className="col-lg-6">
              <h2>Ready to hit the road?</h2>
              <p className="lead text-muted mb-4">
                Join thousands of satisfied customers who trust AraçKontrol for their travel needs.
              </p>
              <Link to="/vehicles" className="btn btn-primary btn-lg">
                Book Your Ride
              </Link>
            </div>
            <div className="col-lg-6 mt-4 mt-lg-0">
               <img 
                 src="https://images.unsplash.com/photo-1485291571150-772bcfc10da5?ixlib=rb-1.2.1&auto=format&fit=crop&w=1000&q=80" 
                 alt="Car Rental" 
                 className="img-fluid rounded-4 shadow-lg"
               />
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}

export default HomePage;