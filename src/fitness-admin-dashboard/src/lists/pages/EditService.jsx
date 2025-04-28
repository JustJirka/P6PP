import React, { useEffect, useState } from 'react';
import { Form, Button, Spinner, Alert, Row, Col, Container } from 'react-bootstrap';
import { useNavigate, useParams } from 'react-router-dom';
import Api from '../../utils/Api';

const EditService = () => {
  const { serviceId } = useParams();
  const navigate = useNavigate();
  const [service, setService] = useState(null);
  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        
        // Fetch service data
        const serviceData = await Api.get(`/booking-api/api/Services/${serviceId}`);
        const service = serviceData.data || serviceData;

        // Format dates for input
        if (service.start) {
          const startDate = new Date(service.start);
          service.start = startDate.toISOString().slice(0, 16); // Format as YYYY-MM-DDThh:mm
        }
        if (service.end) {
          const endDate = new Date(service.end);
          service.end = endDate.toISOString().slice(0, 16);
        }

        // Fetch rooms data
        const roomsResponse = await Api.getRooms();
        const rooms = roomsResponse.data || [];

        // Map room name to room ID if needed
        if (service.roomName) {
          const room = rooms.find(r => r.name === service.roomName);
          if (room) {
            service.roomId = room.id;
          }
        }

        setService(service);
        setRooms(rooms);
        setError(null);
      } catch (err) {
        console.error('Error fetching data:', err);
        if (err.message === 'Failed to fetch') {
          setError('Cannot contact the API server.');
        } else {
          setError(`Error loading service: ${err.message}`);
        }
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
      setSubmitting(true);
      setError(null);

      const formattedService = {
        id: Number(serviceId), // Přidat id do body
        start: formatDateForApi(service.start),
        end: formatDateForApi(service.end),
        serviceName: service.serviceName,
        roomId: Number(service.roomId)
      };

      console.log('Updating service with data:', formattedService);

      await Api.updateService(formattedService); // Posíláme celé tělo, ne id v URL

      navigate('/admin/services');
    } catch (err) {
      console.error('Error updating service:', err);
      setError(`Error when saving service: ${err.message}`);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <Container>
        <div className="text-center my-5">
          <Spinner animation="border" role="status">
            <span className="visually-hidden">Loading...</span>
          </Spinner>
        </div>
      </Container>
    );
  }

  if (!service) {
    return <Alert variant="danger">Service not found</Alert>;
  }

  return (
    <Container>
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

        <Button type="submit" disabled={submitting}>
          {submitting ? (
            <>
              <Spinner as="span" animation="border" size="sm" role="status" aria-hidden="true" />
              Saving...
            </>
          ) : (
            'Save Changes'
          )}
        </Button>
      </Form>
    </Container>
  );
};

export default EditService;
