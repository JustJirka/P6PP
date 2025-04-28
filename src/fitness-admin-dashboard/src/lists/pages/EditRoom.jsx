import React, { useEffect, useState } from 'react';
import { Form, Button, Spinner, Alert } from 'react-bootstrap';
import { useNavigate, useParams } from 'react-router-dom';
import Api from '../../utils/Api';

const EditRoom = () => {
  const { roomId } = useParams();
  const navigate = useNavigate();
  const [room, setRoom] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchRoom = async () => {
      try {
        const data = await Api.get(`/booking-api/api/Rooms/${roomId}`);
        console.log('Room from API:', data);
        setRoom(data.data); // ✅ jen samotná místnost
      } catch (err) {
        setError(`Error with loading room: ${err.message}`);
      }
    };    

    fetchRoom();
  }, [roomId]);

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
      await Api.put(`/booking-api/api/Rooms/${roomId}`, room);
      navigate('/admin/rooms');
    } catch (err) {
      setError(`Error when saving room: ${err.message}`);
    }
  };

  if (!room) return <Spinner animation="border" />;

  return (
    <div>
      <h2>Edit room</h2>
      {error && <Alert variant="danger">{error}</Alert>}
      <Form onSubmit={handleSubmit}>
        <Form.Group className="mb-3">
          <Form.Label>Name</Form.Label>
          <Form.Control
            name="name"
            value={room?.name || ''}
            onChange={handleChange}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Capacity</Form.Label>
          <Form.Control
            name="capacity"
            type="number"
            value={room?.capacity || ''}
            onChange={handleChange}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Status</Form.Label>
          <Form.Select
            name="status"
            value={room?.status || 'Available'}
            onChange={handleChange}
          >
            <option value="Available">Available</option>
            <option value="Occupied">Occupied</option>
            <option value="Maintenance">Maintenance</option>
            <option value="Reserved">Reserved</option>
          </Form.Select>
        </Form.Group>

        <Button type="submit">Save</Button>
      </Form>
    </div>
  );
};

export default EditRoom;
