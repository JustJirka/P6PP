import React, { useState, useEffect } from 'react';
import { Form, Button, Alert, Row, Col } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import Api from '../../utils/Api';

const CreateService = () => {
  const navigate = useNavigate();
  const [service, setService] = useState({
    start: '',
    end: '',
    price: 0,
    serviceName: '',
    trainerId: 0,
    roomId: 0,
    isCancelled: false
  });
  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchRooms = async () => {
      try {
        setLoading(true);
        const response = await Api.getRooms();
        setRooms(response.data || []);
      } catch (err) {
        setError(`Error loading rooms: ${err.message}`);
      } finally {
        setLoading(false);
      }
    };

    fetchRooms();
  }, []);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    let finalValue = value;

    if (type === 'checkbox') {
      finalValue = checked;
    } else if (['price', 'trainerId', 'roomId'].includes(name)) {
      finalValue = value === '' ? 0 : parseInt(value, 10);
    }

    setService(prev => ({ ...prev, [name]: finalValue }));
  };

  const formatDateForApi = (dateTimeString) => {
    if (!dateTimeString) return null;
    return new Date(dateTimeString).toISOString();
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    try {
      const formattedService = {
        start: formatDateForApi(service.start),
        end: formatDateForApi(service.end),
        price: Number(service.price),
        serviceName: service.serviceName,
        trainerId: Number(service.trainerId),
        roomId: Number(service.roomId)
      };
      
      console.log('Submitting service data:', formattedService);
      const response = await Api.createService(formattedService);
      console.log('Service created successfully:', response);
      navigate('/admin/services');
    } catch (err) {
      console.error('Failed to create service:', err);
      setError(`Error creating service: ${err.message || err}`);
    }
  };

  return (
    <div>
      <h2>New Service</h2>
      {error && <Alert variant="danger">{error}</Alert>}
      <Form onSubmit={handleSubmit}>
        <Row>
          <Col md={6}>
            <Form.Group className="mb-3">
              <Form.Label>Service Name</Form.Label>
              <Form.Control
                name="serviceName"
                value={service.serviceName}
                onChange={handleChange}
                required
              />
            </Form.Group>
          </Col>
          <Col md={6}>
            <Form.Group className="mb-3">
              <Form.Label>Price</Form.Label>
              <Form.Control
                name="price"
                type="number"
                min="0"
                value={service.price}
                onChange={handleChange}
                required
              />
            </Form.Group>
          </Col>
        </Row>

        <Row>
          <Col md={6}>
            <Form.Group className="mb-3">
              <Form.Label>Start Time</Form.Label>
              <Form.Control
                name="start"
                type="datetime-local"
                value={service.start}
                onChange={handleChange}
                required
              />
            </Form.Group>
          </Col>
          <Col md={6}>
            <Form.Group className="mb-3">
              <Form.Label>End Time</Form.Label>
              <Form.Control
                name="end"
                type="datetime-local"
                value={service.end}
                onChange={handleChange}
                required
              />
            </Form.Group>
          </Col>
        </Row>

        <Row>
          <Col md={6}>
            <Form.Group className="mb-3">
              <Form.Label>Trainer ID</Form.Label>
              <Form.Control
                name="trainerId"
                type="number"
                min="0"
                value={service.trainerId}
                onChange={handleChange}
                required
              />
            </Form.Group>
          </Col>
          <Col md={6}>
            <Form.Group className="mb-3">
              <Form.Label>Room</Form.Label>
              <Form.Select 
                name="roomId"
                value={service.roomId}
                onChange={handleChange}
                required
              >
                <option value="">Select a room</option>
                {rooms.map(room => (
                  <option key={room.id} value={room.id}>
                    {room.name}
                  </option>
                ))}
              </Form.Select>
            </Form.Group>
          </Col>
        </Row>

        <Button variant="primary" type="submit">Create Service</Button>
      </Form>
    </div>
  );
};

export default CreateService;
