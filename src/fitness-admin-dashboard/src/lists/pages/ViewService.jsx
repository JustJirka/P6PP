import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Spinner, Alert, Card, Button, Badge, Row, Col, Container } from 'react-bootstrap';
import Api from '../../utils/Api';

const ViewService = () => {
  const { serviceId } = useParams();
  const navigate = useNavigate();
  const [service, setService] = useState(null);
  const [room, setRoom] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchService = async () => {
      try {
        setLoading(true);
        // Načtení služby
        const serviceData = await Api.get(`/booking-api/api/Services/${serviceId}`);
        console.log('Service from API:', serviceData);
        
        const serviceInfo = serviceData.data || serviceData;
        setService(serviceInfo);
        
        // Pokud služba má roomId, načteme také informace o místnosti
        if (serviceInfo.roomId) {
          try {
            const roomData = await Api.get(`/booking-api/api/Rooms/${serviceInfo.roomId}`);
            setRoom(roomData.data || roomData);
          } catch (roomErr) {
            console.error('Error loading room:', roomErr);
            // Nebudeme to považovat za fatální chybu, jen nezobrazíme informace o místnosti
          }
        }

        setError(null);
      } catch (err) {
        if (err.message === 'Failed to fetch') {
          setError('Cannot contact the API server.');
        } else {
          setError(`Error loading service: ${err.message}`);
        }
      } finally {
        setLoading(false);
      }
    };

    fetchService();
  }, [serviceId]);

  const handleEdit = () => {
    navigate(`/admin/services/edit/${serviceId}`);
  };

  const handleBack = () => {
    navigate('/admin/services');
  };

  const formatDateTime = (dateString) => {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleString();
  };

  if (loading) return (
    <Container>
      <div className="text-center my-5">
        <Spinner animation="border" role="status">
          <span className="visually-hidden">Loading...</span>
        </Spinner>
      </div>
    </Container>
  );

  return (
    <Container>
      {error && <Alert variant="danger">{error}</Alert>}
      {!service && !error && <Alert variant="warning">Service not found</Alert>}
      
      {service && (
        <>
          <div className="d-flex justify-content-between align-items-center mb-4">
            <h2>Service Detail</h2>
            <div>
              <Button variant="primary" className="me-2" onClick={handleEdit}>
                Edit
              </Button>
              <Button variant="secondary" onClick={handleBack}>
                Back
              </Button>
            </div>
          </div>

          <Card className="mb-4">
            <Card.Header>
              <h4>{service.serviceName || 'Unnamed Service'}</h4>
            </Card.Header>
            <Card.Body>
              <Row>
                <Col md={6} className="mb-3">
                  <h5>Basic Information</h5>
                  <p><strong>ID:</strong> {service.id}</p>
                  <p><strong>Service Name:</strong> {service.serviceName || 'N/A'}</p>
                  <p>
                    <strong>Status:</strong>{' '}
                    {service.isCancelled ? (
                      <Badge bg="danger">Cancelled</Badge>
                    ) : (
                      <Badge bg="success">Active</Badge>
                    )}
                  </p>
                  {service.price !== undefined && (
                    <p><strong>Price:</strong> {service.price} Kč</p>
                  )}
                  {service.trainerId !== undefined && (
                    <p><strong>Trainer ID:</strong> {service.trainerId}</p>
                  )}
                </Col>
                
                <Col md={6} className="mb-3">
                  <h5>Schedule</h5>
                  <p><strong>Start Time:</strong> {formatDateTime(service.start)}</p>
                  <p><strong>End Time:</strong> {formatDateTime(service.end)}</p>
                  {service.duration && (
                    <p><strong>Duration:</strong> {service.duration} minutes</p>
                  )}
                </Col>
              </Row>

              {room && (
                <div className="mt-3">
                  <h5>Room Information</h5>
                  <p><strong>Room:</strong> {room.name} (ID: {room.id})</p>
                  <p><strong>Capacity:</strong> {room.capacity} people</p>
                  <p><strong>Status:</strong> {room.status}</p>
                </div>
              )}
              
              {service.description && (
                <div className="mt-3">
                  <h5>Description</h5>
                  <p>{service.description}</p>
                </div>
              )}
            </Card.Body>
          </Card>
        </>
      )}
    </Container>
  );
};

export default ViewService;
