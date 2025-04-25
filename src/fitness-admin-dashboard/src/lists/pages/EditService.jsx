import React, { useEffect, useState } from 'react';
import { Form, Button, Spinner, Alert, Row, Col } from 'react-bootstrap';
import { useNavigate, useParams } from 'react-router-dom';
import Api from '../../utils/Api';

const EditService = () => {
  const { serviceId } = useParams();
  const navigate = useNavigate();
  const [service, setService] = useState(null);
  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        // Fetch service data
        const serviceData = await Api.get(`/booking-api/api/Services/${serviceId}`);
        console.log('Service from API:', serviceData);
        
        // Format dates for datetime-local input
        const service = serviceData.data || serviceData;
        if (service.start) {
          const startDate = new Date(service.start);
          service.start = startDate.toISOString().slice(0, 16); // Format as YYYY-MM-DDThh:mm
        }
        if (service.end) {
          const endDate = new Date(service.end);
          service.end = endDate.toISOString().slice(0, 16); // Format as YYYY-MM-DDThh:mm
        }
        
        // Fetch rooms data
        const roomsResponse = await Api.getRooms();
        const rooms = roomsResponse.data || [];
        
        // Convert roomName to roomId if needed
        if (service.roomName) {
          const room = rooms.find(r => r.name === service.roomName);
          if (room) {
            service.roomId = room.id;
          }
        }
        
        setService(service);
        setRooms(rooms);
      } catch (err) {
        setError(`Error loading service: ${err.message}`);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [serviceId]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setService(prev => ({
      ...prev,
      [name]: name === 'roomId' ? parseInt(value, 10) : value
    }));
  };

  const formatDateForApi = (dateTimeString) => {
    if (!dateTimeString) return null;
    return new Date(dateTimeString).toISOString();
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      // Create the service object without including the ID in the body
      const formattedService = {
        start: formatDateForApi(service.start),
        end: formatDateForApi(service.end),
        serviceName: service.serviceName,
        roomId: Number(service.roomId)
      };
      
      console.log('Updating service with data:', formattedService);
      
      // The ID is already included in the URL path
      await Api.updateService(serviceId, formattedService);
      
      navigate('/admin/services');
    } catch (err) {
      console.error('Error updating service:', err);
      setError(`Error when saving service: ${err.message}`);
    }
  };

  if (loading) return <Spinner animation="border" />;
  if (!service) return <Alert variant="danger">Service not found</Alert>;

  return (
    <div>
      <h2>Edit Service</h2>
      {error && <Alert variant="danger">{error}</Alert>}
      <Form onSubmit={handleSubmit}>
        <Row>
          <Col md={6}>
            <Form.Group className="mb-3">
              <Form.Label>Service Name</Form.Label>
              <Form.Control
                name="serviceName"
                value={service.serviceName || ''}
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
                value={service.roomId || ''}
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

        <Row>
          <Col md={6}>
            <Form.Group className="mb-3">
              <Form.Label>Start Time</Form.Label>
              <Form.Control
                name="start"
                type="datetime-local"
                value={service.start || ''}
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
                value={service.end || ''}
                onChange={handleChange}
                required
              />
            </Form.Group>
          </Col>
        </Row>

        <Button type="submit">Save Changes</Button>
      </Form>
    </div>
  );
};

export default EditService;
