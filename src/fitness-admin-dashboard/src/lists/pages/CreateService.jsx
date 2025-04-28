import React, { useState, useEffect } from 'react';
import { Form, Button, Alert, Row, Col, Container, Spinner } from 'react-bootstrap';
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
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);
  const [validationErrors, setValidationErrors] = useState({});

  useEffect(() => {
    const fetchRooms = async () => {
      try {
        setLoading(true);
        const response = await Api.getRooms();
        setRooms(response.data || []);
        setError(null);
      } catch (err) {
        if (err.message === 'Failed to fetch') {
          setError('Cannot contact the API server.');
        } else {
          setError(`Error loading rooms: ${err.message}`);
        }
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

    // Clear validation errors for this field when it changes
    setValidationErrors(prev => {
      const newErrors = { ...prev };
      delete newErrors[name];
      return newErrors;
    });

    setService(prev => ({ ...prev, [name]: finalValue }));
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
      setValidationErrors({});
  
      const formattedService = {
        start: formatDateForApi(service.start),
        end: formatDateForApi(service.end),
        price: Number(service.price),
        serviceName: service.serviceName,
        trainerId: Number(service.trainerId),
        roomId: Number(service.roomId)
      };
  
      const response = await Api.createService(formattedService);
  
      if (response && !response.success && response.errors) {
        setValidationErrors(response.errors);
        return;
      }
  
      navigate('/admin/services');
  
    } catch (err) {
      console.error('Failed to create service:', err);
  
      if (err.status === 400 && err.data?.errors) {
        setValidationErrors(err.data.errors);
      } else if (err.message === 'Failed to fetch') {
        setError('Cannot contact the API server.');
      } else {
        setError(`Error creating service: ${err.message || 'Unknown error'}`);
      }
    } finally {
      setSubmitting(false);
    }
  };
  

  // Helper function to get validation error for a field
  const getFieldError = (fieldName) => {
    return validationErrors[fieldName]?.[0];
  };

  // Helper function to determine if a field has an error
  const hasError = (fieldName) => {
    return !!validationErrors[fieldName];
  };

  // Helper function to determine if there's a specific field error or error is about that field
  const hasFieldError = (fieldName) => {
    if (validationErrors[fieldName]) return true;
    if (error && error.toLowerCase().includes(fieldName.toLowerCase())) return true;
    return false;
  };

  return (
    <Container>
      <h2>New Service</h2>
      {error && <Alert variant="danger">{error}</Alert>}
      
      {/* Display validation errors summary if any */}
      {Object.keys(validationErrors).length > 0 && (
        <Alert variant="danger">
          <ul className="mb-0">
            {Object.entries(validationErrors).map(([field, errors]) => (
              errors.map((errorMsg, index) => (
                <li key={`${field}-${index}`}><strong>{field}:</strong> {errorMsg}</li>
              ))
            ))}
          </ul>
        </Alert>
      )}
      
      {loading ? (
        <div className="text-center my-5">
          <Spinner animation="border" role="status">
            <span className="visually-hidden">Loading...</span>
          </Spinner>
        </div>
      ) : (
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
                  isInvalid={hasFieldError('serviceName')}
                />
                {hasError('serviceName') && (
                  <Form.Control.Feedback type="invalid">
                    {getFieldError('serviceName')}
                  </Form.Control.Feedback>
                )}
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
                  isInvalid={hasFieldError('price')}
                />
                {hasError('price') && (
                  <Form.Control.Feedback type="invalid">
                    {getFieldError('price')}
                  </Form.Control.Feedback>
                )}
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
                  isInvalid={hasFieldError('start') || hasFieldError('Start')}
                />
                {(hasError('start') || hasError('Start')) && (
                  <Form.Control.Feedback type="invalid">
                    {getFieldError('start') || getFieldError('Start')}
                  </Form.Control.Feedback>
                )}
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
                  isInvalid={hasFieldError('end') || hasFieldError('End')}
                />
                {(hasError('end') || hasError('End')) && (
                  <Form.Control.Feedback type="invalid">
                    {getFieldError('end') || getFieldError('End')}
                  </Form.Control.Feedback>
                )}
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
                  isInvalid={hasFieldError('trainerId') || hasFieldError('TrainerId')}
                />
                {(hasError('trainerId') || hasError('TrainerId')) && (
                  <Form.Control.Feedback type="invalid">
                    {getFieldError('trainerId') || getFieldError('TrainerId')}
                  </Form.Control.Feedback>
                )}
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
                  isInvalid={hasFieldError('roomId') || hasFieldError('RoomId')}
                >
                  <option value="">Select a room</option>
                  {rooms.map(room => (
                    <option key={room.id} value={room.id}>
                      {room.name}
                    </option>
                  ))}
                </Form.Select>
                {(hasError('roomId') || hasError('RoomId')) && (
                  <Form.Control.Feedback type="invalid">
                    {getFieldError('roomId') || getFieldError('RoomId')}
                  </Form.Control.Feedback>
                )}
              </Form.Group>
            </Col>
          </Row>

          <Button 
            variant="primary" 
            type="submit" 
            disabled={submitting}
          >
            {submitting ? (
              <>
                <Spinner as="span" animation="border" size="sm" role="status" aria-hidden="true" /> 
                Creating...
              </>
            ) : (
              'Create Service'
            )}
          </Button>
        </Form>
      )}
    </Container>
  );
};

export default CreateService;
