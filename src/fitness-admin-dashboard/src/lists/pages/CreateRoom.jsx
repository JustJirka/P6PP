import React, { useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import Api from '../../utils/Api';

const CreateRoom = () => {
  const navigate = useNavigate();
  const [room, setRoom] = useState({ name: '', capacity: '', status: 'Available' });
  const [error, setError] = useState(null);

  const handleChange = (e) => {
    const { name, value } = e.target;

    setRoom(prev => ({
      ...prev,
      [name]: name === 'capacity' ? parseInt(value, 10) : value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
  
    try {
      console.log('Sending payload:', room);
  
      await Api.post('/booking-api/api/Rooms', room);
  
      navigate('/admin/rooms');
    } catch (err) {
      setError(`Error when creating a room: ${err.message}`);
    }
  };
  

  return (
    <div>
      <h2>Create new room</h2>
      {error && <Alert variant="danger">{error}</Alert>}
      <Form onSubmit={handleSubmit}>
        <Form.Group className="mb-3">
          <Form.Label>Name</Form.Label>
          <Form.Control
            name="name"
            value={room.name}
            onChange={handleChange}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Capacity</Form.Label>
          <Form.Control
            name="capacity"
            type="number"
            min="1"
            value={room.capacity}
            onChange={handleChange}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Status</Form.Label>
          <Form.Select
            name="status"
            value={room.status}
            onChange={handleChange}
          >
            <option value="Available">Available</option>
            <option value="Occupied">Occupied</option>
            <option value="Maintenance">Maintenance</option>
            <option value="Reserved">Reserved</option>
          </Form.Select>
        </Form.Group>

        <Button type="submit">Create</Button>
      </Form>
    </div>
  );
};

export default CreateRoom;
