import React, { useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import Api from '../../utils/Api';

const CreateBooking = () => {
  const navigate = useNavigate();
  const [booking, setBooking] = useState({ serviceId: '', status: 'Confirmed' });
  const [error, setError] = useState(null);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setBooking(prev => ({ ...prev, [name]: name === 'serviceId' ? parseInt(value, 10) : value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
        await Api.createBooking(booking);
      navigate('/admin/bookings');
    } catch (err) {
      setError(`Error making a reservation: ${err.message}`);
    }
  };

  return (
    <div>
      <h2>New reservation</h2>
      {error && <Alert variant="danger">{error}</Alert>}
      <Form onSubmit={handleSubmit}>
        <Form.Group className="mb-3">
          <Form.Label>Service ID</Form.Label>
          <Form.Control
            name="serviceId"
            type="number"
            value={booking.serviceId}
            onChange={handleChange}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Status</Form.Label>
          <Form.Select name="status" value={booking.status} onChange={handleChange}>
            <option value="Confirmed">Confirmed</option>
            <option value="Pending">Pending</option>
            <option value="Cancelled">Cancelled</option>
          </Form.Select>
        </Form.Group>

        <Button type="submit">Create</Button>
      </Form>
    </div>
  );
};

export default CreateBooking;
