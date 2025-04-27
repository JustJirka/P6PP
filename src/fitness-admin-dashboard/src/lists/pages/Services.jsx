import React, { useEffect, useState } from 'react';
import { Table, Button, Spinner, Alert, Dropdown, ButtonGroup, Container } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import Api from '../../utils/Api';

const Services = () => {
  const navigate = useNavigate();
  const [Services, setServices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchServices = async () => {
    try {
      setLoading(true);
      const res = await Api.getServices();
      setServices(res.data || []);
      setError(null);
    } catch (err) {
      if (err.message === 'Failed to fetch') {
        setError('Cannot contact the API server.');
      } else {
        setError(`Failed to load services: ${err.message}`);
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchServices();
  }, []);

  const handleEdit = (id) => {
    navigate(`/admin/services/edit/${id}`);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Do you really want to delete this service?')) return;

    try {
      await Api.deleteService(id);
      setServices(prev => prev.filter(b => b.id !== id));
      setError(null);
    } catch (err) {
      setError(`Error deleting service: ${err.message}`);
    }
  };

  const handleCreate = () => {
    navigate('/admin/services/create');
  };
  
  const handleRowClick = (id, e) => {
    // Pokud bylo kliknuto na dropdown menu nebo jeho prvky, nereaguj
    if (e.target.closest('.dropdown')) return;
    navigate(`/admin/services/view/${id}`);
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>My Services</h2>
        <Button onClick={handleCreate}>+ New service</Button>
      </div>

      {error && <Alert variant="danger">{error}</Alert>}
      {loading ? (
        <Spinner animation="border" />
      ) : (
        <Table hover responsive bordered className="align-middle">
          <thead>
            <tr>
              <th>ID</th>
              <th>Service name</th>
              <th>Start</th>
              <th>End</th>
              <th>Price</th>
              <th>Cancelled?</th>
              <th style={{ width: '60px' }}></th>
            </tr>
          </thead>
          <tbody>
            {Services.length === 0 ? (
              <tr>
                <td colSpan="7" className="text-center">No Services available</td>
              </tr>
            ) : (
              Services.map(b => (
                <tr 
                  key={b.id} 
                  onClick={(e) => handleRowClick(b.id, e)}
                  style={{ cursor: 'pointer' }}
                >
                  <td>{b.id}</td>
                  <td>{b.serviceName}</td>
                  <td>{formatDateTime(b.start)}</td>
                  <td>{formatDateTime(b.end)}</td>
                  <td>{b.price}</td>
                  <td>{b.isCancelled ? 'Yes' : 'No'}</td>
                  <td>
                    <Dropdown as={ButtonGroup}>
                      <Dropdown.Toggle variant="light" size="sm" className="border-0">⋯</Dropdown.Toggle>
                      <Dropdown.Menu align="end">
                        <Dropdown.Item onClick={() => handleEdit(b.id)}>Edit</Dropdown.Item>
                        <Dropdown.Item onClick={() => handleDelete(b.id)} className="text-danger">
                          Delete
                        </Dropdown.Item>
                      </Dropdown.Menu>
                    </Dropdown>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </Table>
      )}
    </div>
  );
};

// Pomocná funkce pro formátování data a času
const formatDateTime = (dateString) => {
  if (!dateString) return 'N/A';
  
  try {
    const date = new Date(dateString);
    return date.toLocaleString();
  } catch (error) {
    return dateString;
  }
};

export default Services;
